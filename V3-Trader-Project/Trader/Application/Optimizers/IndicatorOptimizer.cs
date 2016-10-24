using NinjaTrader_Client.Trader.Indicators;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        private double[][] outcomeMatrix = null;
        private IndicatorGenerator generator = new IndicatorGenerator();
        private string resultFolderPath;

        private double desiredOutcomeCodeDistribution;

        private double buyDist, sellDist;

        public Image lastImage;
        public string lastIndicatorName;

        public IndicatorOptimizer(string resultFolderPath, string dataPath, long outcomeTimeframe, int dataDistanceInSeconds, double desiredOutcomeCodeDistribution = double.NaN)
        {
            Logger.log("Loading files ...");
            DataLoader dl = new DataLoader(dataPath);
            data = dl.getArray(1000 * dataDistanceInSeconds);
            Logger.log("End loading files");

            double successRatio;
            outcomeMatrix = OutcomeGenerator.getOutcome(data, outcomeTimeframe, out successRatio);

            if (successRatio < 0.9)
                throw new TooLittleValidDataException("Way too low success rate: " + successRatio);

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
                if (double.IsNaN(desiredOutcomeCodeDistribution))
                {
                    Logger.log("Optimizing outcomecode percentage");
                    outcomeCodePercent = OutcomeCodePercentOptimizer.optimizeOutcomeCodePercentage(200, out outcomeCodePercent, data, outcomeMatrix, out buyDist, out sellDist);
                }
                else
                {
                    Logger.log("Find outcome percent for " + desiredOutcomeCodeDistribution);
                    double desiredDistributionTolerance = desiredOutcomeCodeDistribution / 100d;
                    outcomeCodePercent = OutcomeCodePercentOptimizer.findOutcomeCodeForDesiredDistribution(desiredOutcomeCodeDistribution, desiredDistributionTolerance, data, outcomeMatrix, out buyDist, out sellDist);
                }

                Logger.log("Loading outcome codes: " + outcomeCodePercent);

                double codeSuccessRatio;
                outcomeCodes = OutcomeGenerator.getOutcomeCode(data, outcomeMatrix, outcomeCodePercent, out codeSuccessRatio);
                if (codeSuccessRatio < 0.7)
                    throw new TooLittleValidDataException("The outcome codes deliver to little data " + codeSuccessRatio);

                File.WriteAllText(resultFolderPath + "dist_" + outcomeCodePercent + "_" + outcomeTimeframe + ".txt", outcomeCodePercent + "% at b" + Math.Round(buyDist, 4) + " s" + Math.Round(sellDist, 4));
                Logger.log("SATTLE OPT. dist for " + outcomeCodePercent + "% at b" + Math.Round(buyDist, 4) + " s" + Math.Round(sellDist, 4));
            }

            submitResults(LearningIndicator.getPredictivePowerArrayHeader());

            Logger.log("Start testing indicators");
            new Thread(delegate () {
                running = true;
                while(running)
                {
                    //How about a genetic algo?
                    try {
                        testRandomIndicator(generator.getRandomIndicator(Convert.ToInt32(outcomeTimeframe / 1000 / 15), Convert.ToInt32(outcomeTimeframe * 100 / 1000)));
                    }
                    catch (TooLittleValidDataException e)
                    {
                        Logger.log("E:" + e.Message);
                    }
                    catch (Exception e)
                    {
                        Logger.log("FATAL:" + e.Message);
                    }
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
            Logger.log("Testing Indicator: " + indicator.getName());

            LearningIndicator li = new LearningIndicator(indicator, data, outcomeCodes, outcomeMatrix, outcomeTimeframe, buyDist, sellDist, outcomeCodePercent);
            double[] pp = li.getPredictivePowerArray();

            //Results
            string output = "";
            foreach (double d in pp)
                output += d + ";";

            output += indicator.getName().Split('_')[0] + ";" + indicator.getName();

            lastImage = li.visualizeTables(2000, 1000);
            lastIndicatorName = li.getName();

            Logger.log("Result: " + Math.Round(li.getPredictivePowerScore(), 4) + " " + li.getName());
            submitResults(output);
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
