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
            DateTime dt = Timestamp.getDate(currentTime);

            int day = 0;
            switch(dt.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    day = 0;
                    break;

                case DayOfWeek.Tuesday:
                    day = 1;
                    break;

                case DayOfWeek.Wednesday:
                    day = 2;
                    break;

                case DayOfWeek.Thursday:
                    day = 3;
                    break;

                case DayOfWeek.Friday:
                    day = 4;
                    break;

                case DayOfWeek.Saturday:
                    day = 5;
                    break;

                case DayOfWeek.Sunday:
                    day = 6;
                    break;
            }

            return Convert.ToInt32(Convert.ToDouble(day) / 6d);
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
