using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;

namespace V3_Trader_Project.Trader
{
    public static class DistributionHelper
    {
        public static void getOutcomeCodeDistribution(bool[][] outcomeCodes, out double buyRatio, out double sellRatio)
        {
            double countedBuysSells = 0;
            double buys = 0, sells = 0;
            foreach (bool[] row in outcomeCodes)
            {
                if (row != null)
                {
                    buys += row[(int)OutcomeCodeMatrixIndices.Buy] ? 1 : 0;
                    sells += row[(int)OutcomeCodeMatrixIndices.Sell] ? 1 : 0;
                    countedBuysSells++;
                }
            }

            buyRatio = buys / countedBuysSells;
            sellRatio = sells / countedBuysSells;
        }

        public static void getMinMax(double[] input, int percentToDrop, out double min, out double max)
        {
            max = input.Percentile(100 - percentToDrop / 2);
            min = input.Percentile(percentToDrop / 2);
        }

        public static void getSampleCodesMinMax(double[][] sampleData, out double maxBuy, out double maxSell)
        {
            maxBuy = double.MinValue;
            maxSell = double.MinValue;
            
            foreach(double[] row in sampleData)
            {
                if (row != null)
                {
                    double b = row[(int)SampleValuesOutcomeCodesIndices.BuyRatio];
                    if (b > maxBuy)
                        maxBuy = b;

                    double s = row[(int)SampleValuesOutcomeCodesIndices.SellRatio];
                    if (s > maxSell)
                        maxSell = s;
                }
            }
        }
    }
}
