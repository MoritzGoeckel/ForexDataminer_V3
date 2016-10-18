using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NinjaTrader_Client.Trader.Indicators
{
    public abstract class WalkerIndicator
    {
        public WalkerIndicator()
        {
            
        }

        public abstract void setNextData(long timestamp, double value);
        public abstract double getIndicator();

        public struct TimestampValuePair
        {
            public long timestamp;
            public double value;
        }

        public abstract string getName();

        public abstract bool isValid(long timestamp);

        public double setNextDataAndGetIndicator(long timestamp, double value)
        {
            setNextData(timestamp, value);
            return getIndicator();
        }
    }
}
