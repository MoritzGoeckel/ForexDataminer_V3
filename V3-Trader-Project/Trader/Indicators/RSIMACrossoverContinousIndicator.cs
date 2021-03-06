﻿using System;
using System.Collections.Generic;

namespace NinjaTrader_Client.Trader.Indicators
{
    class RSIMACrossoverContinousIndicator : WalkerIndicator
    {
        private long rsiTimeframe, signalTimeframe;
        private RSIIndicator rsi;
        private MovingAverageIndicator signalMa;

        public const string Name = "RSIMACrossoverContinousIndicator";

        public RSIMACrossoverContinousIndicator(long rsiTimeframe, long signalTimeframe)
        {
            this.rsiTimeframe = rsiTimeframe;
            this.signalTimeframe = signalTimeframe;
            rsi = new RSIIndicator(rsiTimeframe);
            signalMa = new MovingAverageIndicator(signalTimeframe);
        }

        double valueNow;
        long timestampNow;
        public override void setNextData(long _timestamp, double _value)
        {
            if (_timestamp < timestampNow)
                throw new Exception("Cant add older data here!");

            if (_timestamp == timestampNow && _value != valueNow)
                throw new Exception("Same timestamp different value!");

            if (_timestamp == timestampNow && _value == valueNow)
                return;

            timestampNow = _timestamp;
            valueNow = _value;
            
            rsi.setNextData(_timestamp, _value);
            signalMa.setNextData(_timestamp, rsi.getIndicator());
        }

        public override double getIndicator()
        {
            return rsi.getIndicator() - signalMa.getIndicator();
        }

        public override string getName()
        {
            return Name + "_" + rsiTimeframe + "_" + signalTimeframe;
        }

        public override bool isValid(long timestamp)
        {
            return rsi.isValid(timestamp) && signalMa.isValid(timestamp);
        }

        public override WalkerIndicator Clone()
        {
            return new RSIMACrossoverIndicator(rsiTimeframe, signalTimeframe);
        }
    }
}
