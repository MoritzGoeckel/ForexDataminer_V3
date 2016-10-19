using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader
{
    public enum SampleValuesOutcomeIndices{
        Start = 0, BuyRatio = 1, SellRatio = 2, SamplesCount = 3
    };

    public class IndicatorSampler
    {
        public void getStatistics(double[] values, bool[][] outcomeCodes, out double spearmanBuy, out double spearmanSell, out double pearsonBuy, out double pearsonSell)
        {
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

        public double[][] sampleValuesOutcomeCode(double[] values, bool[][] outcomeCodes, double min, double max, int steps = 20)
        {
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

                    output[targetIndex][(int)SampleValuesOutcomeIndices.BuyRatio] += buy;
                    output[targetIndex][(int)SampleValuesOutcomeIndices.SellRatio] += sell;

                    output[targetIndex][(int)SampleValuesOutcomeIndices.SellRatio] += sell;

                    output[targetIndex][(int)SampleValuesOutcomeIndices.SamplesCount]++;
                }
            }

            for(int i = 0; i < output.Length; i++)
            {
                double samplesInStep = output[i][(int)SampleValuesOutcomeIndices.SamplesCount];
                output[i][(int)SampleValuesOutcomeIndices.BuyRatio] /= samplesInStep;
                output[i][(int)SampleValuesOutcomeIndices.SellRatio] /= samplesInStep;
            }

            return output;
        }
    }
}
