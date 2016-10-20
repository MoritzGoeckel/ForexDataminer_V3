using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader
{
    public static class DataValidator
    {
        public static bool checkGeneralArrayIsValid(double[][] input, bool ingoreNan, bool ingoreNullRow, out string msg)
        {
            int rowLength = input[0].Length;
            foreach (double[] row in input)
            {
                if (row != null)
                {
                    if (row.Length != rowLength)
                    {
                        msg = "Bad length: " + row.Length + " != " + rowLength;
                        return false;
                    }
                    else
                        foreach (double d in row)
                            if ((double.IsNaN(d) && ingoreNan == false) || d == double.MinValue || double.MaxValue == d || double.IsInfinity(d))
                            {
                                msg = "Bad value: " + d;
                                return false;
                            }
                }
                else //Row == null
                {
                    if(ingoreNullRow == false)
                    {
                        msg = "Null row";
                        return false;
                    }
                }
            }

            msg = "OK";
            return true;
        }

        public static bool checkPriceDataArray(double[][] input, out string msg)
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

                if (currentTime < lastDate)
                {
                    msg = "Bad date, expired already!";
                    return false;
                }

                if (double.IsNaN(bid) || double.IsInfinity(bid) || bid == double.MinValue || bid == double.MaxValue || bid == 0d)
                {
                    msg = "Bad bid: " + bid;
                    return false;
                }

                if (double.IsNaN(ask) || double.IsInfinity(ask) || ask == double.MinValue || ask == double.MaxValue || ask == 0d)
                {
                    msg = "Bad ask: " + ask;
                    return false;
                }

                if (double.IsNaN(volume) || double.IsInfinity(volume) || volume == double.MinValue || volume == double.MaxValue)
                {
                    msg = "Bad volume: " + volume;
                    return false;
                }

                double bidJump = ((bid / lastBid) - 1) * 100;
                if (bidJump > maxPercentJump)
                {
                    msg = "Bid jump: " + bidJump;
                    return false;
                }

                double askJump = ((ask / lastAsk) - 1) * 100;
                if (askJump > maxPercentJump)
                {
                    msg = "Bid jump: " + bidJump;
                    return false;
                }

                double dayJump = (currentTime - lastDate).TotalDays;
                if (dayJump > maxDayJump)
                {
                    msg = "Time jump: " + dayJump;
                    return false;
                }

                lastAsk = ask;
                lastBid = bid;
                lastDate = currentTime;
                lastVolume = volume; //Did not check volume
            }

            msg = "OK";
            return true;
        }
    }
}
