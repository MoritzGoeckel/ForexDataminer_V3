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
            List<double> valuesNoNans = new List<double>();
            foreach (double d in input)
                if (double.IsNaN(d) == false)
                    valuesNoNans.Add(d);

            int upperP = Convert.ToInt32(100d - Convert.ToDouble(percentToDrop) / 2d);
            max = valuesNoNans.Percentile(upperP);

            int lowerP = Convert.ToInt32(percentToDrop / 2d);
            min = valuesNoNans.Percentile(lowerP);

            if (double.IsNaN(min) || double.IsNaN(max))
                throw new Exception("Minmax is not valid: " + min + " " + max + " " + upperP + " " + lowerP);
        }

        public static void getSampleOutcomeCodesBuyMaxSellMax(double[][] sampleData, out double maxBuy, out double maxSell)
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

            if (maxBuy == double.MinValue || maxSell == double.MinValue)
                throw new Exception("Nothing found :(");
        }

        public static void getSampleOutcomesMinMax(double[][] sampleData, out double maxMax, out double minMin, out double maxActual, out double minActual)
        {
            maxMax = double.MinValue;
            minMin = double.MaxValue;
            maxActual = double.MinValue;
            minActual = double.MaxValue;

            foreach (double[] row in sampleData)
            {
                if (row != null)
                {
                    double maxAvg = row[(int)SampleValuesOutcomeIndices.MaxAvg];
                    if (maxAvg > maxMax)
                        maxMax = maxAvg;

                    double minAvg = row[(int)SampleValuesOutcomeIndices.MinAvg];
                    if (minAvg < minMin)
                        minMin = minAvg;

                    double actualAvg = row[(int)SampleValuesOutcomeIndices.ActualAvg];
                    if (actualAvg > maxActual)
                        maxActual = actualAvg;

                    if (actualAvg < minActual)
                        minActual = actualAvg;
                }
            }

            if (maxMax == double.MinValue || minMin == double.MaxValue || maxActual == double.MinValue || minActual == double.MaxValue)
                throw new Exception("Nothing found :(");
        }
    }
}
