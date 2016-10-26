using System;
using System.Collections.Generic;
using V3_Trader_Project.Trader;

namespace NinjaTrader_Client.Trader.Indicators
{
    class TimeDayOfWeekIndicator : WalkerIndicator
    {
        public const string Name = "TimeDayOfWeekIndicator";

        public TimeDayOfWeekIndicator()
        {
            
        }

        long currentTime = 0;

        public override double getIndicator()
        {
            DateTime dt = Timestamp.getDate(currentTime).ToUniversalTime();
            double v = (dt.DayOfWeek != 0 ? Convert.ToDouble(dt.DayOfWeek) / 7d : 0);
            return v;
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
            return new TimeDayOfWeekIndicator();
        }
    }
}
