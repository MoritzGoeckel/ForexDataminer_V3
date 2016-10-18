using System;
using System.Collections.Generic;
using V3_Trader_Project.Trader;

namespace NinjaTrader_Client.Trader.Indicators
{
    class TimeDayOfWeekIndicator : WalkerIndicator
    {
        public TimeDayOfWeekIndicator()
        {
            
        }

        long currentTime = 0;

        public override double getIndicator()
        {
            DateTime dt = Timestamp.getDate(currentTime);
            return (dt.DayOfWeek != 0 ? Convert.ToDouble(dt.DayOfWeek) / 7d : 0);
        }

        public override void setNextData(long timestamp, double value)
        {
            currentTime = timestamp;
        }

        public override string getName()
        {
            return "TimeDayOfWeek";
        }

        public override bool isValid(long timestamp)
        {
            return true;
        }
    }
}
