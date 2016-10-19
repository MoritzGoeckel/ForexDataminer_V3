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
        Start = 0, MaxAvg = 1, MinAvg = 2, ActualAvg = 3, SamplesCount = 4
    };

    public static class IndicatorSampler
    {
        public static void getStatistics(double[] values, bool[][] outcomeCodes, out double spearmanBuy, out double spearmanSell, out double pearsonBuy, out double pearsonSell)
        {
            if (values.Length != outcomeCodes.Length)
                throw new Exception("Arrays have to be the same size: " + values.Length + " != " + outcomeCodes.Length);

            double[] buyArray = new double[outcomeCodes.Length];
            double[] sellArray = new double[outcomeCodes.Length];

            for(int i = 0; i < outcomeCodes.Length; i++)
            {
                buyArray[i] = outcomeCodes[i][(int)OutcomeCodeMatrixIndices.Buy] ? 1 : 0;
                sellArray[i] = outcomeCodes[i][(int)OutcomeCodeMatrixIndices.Sell] ? 1 : 0;
            }

            spearmanBuy = Correlation.Spearman(values, buyArray);
            spearmanSell = Correlation.Spearman(values, sellArray);

            pearsonBuy = Correlation.Pearson(values, buyArray);
            pearsonSell = Correlation.Pearson(values, sellArray);
        }

        public static double[][] sampleValuesOutcomeCode(double[] values, bool[][] outcomeCodes, double min, double max, int steps = 20)
        {
            if (values.Length != outcomeCodes.Length)
                throw new Exception("Arrays have to be the same size: " + values.Length + " != " + outcomeCodes.Length);

            double stepsSize = (max - min) / steps;

            double[][] output = new double[steps][];
            for (int i = 0; i < output.Length; i++)
                output[i] = new double[] { min + stepsSize * i, 0, 0, 0};

            for(int i = 0; i < values.Length; i++)
            {
                if(values[i] > min && values[i] < max)
                {
                    int targetIndex = Convert.ToInt32(Math.Floor((values[i] - min) / stepsSize));

                    int buy = outcomeCodes[i][(int)OutcomeCodeMatrixIndices.Buy] ? 1 : 0;
                    int sell = outcomeCodes[i][(int)OutcomeCodeMatrixIndices.Sell] ? 1 : 0;

                    output[targetIndex][(int)SampleValuesOutcomeCodesIndices.BuyRatio] += buy;
                    output[targetIndex][(int)SampleValuesOutcomeCodesIndices.SellRatio] += sell;

                    output[targetIndex][(int)SampleValuesOutcomeCodesIndices.SellRatio] += sell;

                    output[targetIndex][(int)SampleValuesOutcomeCodesIndices.SamplesCount]++;
                }
            }

            for(int i = 0; i < output.Length; i++)
            {
                double samplesInStep = output[i][(int)SampleValuesOutcomeCodesIndices.SamplesCount];
                output[i][(int)SampleValuesOutcomeCodesIndices.BuyRatio] /= samplesInStep;
                output[i][(int)SampleValuesOutcomeCodesIndices.SellRatio] /= samplesInStep;
            }

            return output;
        }

        public static double[][] sampleValuesOutcome(double[] values, double[][] outcomes, double min, double max, out double validValueRatio, int steps = 20)
        {
            if (values.Length != outcomes.Length)
                throw new Exception("Arrays have to be the same size: " + values.Length + " != " + outcomes.Length);

            validValueRatio = 0;
            double stepsSize = (max - min) / steps;

            double[][] output = new double[steps][];
            for (int i = 0; i < output.Length; i++)
                output[i] = new double[] { min + stepsSize * i, 0, 0, 0 };

            for (int i = 0; i < values.Length; i++)
            {
                if (double.IsNaN(values[i]) == false && values[i] > min && values[i] < max)
                {
                    int targetIndex = Convert.ToInt32(Math.Floor((values[i] - min) / stepsSize));

                    output[targetIndex][(int)SampleValuesOutcomeIndices.MaxAvg] += outcomes[i][(int)OutcomeMatrixIndices.Max];
                    output[targetIndex][(int)SampleValuesOutcomeIndices.MinAvg] += outcomes[i][(int)OutcomeMatrixIndices.Min];
                    output[targetIndex][(int)SampleValuesOutcomeIndices.ActualAvg] += outcomes[i][(int)OutcomeMatrixIndices.Actual];

                    output[targetIndex][(int)SampleValuesOutcomeIndices.SamplesCount]++;
                    validValueRatio++;
                }
            }

            validValueRatio /= outcomes.Length;

            for (int i = 0; i < output.Length; i++)
            {
                double samplesInStep = output[i][(int)SampleValuesOutcomeIndices.SamplesCount];
                output[i][(int)SampleValuesOutcomeIndices.MinAvg] /= samplesInStep;
                output[i][(int)SampleValuesOutcomeIndices.MaxAvg] /= samplesInStep;
                output[i][(int)SampleValuesOutcomeIndices.ActualAvg] /= samplesInStep;
            }

            return output;
        }
    }
}
