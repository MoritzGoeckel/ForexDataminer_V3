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
                throw new Exception("#1 - Minmax is not valid: " + min + " " + max + " " + upperP + " " + lowerP);
        }

        public static void getMinMax(double[] input, out double min, out double max)
        {
            min = double.MaxValue;
            max = double.MinValue;

            foreach (double d in input)
            {
                if (d < min)
                    min = d;

                if (d > max)
                    max = d;
            }

            if (min == double.MaxValue || max == double.MinValue)
                throw new Exception("#2 - Minmax is not valid: " + min + " " + max);
        }

        public static void getSampleOutcomeCodesBuyMaxSellMax(double[][] sampleData, double minValuesPercentThreshold, out double maxBuy, out double maxBuyValuesCount, out double maxSell, out double maxSellValuesCount)
        {
            maxBuy = double.MinValue;
            maxSell = double.MinValue;
            maxBuyValuesCount = double.NaN;
            maxSellValuesCount = double.NaN;

            double sumAllSamples = 0;
            foreach (double[] row in sampleData)
            {
                if (row != null)
                    sumAllSamples += row[(int)SampleValuesOutcomeCodesIndices.SamplesCount];
            }
            
            foreach (double[] row in sampleData)
            {
                if (row != null)
                {
                    double samplesCount = row[(int)SampleValuesOutcomeCodesIndices.SamplesCount];

                    if (samplesCount / sumAllSamples >= minValuesPercentThreshold / 100d)
                    {
                        double b = row[(int)SampleValuesOutcomeCodesIndices.BuyRatio];
                        if (b > maxBuy)
                        {
                            maxBuy = b;
                            maxBuyValuesCount = samplesCount;
                        }

                        double s = row[(int)SampleValuesOutcomeCodesIndices.SellRatio];
                        if (s > maxSell)
                        {
                            maxSell = s;
                            maxSellValuesCount = samplesCount;
                        }
                    }
                }
            }

            if (maxBuy == double.MinValue || maxSell == double.MinValue || double.IsNaN(maxBuyValuesCount) || double.IsNaN(maxSellValuesCount))
                throw new Exception("Nothing found :(");
        }

        public static void getSampleOutcomesMinMax(double[][] sampleData, double minValuesPercentThreshold, out double maxMax, out double maxMaxValuesCount, out double minMin, out double minMinValuesCount, out double maxMinMaxDistance, out double maxMinMaxDistanceValuesCount, out double maxActual, out double maxActualValuesCount, out double minActual, out double minActualValuesCount)
        {
            maxMax = double.MinValue;
            minMin = double.MaxValue;
            maxActual = double.MinValue;
            minActual = double.MaxValue;
            maxMinMaxDistance = double.MinValue;

            maxMaxValuesCount = double.NaN;
            minMinValuesCount = double.NaN;
            maxActualValuesCount = double.NaN;
            minActualValuesCount = double.NaN;
            maxMinMaxDistanceValuesCount = double.NaN;

            double sumAllSamples = 0;
            foreach (double[] row in sampleData)
            {
                if (row != null)
                    sumAllSamples += row[(int)SampleValuesOutcomeIndices.SamplesCount];
            }
            
            foreach (double[] row in sampleData)
            {
                if (row != null)
                {
                    double valuesCount = row[(int)SampleValuesOutcomeIndices.SamplesCount];

                    if (valuesCount / sumAllSamples >= minValuesPercentThreshold / 100d)
                    {
                        double maxAvg = row[(int)SampleValuesOutcomeIndices.MaxAvg];
                        if (maxAvg > maxMax)
                        {
                            maxMax = maxAvg;
                            maxMaxValuesCount = valuesCount;
                        }

                        double minAvg = row[(int)SampleValuesOutcomeIndices.MinAvg];
                        if (minAvg < minMin)
                        {
                            minMin = minAvg;
                            minMinValuesCount = valuesCount;
                        }

                        double actualAvg = row[(int)SampleValuesOutcomeIndices.ActualAvg];
                        if (actualAvg > maxActual)
                        {
                            maxActual = actualAvg;
                            maxActualValuesCount = valuesCount;
                        }

                        if (actualAvg < minActual)
                        {
                            minActual = actualAvg;
                            minActualValuesCount = valuesCount;
                        }

                        double distance = Math.Abs(Math.Abs(minAvg) - maxAvg);
                        if (distance > maxMinMaxDistance)
                        {
                            maxMinMaxDistance = distance;
                            maxMinMaxDistanceValuesCount = valuesCount;
                        }
                    }
                }
            }

            if (maxMax == double.MinValue || minMin == double.MaxValue 
                || maxActual == double.MinValue || minActual == double.MaxValue 
                || double.IsNaN(maxMaxValuesCount) || double.IsNaN(minMinValuesCount)
                || double.IsNaN(maxActualValuesCount) || double.IsNaN(minActualValuesCount)
                || double.IsNaN(maxMinMaxDistanceValuesCount))
                    throw new Exception("Nothing found :(");
        }
    }
}
