using System;
using System.Collections.Generic;
using V3_Trader_Project.Trader;

namespace NinjaTrader_Client.Trader.Indicators
{
    class TimeOfDayIndicator : WalkerIndicator
    {
        public TimeOfDayIndicator()
        {
            
        }

        long currentTime = 0;

        public override double getIndicator()
        {
            DateTime dt = Timestamp.getDate(currentTime);
            return (dt.Hour != 0 ? Convert.ToDouble(dt.Hour) / 24d : 0);
        }

        public override void setNextData(long timestamp, double value)
        {
            currentTime = timestamp;
        }

        public override string getName()
        {
            return "TimeOfDay";
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
