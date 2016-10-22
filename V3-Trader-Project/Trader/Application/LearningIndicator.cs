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
        private double[] predictivePower;

        //Not known is base distribution of outcomeCodes and timeframe
        //Plug ML in here?
        
        private double[][] outcomeCodeSamplingTable;
        private double[][] outcomeSamplingTable;

        private long timeframe;
        private double meanBuyDist, meanSellDist, targetPercent;

        private WalkerIndicator indicator;
        public LearningIndicator(WalkerIndicator indicator, double[][] prices, bool[][] outcomeCodes, double[][] outcomes, long timeframe, double meanBuyDist, double meanSellDist, double targetPercent)
        {
            this.meanBuyDist = meanBuyDist;
            this.meanSellDist = meanSellDist;
            this.targetPercent = targetPercent;

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

            //Predictive power calculation
            predictivePower = new double[12];
            IndicatorSampler.getStatisticsOutcomeCodes(values, outcomeCodes, out predictivePower[0], out predictivePower[1], out predictivePower[2], out predictivePower[3]);
            IndicatorSampler.getStatisticsOutcomes(values, prices, outcomes, out predictivePower[4], out predictivePower[5], out predictivePower[6], out predictivePower[7], out predictivePower[8], out predictivePower[9]);

            DistributionHelper.getSampleCodesMinMax(outcomeCodeSamplingTable, out predictivePower[10], out predictivePower[11]);
            predictivePower[10] -= meanBuyDist;
            predictivePower[11] -= meanSellDist;
            //End predictive power calculation

            this.indicator = indicator;
        }

        public double[] getPredictivePowerArray()
        {
            return predictivePower;
        }

        public double getPredictivePowerScore()
        {
            double output = 1;
            foreach (double d in predictivePower)
                if (double.IsNaN(d) == false)
                    output += Math.Abs(d);

            return output;
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
