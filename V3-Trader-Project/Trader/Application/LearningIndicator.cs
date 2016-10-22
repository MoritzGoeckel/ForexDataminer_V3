using NinjaTrader_Client.Trader.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Application
{
    public enum LearningIndicatorResult{
        BuyCodeProbability = 0, SellCodeProbability = 1, AvgOutcomeMax = 2, AvgOutcomeMin = 3, AvgOutcomeActual = 4
    };

    public class LearningIndicator
    {
        //Not known is base distribution of outcomeCodes and timeframe
        
        //Plug ML in here?
        //Calculate indicator PP and return as function?

        private double[][] outcomeCodeSamplingTable;
        private double[][] outcomeSamplingTable;

        private WalkerIndicator indicator;
        public LearningIndicator(WalkerIndicator indicator, double[][] prices, bool[][] outcomeCodes, double[][] outcomes)
        {
            double validRatio;
            double[] values = IndicatorRunner.getIndicatorValues(prices, indicator.Clone(), out validRatio);
            if (validRatio < 0.7)
                throw new Exception("Not enough valid values: " + validRatio);

            double min, max, usedValuesRatio;
            DistributionHelper.getMinMax(values, 10, out min, out max);

            outcomeCodeSamplingTable = IndicatorSampler.sampleValuesOutcomeCode(values, outcomeCodes, min, max, 20, out usedValuesRatio);
            if (usedValuesRatio < 0.7)
                throw new Exception("Not enough sampling for outcomeCode: " + usedValuesRatio);

            outcomeSamplingTable = IndicatorSampler.sampleValuesOutcome(values, prices, outcomes, min, max, out usedValuesRatio, 20);
            if (usedValuesRatio < 0.7)
                throw new Exception("Not enough sampling for outcome: " + usedValuesRatio);

            //IndicatorSampler.getStatisticsOutcomeCodes ... todo

            this.indicator = indicator;
        }

        public void setNewPrice(double[] prices)
        {
            double mid = (prices[(int)PriceDataIndeces.Ask] + prices[(int)PriceDataIndeces.Bid]) / 2d;
            indicator.setNextData(Convert.ToInt32(prices[(int)PriceDataIndeces.Date]), mid);
        }

        public double[] getPrediction(long timestamp)
        {
            if (indicator.isValid(timestamp) == false)
                return new double[] { double.NaN, double.NaN, double.NaN, double.NaN, double.NaN };
            else
            {
                double buyRatio = double.NaN, sellRatio = double.NaN, max = double.NaN, min = double.NaN, actual = double.NaN;
                //Search in outcomeCodeSamplingTable
                double v = indicator.getIndicator();
                for(int i = 0; i < outcomeCodeSamplingTable.Length; i++)
                {
                    if (v >= outcomeCodeSamplingTable[i][(int)SampleValuesOutcomeCodesIndices.Start] && (i == outcomeCodeSamplingTable.Length - 1 || outcomeCodeSamplingTable[i + 1][(int)SampleValuesOutcomeCodesIndices.Start] > v))
                    {
                        buyRatio = outcomeCodeSamplingTable[i][(int)SampleValuesOutcomeCodesIndices.BuyRatio];
                        sellRatio = outcomeCodeSamplingTable[i][(int)SampleValuesOutcomeCodesIndices.SellRatio];
                        break;
                    }
                }

                //Search in outcomeSamplingTable
                for (int i = 0; i < outcomeSamplingTable.Length; i++)
                {
                    if (v >= outcomeSamplingTable[i][(int)SampleValuesOutcomeIndices.Start] && (i == outcomeSamplingTable.Length - 1 || outcomeSamplingTable[i + 1][(int)SampleValuesOutcomeIndices.Start] > v))
                    {
                        min = outcomeSamplingTable[i][(int)SampleValuesOutcomeIndices.MinAvg];
                        max = outcomeSamplingTable[i][(int)SampleValuesOutcomeIndices.MaxAvg];
                        actual = outcomeSamplingTable[i][(int)SampleValuesOutcomeIndices.ActualAvg];
                        break;
                    }
                }

                return new double[]{ buyRatio, sellRatio, min, max, actual };
            }
        }
    }
}
