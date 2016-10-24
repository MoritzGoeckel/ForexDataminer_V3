using NinjaTrader_Client.Trader.Indicators;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V3_Trader_Project.Trader.Visualizers;

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
        
        //Not used
        private long timeframe;
        private double targetPercent;
        private double meanBuyDist, meanSellDist;

        private WalkerIndicator indicator;
        public LearningIndicator(WalkerIndicator indicator, double[][] prices, bool[][] outcomeCodes, double[][] outcomes, long timeframe, double meanBuyDist, double meanSellDist, double targetPercent)
        {
            this.meanBuyDist = meanBuyDist;
            this.meanSellDist = meanSellDist;
            this.targetPercent = targetPercent;
            this.timeframe = timeframe;

            double validRatio;
            double[] values = IndicatorRunner.getIndicatorValues(prices, indicator.Clone(), out validRatio);
            if (validRatio < 0.7)
                throw new TooLittleValidDataException("Not enough valid values: " + validRatio);

            double min, max, usedValuesRatio;
            DistributionHelper.getMinMax(values, 10, out min, out max);

            outcomeCodeSamplingTable = IndicatorSampler.sampleValuesOutcomeCode(values, outcomeCodes, min, max, 20, out usedValuesRatio);
            if (usedValuesRatio < 0.7)
                throw new TooLittleValidDataException("Not enough sampling for outcomeCode: " + usedValuesRatio);

            outcomeSamplingTable = IndicatorSampler.sampleValuesOutcome(values, prices, outcomes, min, max, out usedValuesRatio, 20);
            if (usedValuesRatio < 0.7)
                throw new TooLittleValidDataException("Not enough sampling for outcome: " + usedValuesRatio);

            //Predictive power calculation
            predictivePower = new double[16];
            IndicatorSampler.getStatisticsOutcomeCodes(values, outcomeCodes, out predictivePower[0], out predictivePower[1], out predictivePower[2], out predictivePower[3]);
            IndicatorSampler.getStatisticsOutcomes(values, prices, outcomes, out predictivePower[4], out predictivePower[5], out predictivePower[6], out predictivePower[7], out predictivePower[8], out predictivePower[9]);

            DistributionHelper.getSampleOutcomeCodesBuyMaxSellMax(outcomeCodeSamplingTable, out predictivePower[10], out predictivePower[11]);
            DistributionHelper.getSampleOutcomesMinMax(outcomeSamplingTable, out predictivePower[12], out predictivePower[13], out predictivePower[14], out predictivePower[15]);
            //End predictive power calculation

            this.indicator = indicator;
        }

        public string getName()
        {
            return indicator.getName();
        }

        public long getTimeframe()
        {
            return timeframe;
        }

        public double getPercent()
        {
            return targetPercent;
        }

        public double[] getPredictivePowerArray()
        {
            return predictivePower;
        }

        public static string getPredictivePowerArrayHeader()
        {
            return "spBuy;spSell;pBuy;pSell;spMin;spMax;spActual;pMin;pMax;pActual;maxBuyCode;maxSellCode;maxMaxGain%;minMinFall%;maxActualGain%;minActualFall%;";
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

        public Image visualizeTables(int width, int height, bool showState = false)
        {
            double currentIndicatorValue = indicator.getIndicator();
            Image outcomeImg = OutcomeSamplingVisualizer.visualizeOutcomeSamplingTable(outcomeSamplingTable, width, height / 2, showState ? currentIndicatorValue : double.NaN);
            Image outcomeCodeImg = OutcomeSamplingVisualizer.visualizeOutcomeCodeSamplingTable(outcomeCodeSamplingTable, width, height / 2, showState ? currentIndicatorValue : double.NaN);

            Image o = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(o);
            g.Clear(Color.White);
            g.DrawImage(outcomeImg, 0, 0);
            g.DrawImage(outcomeCodeImg, 0, outcomeImg.Height);

            return o;
        }
    }
}
