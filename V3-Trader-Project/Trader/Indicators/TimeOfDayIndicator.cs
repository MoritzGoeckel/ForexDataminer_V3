using System;
using System.Collections.Generic;
using V3_Trader_Project.Trader;

namespace NinjaTrader_Client.Trader.Indicators
{
    class TimeOfDayIndicator : WalkerIndicator
    {
        public const string Name = "TimeOfDayIndicator";

        public TimeOfDayIndicator()
        {
            
        }

        long currentTime = 0;

        public override double getIndicator()
        {
            DateTime dt = Timestamp.getDate(currentTime);
            return Convert.ToInt32(Convert.ToDouble(dt.Hour) / 24d);
        }

        public override void setNextData(long timestamp, double value)
        {
            currentTime = timestamp;
        }

        public override string getName()
        {
            return Name;
        }

        public override bool isValid(long timestamp)
        {
            return true;
        }

        public override WalkerIndicator Clone()
        {
            return new TimeOfDayIndicator();
        }
    }
}
