using NinjaTrader_Client.Trader.Indicators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Application
{
    class IndicatorOptimizer
    {
        private double[][] data = null;

        private double outcomeCodePercent = double.NaN;
        private long outcomeTimeframe;

        private bool[][] outcomeCodes = null;
        private IndicatorGenerator generator = new IndicatorGenerator();
        private string resultFolderPath;

        private double desiredOutcomeCodeDistribution;

        private double buyDist, sellDist;

        public IndicatorOptimizer(string resultFolderPath, string dataPath, long outcomeTimeframe, int dataDistanceInSeconds, double desiredOutcomeCodeDistribution = double.NaN)
        {
            Logger.log("Loading files ...");
            DataLoader dl = new DataLoader(dataPath);
            data = dl.getArray(1000 * dataDistanceInSeconds);
            Logger.log("End loading files");

            this.desiredOutcomeCodeDistribution = desiredOutcomeCodeDistribution;
            this.outcomeTimeframe = outcomeTimeframe;
            this.resultFolderPath = resultFolderPath;
        }

        private bool running = false;
        public void start()
        {
            if (running == true)
                throw new Exception("Already running!");

            if (outcomeCodes == null || double.IsNaN(outcomeCodePercent))
            {
                double successRatio;
                double[][] outcomeMatrix = OutcomeGenerator.getOutcome(data, outcomeTimeframe, out successRatio);

                if (successRatio < 0.9)
                    throw new Exception("Way too low success rate: " + successRatio);

                if (double.IsNaN(desiredOutcomeCodeDistribution))
                {
                    Logger.log("Optimizing outcomecode percentage");
                    OutcomeCodePercentOptimizer.optimizeOutcomeCodePercentage(300, out outcomeCodePercent, data, outcomeMatrix, out buyDist, out sellDist);
                }
                else
                {
                    Logger.log("Find outcome percent for " + desiredOutcomeCodeDistribution);
                    double desiredDistributionTolerance = desiredOutcomeCodeDistribution / 100d;
                    OutcomeCodePercentOptimizer.findOutcomeCodeForDesiredDistribution(desiredOutcomeCodeDistribution, desiredDistributionTolerance, data, outcomeMatrix, out buyDist, out sellDist);
                }

                File.WriteAllText(resultFolderPath + "dist_" + outcomeCodePercent + "_" + outcomeTimeframe + ".txt", outcomeCodePercent + "% at b" + Math.Round(buyDist, 4) + " s" + Math.Round(sellDist, 4));
                Logger.log("SATTLE OPT. dist for " + outcomeCodePercent + "% at b" + Math.Round(buyDist, 4) + " s" + Math.Round(sellDist, 4));
            }

            Logger.log("Start testing indicators");
            new Thread(delegate () {
                running = true;
                while(running)
                {
                    //How about a genetic algo?
                    try { testRandomIndicator(generator.getRandomIndicator()); } catch { }
                    //testRandomIndicator();
                }
            }).Start();
        }

        public void stop()
        {
            running = false;
        }

        private void testRandomIndicator(WalkerIndicator indicator)
        {
            //Todo: why not just use the new learning indicator? :)
            //Create the random Indicator and retrive values
            Logger.log("Testing Indicator: " + indicator.getName());

            double validR;
            double[] values = IndicatorRunner.getIndicatorValues(data, indicator, out validR);

            if (validR < 0.7)
                throw new Exception("Indicator not valid: " + validR);

            //Get min / max -10% of indicator
            double min, max;
            DistributionHelper.getMinMax(values, 5, out min, out max);

            //Sample the indicator with min max
            double usedValuesRatio;
            double[][] samplesMatrix = IndicatorSampler.sampleValuesOutcomeCode(values, outcomeCodes, min, max, 20, out usedValuesRatio);

            if (usedValuesRatio < 0.65) //Todo: Why does that happen so often when the indicator is fine?
                throw new Exception("Invalid sampling: " + usedValuesRatio + " min" + min + " max" + max + " " + indicator.getName());

            //Retrive the max for buy and sell
            double maxBuy, maxSell;
            DistributionHelper.getSampleCodesMinMax(samplesMatrix, out maxBuy, out maxSell);

            //Retrive the correalations
            double spBuy, spSell, pBuy, pSell;
            IndicatorSampler.getStatisticsOutcomeCodes(values, outcomeCodes, out spBuy, out spSell, out pBuy, out pSell);

            //Submit the results
            Logger.log("Result: " + Math.Round(spBuy, 4) + " " + Math.Round(spSell, 4) + " " + Math.Round(pBuy, 4) + " " + Math.Round(pSell, 4) + " " + Math.Round(maxBuy, 4) + " " + Math.Round(maxSell, 4) + " v" + Math.Round(validR, 1));
            submitResults(spBuy + ";" + spSell + ";" + pBuy + ";" + pSell + ";" + maxBuy + ";" + maxSell + ";" + validR + ";" + indicator.getName().Split('_')[0] + ";" + indicator.getName());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void submitResults(string results)
        {
            string fileName = "outcomeIndicators_" + outcomeCodePercent + "_" + outcomeTimeframe + ".csv";
            File.AppendAllText(resultFolderPath + fileName, results + Environment.NewLine);
        }

        public static IndicatorOptimizer load(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (IndicatorOptimizer)binaryFormatter.Deserialize(stream);
            }
        }

        public void save(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, this);
            }
        }
    }
}
