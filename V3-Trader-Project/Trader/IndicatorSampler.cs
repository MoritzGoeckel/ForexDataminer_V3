using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader
{
    public enum SampleValuesOutcomeCodesIndices{
        Start = 0, BuyRatio = 1, SellRatio = 2, SamplesCount = 3
    };

    public enum SampleValuesOutcomeIndices
    {
        Start = 0, MinAvg = 1, MaxAvg = 2, ActualAvg = 3, SamplesCount = 4
    };

    public static class IndicatorSampler
    {
        public static void getStatisticsOutcomeCodes(double[] values, bool[][] outcomeCodes, out double spearmanBuy, out double spearmanSell, out double pearsonBuy, out double pearsonSell)
        {
            if (values.Length != outcomeCodes.Length)
                throw new Exception("Arrays have to be the same size: " + values.Length + " != " + outcomeCodes.Length);

            List<double> buyList = new List<double>();
            List<double> sellList = new List<double>();
            List<double> valuesList = new List<double>();

            for (int i = 0; i < outcomeCodes.Length; i++)
            {
                if (outcomeCodes[i] != null)
                {
                    buyList.Add(outcomeCodes[i][(int)OutcomeCodeMatrixIndices.Buy] ? 1 : 0);
                    sellList.Add(outcomeCodes[i][(int)OutcomeCodeMatrixIndices.Sell] ? 1 : 0);
                    valuesList.Add(values[i]);
                }
            }

            spearmanBuy = Correlation.Spearman(valuesList, buyList);
            spearmanSell = Correlation.Spearman(valuesList, sellList);

            pearsonBuy = Correlation.Pearson(valuesList, buyList);
            pearsonSell = Correlation.Pearson(valuesList, sellList);
        }

        public static void getStatisticsOutcomes(double[] values, double[][] prices, double[][] outcomes, out double spearmanMin, out double spearmanMax, out double spearmanActual, out double pearsonMin, out double pearsonMax, out double pearsonActual)
        {
            if (values.Length != outcomes.Length)
                throw new Exception("Arrays have to be the same size: " + values.Length + " != " + outcomes.Length);

            List<double> minList = new List<double>();
            List<double> maxList = new List<double>();
            List<double> actualList = new List<double>();
            List<double> valuesList = new List<double>();

            for (int i = 0; i < outcomes.Length; i++)
            {
                if (outcomes[i] != null)
                {
                    double mid = (prices[i][(int)PriceDataIndeces.Ask] + prices[i][(int)PriceDataIndeces.Bid]) / 2d;

                    double minChange = ((outcomes[i][(int)OutcomeMatrixIndices.Min] / mid) - 1d) * 100;
                    double maxChange = ((outcomes[i][(int)OutcomeMatrixIndices.Max] / mid) - 1d) * 100;
                    double actualChange = ((outcomes[i][(int)OutcomeMatrixIndices.Actual] / mid) - 1d) * 100;

                    minList.Add(minChange);
                    maxList.Add(maxChange);
                    actualList.Add(actualChange);
                    valuesList.Add(values[i]);
                }
            }

            spearmanMin = Correlation.Spearman(valuesList, minList);
            spearmanMax = Correlation.Spearman(valuesList, maxList);
            spearmanActual = Correlation.Spearman(valuesList, maxList);

            pearsonMin = Correlation.Pearson(valuesList, minList);
            pearsonMax = Correlation.Pearson(valuesList, maxList);
            pearsonActual = Correlation.Pearson(valuesList, actualList);
        }

        public static double[][] sampleValuesOutcomeCode(double[] values, bool[][] outcomeCodes, double min, double max, int steps, out double usedValuesRatio)
        {
            if (values.Length != outcomeCodes.Length)
                throw new Exception("Arrays have to be the same size: " + values.Length + " != " + outcomeCodes.Length);

            double stepsSize = (max - min) / steps;
            usedValuesRatio = 0;

            double[][] output = new double[steps][];
            for (int i = 0; i < output.Length; i++)
                output[i] = new double[] { min + stepsSize * i, 0, 0, 0};

            for(int i = 0; i < values.Length; i++)
            {
                if(values[i] > min && values[i] < max && outcomeCodes[i] != null)
                {
                    int targetIndex = Convert.ToInt32(Math.Floor((values[i] - min) / stepsSize));

                    int buy = outcomeCodes[i][(int)OutcomeCodeMatrixIndices.Buy] ? 1 : 0;
                    int sell = outcomeCodes[i][(int)OutcomeCodeMatrixIndices.Sell] ? 1 : 0;

                    output[targetIndex][(int)SampleValuesOutcomeCodesIndices.BuyRatio] += buy;
                    output[targetIndex][(int)SampleValuesOutcomeCodesIndices.SellRatio] += sell;

                    output[targetIndex][(int)SampleValuesOutcomeCodesIndices.SamplesCount]++;
                    usedValuesRatio++;
                }
            }

            for(int i = 0; i < output.Length; i++)
            {
                double samplesInStep = output[i][(int)SampleValuesOutcomeCodesIndices.SamplesCount];
                if (samplesInStep != 0)
                {
                    output[i][(int)SampleValuesOutcomeCodesIndices.BuyRatio] /= samplesInStep;
                    output[i][(int)SampleValuesOutcomeCodesIndices.SellRatio] /= samplesInStep;

                    checkValue(output[i][(int)SampleValuesOutcomeCodesIndices.BuyRatio]);
                    checkValue(output[i][(int)SampleValuesOutcomeCodesIndices.SellRatio]);
                }
                else
                    output[i] = null;
            }

            usedValuesRatio /= values.Length;

            return output;
        }

        public static double[][] sampleValuesOutcome(double[] values, double[][] prices, double[][] outcomes, double min, double max, out double validValueRatio, int steps = 20)
        {
            if (values.Length != outcomes.Length)
                throw new Exception("Arrays have to be the same size: " + values.Length + " != " + outcomes.Length);

            validValueRatio = 0;
            double stepsSize = (max - min) / steps;

            double[][] output = new double[steps][];
            for (int i = 0; i < output.Length; i++)
                output[i] = new double[] { min + stepsSize * i, 0, 0, 0, 0 };

            for (int i = 0; i < values.Length; i++)
            {
                if (outcomes[i] != null && double.IsNaN(values[i]) == false && values[i] > min && values[i] < max)
                {
                    int targetIndex = Convert.ToInt32(Math.Floor((values[i] - min) / stepsSize));

                    double mid = (prices[i][(int)PriceDataIndeces.Ask] + prices[i][(int)PriceDataIndeces.Bid]) / 2d;
                    double maxChange = ((outcomes[i][(int)OutcomeMatrixIndices.Max] / mid) - 1d) * 100d;
                    double minChange = ((outcomes[i][(int)OutcomeMatrixIndices.Min] / mid) - 1d) * 100d;
                    double actualChange = ((outcomes[i][(int)OutcomeMatrixIndices.Actual] / mid) - 1d) * 100d;

                    output[targetIndex][(int)SampleValuesOutcomeIndices.MaxAvg] += maxChange;
                    output[targetIndex][(int)SampleValuesOutcomeIndices.MinAvg] += minChange;
                    output[targetIndex][(int)SampleValuesOutcomeIndices.ActualAvg] += actualChange;

                    output[targetIndex][(int)SampleValuesOutcomeIndices.SamplesCount]++;
                    validValueRatio++;
                }
            }

            validValueRatio /= outcomes.Length;

            for (int i = 0; i < output.Length; i++)
            {
                double samplesInStep = output[i][(int)SampleValuesOutcomeIndices.SamplesCount];
                if (samplesInStep != 0)
                {
                    output[i][(int)SampleValuesOutcomeIndices.MinAvg] /= samplesInStep;
                    output[i][(int)SampleValuesOutcomeIndices.MaxAvg] /= samplesInStep;
                    output[i][(int)SampleValuesOutcomeIndices.ActualAvg] /= samplesInStep;
                }
                else
                    output[i] = null;
            }

            return output;
        }

        private static void checkValue(double val)
        {
            if (double.IsNaN(val) || double.MaxValue == val || double.MinValue == val || val > 1 || val < -1)
                throw new Exception("Bad value calculated: " + val);
        }
    }
}
