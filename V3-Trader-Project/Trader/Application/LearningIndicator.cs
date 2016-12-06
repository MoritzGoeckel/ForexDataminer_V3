using MathNet.Numerics.Statistics;
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
    public enum LearningIndicatorPredictionIndecies{
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

        private double usedValues;

        private WalkerIndicator indicator;

        public LearningIndicator(WalkerIndicator indicator, double[][] prices, bool[][] outcomeCodes, double[][] outcomes, long timeframe, double targetPercent, double minPercentThreshold, int steps, bool createStatistics)
        {
            this.targetPercent = targetPercent;
            this.timeframe = timeframe;

            double validRatio;
            double[] values = IndicatorRunner.getIndicatorValues(prices, indicator.Clone(), out validRatio);
            if (validRatio < 0.5)
                throw new TooLittleValidDataException("Not enough valid values: " + validRatio);

            //May be does not work properly... todo:
            double min, max, usedValuesRatio;
            //DistributionHelper.getMinMax(values, 4, out min, out max);
            DistributionHelper.getMinMax(values, out min, out max);

            outcomeCodeSamplingTable = IndicatorSampler.sampleValuesOutcomeCode(values, outcomeCodes, min, max, steps, out usedValuesRatio);
            if (usedValuesRatio < 0.5)
                throw new TooLittleValidDataException("Not enough sampling for outcomeCode: " + usedValuesRatio);

            outcomeSamplingTable = IndicatorSampler.sampleValuesOutcome(values, prices, outcomes, min, max, out usedValuesRatio, 40);
            if (usedValuesRatio < 0.5)
                throw new TooLittleValidDataException("Not enough sampling for outcome: " + usedValuesRatio);

            this.usedValues = usedValuesRatio;

            if (createStatistics)
            {
                //Predictive power calculation
                predictivePower = new double[29];
                IndicatorSampler.getStatisticsOutcomeCodes(values, outcomeCodes, out predictivePower[0], out predictivePower[1], out predictivePower[2], out predictivePower[3]);
                IndicatorSampler.getStatisticsOutcomes(values, prices, outcomes, out predictivePower[4], out predictivePower[5], out predictivePower[6], out predictivePower[7], out predictivePower[8], out predictivePower[9]);

                DistributionHelper.getSampleOutcomeCodesBuyMaxSellMax(outcomeCodeSamplingTable, minPercentThreshold, out predictivePower[10], out predictivePower[11], out predictivePower[12], out predictivePower[13]);
                DistributionHelper.getSampleOutcomesMinMax(outcomeSamplingTable, minPercentThreshold, out predictivePower[14], out predictivePower[15], out predictivePower[16], out predictivePower[17], out predictivePower[18], out predictivePower[19], out predictivePower[20], out predictivePower[21], out predictivePower[22], out predictivePower[23]);

                //Outcome Code

                List<double> buyCodesDist = new List<double>(), 
                    sellCodesDist = new List<double>(),
                    buySellDistanceDist = new List<double>(),
                    minMaxDistanceDist = new List<double>(),
                    minDist = new List<double>(),
                    maxDist = new List<double>(),
                    actualDist = new List<double>();

                double totalCodeSamples = 0;
                foreach (double[] row in outcomeCodeSamplingTable)
                {
                    totalCodeSamples += row[(int)SampleValuesOutcomeCodesIndices.SamplesCount];
                }

                foreach (double[] row in outcomeCodeSamplingTable)
                {
                    if ((row[(int)SampleValuesOutcomeCodesIndices.SamplesCount] / totalCodeSamples) * 100 >= minPercentThreshold) //minPercentThreshold
                    {
                        buyCodesDist.Add(row[(int)SampleValuesOutcomeCodesIndices.BuyRatio]);
                        sellCodesDist.Add(row[(int)SampleValuesOutcomeCodesIndices.SellRatio]);
                        buySellDistanceDist.Add(Math.Abs(row[(int)SampleValuesOutcomeCodesIndices.BuyRatio] - row[(int)SampleValuesOutcomeCodesIndices.SellRatio]));
                    }
                }

                predictivePower[(int)LearningIndicatorPredictivePowerIndecies.buyCodeStD] = buyCodesDist.StandardDeviation();
                predictivePower[(int)LearningIndicatorPredictivePowerIndecies.sellCodeStD] = sellCodesDist.StandardDeviation();
                predictivePower[(int)LearningIndicatorPredictivePowerIndecies.buySellCodeDistanceStD] = buySellDistanceDist.StandardDeviation();

                //Outcome

                double totalSamples = 0;
                foreach (double[] row in outcomeSamplingTable)
                {
                    totalSamples += row[(int)SampleValuesOutcomeIndices.SamplesCount];
                }

                //Avgs
                foreach (double[] row in outcomeSamplingTable)
                {
                    if ((row[(int)SampleValuesOutcomeIndices.SamplesCount] / totalSamples) * 100 > minPercentThreshold) //minPercentThreshold
                    {
                        maxDist.Add(row[(int)SampleValuesOutcomeIndices.MaxAvg]);
                        minDist.Add(row[(int)SampleValuesOutcomeIndices.MinAvg]);
                        minMaxDistanceDist.Add(Math.Abs(row[(int)SampleValuesOutcomeIndices.MaxAvg]) + row[(int)SampleValuesOutcomeIndices.MinAvg]);
                        actualDist.Add(row[(int)SampleValuesOutcomeIndices.ActualAvg]);
                    }
                }

                //avg distances
                predictivePower[(int)LearningIndicatorPredictivePowerIndecies.maxStD] = maxDist.StandardDeviation();
                predictivePower[(int)LearningIndicatorPredictivePowerIndecies.minStD] = minDist.StandardDeviation();
                predictivePower[(int)LearningIndicatorPredictivePowerIndecies.minMaxDistanceStd] = minMaxDistanceDist.StandardDeviation();
                predictivePower[(int)LearningIndicatorPredictivePowerIndecies.actualStD] = actualDist.StandardDeviation();

                //End predictive power calculation
            }

            this.indicator = indicator;
        }
        
        public string getName()
        {
            return indicator.getName();
        }

        public string getAlgoName()
        {
            if (indicator.getName().Contains("_"))
                return indicator.getName().Split('_')[0];
            else
                return indicator.getName();
        }

        public long getTimeframe()
        {
            return timeframe;
        }

        public double getUsedValues()
        {
            return usedValues;
        }

        public double getPercent()
        {
            return targetPercent;
        }

        public double[] getPredictivePowerArray()
        {
            return predictivePower;
        }

        public enum LearningIndicatorPredictivePowerIndecies
        {
            spBuy = 0, spSell = 1, pBuy = 2, pSell = 3,
            spMin = 4, spMax = 5, spActual = 6,
            pMin = 7, pMax = 8, pActual = 9,
            maxBuyCode = 10, maxBuyCodeCount = 11, maxSellCode = 12,
            maxSellCodeCount = 13, maxMaxGainPercent = 14, maxMaxGainPercentCount = 15,
            minMinFallPercent = 16, minMinFallPercentCount = 17, maxMinMaxDistancePercent = 18,
            maxMinMaxDistancePercentCount = 19, maxActualGainPercent = 20, maxActualGainPercentCount = 21,
            minActualFallPercent = 22, minActualFallPercentCount = 23, actualStD = 24,
            minStD = 25, maxStD = 26, minMaxDistanceStd = 27,
            buySellCodeDistanceStD = 28, buyCodeStD = 29, sellCodeStD = 30
        };

        public static string[] getPredictivePowerArrayColumnNames()
        {
            string[] header = new string[29];
            header[0] = "spBuy"; header[1] = "spSell"; header[2] = "pBuy"; header[3] = "pSell";
            header[4] = "spMin"; header[5] = "spMax"; header[6] = "spActual";
            header[7] = "pMin"; header[8] = "pMax"; header[9] = "pActual";
            header[10] = "maxBuyCode"; header[11] = "maxBuyCodeCount"; header[12] = "maxSellCode";
            header[13] = "maxSellCodeCount"; header[14] = "maxMaxGainPercent"; header[15] = "maxMaxGainPercentCount";
            header[16] = "minMinFallPercent"; header[17] = "minMinFallPercentCount"; header[18] = "maxMinMaxDistancePercent";
            header[19] = "maxMinMaxDistancePercentCount"; header[20] = "maxActualGainPercent"; header[21] = "maxActualGainPercentCount";
            header[22] = "minActualFallPercent"; header[23] = "minActualFallPercentCount"; header[24] = "actualStD";
            header[25] = "minStD"; header[26] = "maxStD"; header[27] = "minMaxDistanceStd";
            header[28] = "buySellCodeDistanceStD"; header[29] = "buyCodeStD"; header[30] = "sellCodeStD";

            return header;
        }

        public static string getPredictivePowerArrayHeader()
        {
            StringBuilder s = new StringBuilder();
            foreach (string column in getPredictivePowerArrayColumnNames())
                s.Append(column + ";");

            return s.ToString();
        }
                
        public void setNewPrice(double[] prices)
        {
            double mid = (prices[(int)PriceDataIndeces.Ask] + prices[(int)PriceDataIndeces.Bid]) / 2d;
            indicator.setNextData(Convert.ToInt64(prices[(int)PriceDataIndeces.Date]), mid);
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
                for (int i = 0; i < outcomeCodeSamplingTable.Length; i++)
                {
                    bool lastElement = i == outcomeCodeSamplingTable.Length - 1;
                    if (lastElement
                        || (outcomeCodeSamplingTable[i + 1][(int)SampleValuesOutcomeCodesIndices.Start] > v && outcomeCodeSamplingTable[i][(int)SampleValuesOutcomeCodesIndices.Start] <= v))
                    {
                        buyRatio = outcomeCodeSamplingTable[i][(int)SampleValuesOutcomeCodesIndices.BuyRatio];
                        sellRatio = outcomeCodeSamplingTable[i][(int)SampleValuesOutcomeCodesIndices.SellRatio];
                        break;
                    }
                }

                //Search in outcomeSamplingTable
                for (int i = 0; i < outcomeSamplingTable.Length; i++)
                {
                    bool lastElement = i == outcomeSamplingTable.Length - 1;
                    if (lastElement 
                        || (outcomeSamplingTable[i + 1][(int)SampleValuesOutcomeIndices.Start] > v && outcomeSamplingTable[i][(int)SampleValuesOutcomeIndices.Start] <= v))
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

        public Image visualizeIndicatorValues(int widht, int height, double[][] prices)
        {
            double validRatio;
            double[] values = IndicatorRunner.getIndicatorValues(prices, indicator.Clone(), out validRatio);
            return ArrayVisualizer.visualizeArray(values, widht, height, 15);
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
            g.DrawLine(new Pen(Color.Blue, 3), 0, outcomeImg.Height, outcomeImg.Width, outcomeImg.Height);

            return o;
        }
    }
}
