using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader
{
    public static class DataValidator
    {
        public static bool checkGeneralArrayIsValid(double[][] input, bool ingoreNan)
        {
            int rowLength = input[0].Length;
            foreach (double[] row in input)
            {
                if (row.Length != rowLength)
                    return false;
                else
                    foreach (double d in row)
                        if ((double.IsNaN(d) && ingoreNan == false) || d == double.MinValue || double.MaxValue == d || double.IsInfinity(d))
                            return false;
            }

            return true;
        }

        public static void checkPriceDataArray(double[][] input)
        {
            int maxPercentJump = 15;
            int maxDayJump = 5;

            DateTime lastDate = Timestamp.getDate(Convert.ToInt64(input[0][(int)PriceDataIndeces.Date]));
            double lastBid = input[0][(int)PriceDataIndeces.Bid],
                lastAsk = input[0][(int)PriceDataIndeces.Ask],
                lastVolume = input[0][(int)PriceDataIndeces.Volume];

            foreach (double[] row in input)
            {
                DateTime currentTime = Timestamp.getDate(Convert.ToInt64(row[(int)PriceDataIndeces.Date]));
                double bid = row[(int)PriceDataIndeces.Bid];
                double ask = row[(int)PriceDataIndeces.Ask];
                double volume = row[(int)PriceDataIndeces.Volume];

                if (double.IsNaN(bid) || double.IsInfinity(bid) || bid == double.MinValue || bid == double.MaxValue || bid == 0d)
                    throw new Exception("Bad bid: " + bid);

                if (double.IsNaN(ask) || double.IsInfinity(ask) || ask == double.MinValue || ask == double.MaxValue || ask == 0d)
                    throw new Exception("Bad ask: " + ask);

                if (double.IsNaN(volume) || double.IsInfinity(volume) || volume == double.MinValue || volume == double.MaxValue || volume == 0d)
                    throw new Exception("Bad volume: " + volume);

                double bidJump = ((bid / lastBid) - 1) * 100;
                if (bidJump > maxPercentJump)
                    throw new Exception("Bid jump: " + bidJump);

                double askJump = ((ask / lastAsk) - 1) * 100;
                if (askJump > maxPercentJump)
                    throw new Exception("Ask jump: " + bidJump);

                double dayJump = (currentTime - lastDate).TotalDays;
                if (dayJump > maxDayJump)
                    throw new Exception("Time Jump: " + dayJump);

                lastAsk = ask;
                lastBid = bid;
                lastDate = currentTime;
                lastVolume = volume; //Did not check volume
            }
        }
    }
}
