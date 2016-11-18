using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Market
{
    class OrderHistoryTimeAnalysis
    {
        public static double getHistoricProfitabilityWight(List<ClosedPosition> history, long timeNow)
        {
            double[] days = getDayInWeekProfitPerTrade(history);
            double[] hours = getTimeOfDayProfitPerTrade(history);

            DateTime now = Timestamp.getDate(timeNow);
            double day = days[getDayOfWeek(now.DayOfWeek)];
            double hour = hours[now.Hour];

            double avg = getAvgProfitPerTrade(history);

            double wight = 1;

            //punish unprofitable times
            wight += day < 0 ? -0.5 : 0;
            wight += hour < 0 ? -0.5 : 0;

            //reward very profitable times
            wight += day >= avg ? 0.5 : 0;
            wight += hour >= avg ? 0.5 : 0;

            if (wight <= 0.1)
                return 0.1;

            return wight;
        }

        private static double getAvgProfitPerTrade(List<ClosedPosition> history)
        {
            double profit = 0;
            foreach (ClosedPosition p in history)
            {
                profit += p.getProfitIngoreAmount();
            }
            profit /= history.Count;

            return profit;
        }

        private static double[] getDayInWeekProfitPerTrade(List<ClosedPosition> history)
        {
            double[] days = new double[7];
            int[] tradesCount = new int[7];

            for (int i = 0; i < days.Length; i++)
            {
                days[i] = 0d;
                tradesCount[i] = 0;
            }

            foreach (ClosedPosition p in history)
            {
                DateTime dt = Timestamp.getDate(p.timestampOpen);
                int index = getDayOfWeek(dt.DayOfWeek);
                days[index] += p.getProfitIngoreAmount();
                tradesCount[index]++;
            }

            for (int i = 0; i < days.Length; i++)
                days[i] /= tradesCount[i];

                return days;
        }

        private static double[] getTimeOfDayProfitPerTrade(List<ClosedPosition> history)
        {
            double[] hours = new double[24];
            int[] tradesCount = new int[24];

            for (int i = 0; i < hours.Length; i++)
            {
                hours[i] = 0d;
                tradesCount[i] = 0;
            }

            foreach (ClosedPosition p in history)
            {
                DateTime dt = Timestamp.getDate(p.timestampOpen);
                hours[dt.Hour] += p.getProfitIngoreAmount();
                tradesCount[dt.Hour]++;
            }

            for (int i = 0; i < hours.Length; i++)
                hours[i] /= tradesCount[i];

            return hours;
        }

        private static int getDayOfWeek(DayOfWeek d)
        {
            switch (d)
            {
                case DayOfWeek.Monday:
                    return 0;

                case DayOfWeek.Tuesday:
                    return 1;

                case DayOfWeek.Wednesday:
                    return 2;

                case DayOfWeek.Thursday:
                    return 3;

                case DayOfWeek.Friday:
                    return 4;

                case DayOfWeek.Saturday:
                    return 5;

                case DayOfWeek.Sunday:
                    return 6;

                default: throw new Exception("Day not found: " + d);
            }
        }
    }
}
