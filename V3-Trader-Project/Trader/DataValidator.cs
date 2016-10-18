using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader
{
    public class DataValidator
    {
        public void checkGeneralArray(double[][] input)
        {
            int rowLength = input[0].Length;
            foreach (double[] row in input)
            {
                if (row.Length != rowLength)
                    throw new Exception("Bad row length");
                else
                    foreach (double d in row)
                        if (d == double.NaN || d == double.MinValue || double.MaxValue == d || double.IsInfinity(d))
                            throw new Exception("Bad value: " + d);
            }
        }

        public void checkPriceDataArray(double[][] input)
        {
            int maxPercentJump = 15;
            int maxDayJump = 5;

            DateTime lastDate = Timestamp.getDate(Convert.ToInt64(input[0][(int)DataIndeces.Date]));
            double lastBid = input[0][(int)DataIndeces.Bid],
                lastAsk = input[0][(int)DataIndeces.Ask],
                lastVolume = input[0][(int)DataIndeces.Volume];

            foreach (double[] row in input)
            {
                DateTime currentTime = Timestamp.getDate(Convert.ToInt64(row[(int)DataIndeces.Date]));
                double bid = row[(int)DataIndeces.Bid];
                double ask = row[(int)DataIndeces.Ask];
                double volume = row[(int)DataIndeces.Volume];

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
