﻿using System;
using System.Collections.Generic;

namespace NinjaTrader_Client.Trader.Indicators
{
    class RangeIndicator : WalkerIndicator
    {
        private long timeframe;
        private List<TimestampValuePair> history = new List<TimestampValuePair>();

        public const string Name = "RangeIndicator";
        
        public RangeIndicator(long timeframe)
        {
            this.timeframe = timeframe;
        }

        private void getMinMaxInPrices(ref double min, ref double max, List<TimestampValuePair> data)
        {
            min = double.MaxValue;
            max = double.MinValue;

            foreach (TimestampValuePair tick in data)
            {
                if (tick.value > max)
                    max = tick.value;

                if (tick.value < min)
                    min = tick.value;
            }

            if (data.Count == 0 || min == double.MaxValue || max == double.MinValue)
                throw new Exception("Range Indicator getMinMaxInData<TickData>: data.Count == " + data.Count + " or min->" + (min == double.MaxValue ? "ns" : "okay") + " or max->" + (max == double.MinValue ? "ns" : "okay"));
        }

        long timestampNow;
        double valueNow;
        public override void setNextData(long _timestamp, double _value)
        {
            if (_timestamp < timestampNow)
                throw new Exception("Cant add older data here!");

            if (_timestamp == timestampNow && _value != valueNow)
                throw new Exception("Same timestamp different value!");

            if (_timestamp == timestampNow && _value == valueNow)
                return;

            history.Add(new TimestampValuePair { timestamp = _timestamp, value = _value });
            timestampNow = _timestamp;
            valueNow = _value;

            if (valueNow > cachedMax)
                cachedMax = valueNow;

            if (valueNow < cachedMin)
                cachedMin = valueNow;
        }

        double cachedMin = double.MaxValue, cachedMax = double.MinValue;
        public override double getIndicator()
        {
            while (history.Count != 0 && history[0].timestamp < timestampNow - timeframe)
            {
                if (history[0].value == cachedMin)
                    cachedMin = double.MaxValue;

                if (history[0].value == cachedMax)
                    cachedMax = double.MinValue;

                history.RemoveAt(0);
            }

            if(cachedMax == double.MinValue || cachedMin == double.MaxValue && history.Count != 0)
            {
                getMinMaxInPrices(ref cachedMin, ref cachedMax, history);
            }

            double max = cachedMax;
            double min = cachedMin;

            double output;

            if (history.Count == 0 || max == min)
                output = double.NaN; //Invalid
            else
                output = (max - min) / valueNow;

            return output;
        }

        public override string getName()
        {
            return Name + "_" + timeframe;
        }

        public override bool isValid(long timestamp)
        {
            if (history.Count == 0)
                return false;

            TimestampValuePair pair = history[0];
            if (timestamp - pair.timestamp > timeframe - (timeframe * 0.2)) //Ältester Datensatz ist älter als die (timeframe - 10% Timeframe) 
                return true;
            else
                return false;
        }

        public override WalkerIndicator Clone()
        {
            return new RangeIndicator(timeframe);
        }
    }
}
