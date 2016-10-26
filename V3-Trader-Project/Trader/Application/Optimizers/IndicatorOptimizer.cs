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
using V3_Trader_Project.Trader.Visualizers;

namespace V3_Trader_Project.Trader.Application
{
    class IndicatorOptimizer
    {
        private double[][] priceData = null;

        private double outcomeCodePercent = double.NaN;
        private long outcomeTimeframe;

        private bool[][] outcomeCodes = null;
        private double[][] outcomes = null;
        private string resultFolderPath;

        private double buyDist, sellDist;

        public double[][] getPriceData()
        {
            return priceData;
        }

        public IndicatorOptimizer(string resultFolderPath, string dataPath, int dataDistanceInSeconds, long onlyTimeframe = 0)
        {
            Logger.log("Loading files ...");
            DataLoader dl = new DataLoader(dataPath);
            priceData = dl.getArray(1000 * dataDistanceInSeconds, onlyTimeframe);
            Logger.log("End loading files");
                        
            this.resultFolderPath = resultFolderPath;
        }

        public void loadOutcomeCodes(long outcomeTimeframe, double desiredOutcomeCodeDistribution = double.NaN)
        {
            this.outcomeTimeframe = outcomeTimeframe;

            double successRatio;
            outcomes = OutcomeGenerator.getOutcome(priceData, outcomeTimeframe, out successRatio);

            if (successRatio < 0.9)
                throw new TooLittleValidDataException("Way too low success rate: " + successRatio);

            if (double.IsNaN(desiredOutcomeCodeDistribution))
            {
                Logger.log("Optimizing outcomecode percentage");
                outcomeCodePercent = OutcomeCodePercentOptimizer.optimizeOutcomeCodePercentage(200, out outcomeCodePercent, priceData, outcomes, out buyDist, out sellDist);
            }
            else
            {
                Logger.log("Find outcome percent for " + desiredOutcomeCodeDistribution);
                double desiredDistributionTolerance = desiredOutcomeCodeDistribution / 100d;
                outcomeCodePercent = OutcomeCodePercentOptimizer.findOutcomeCodeForDesiredDistribution(desiredOutcomeCodeDistribution, desiredDistributionTolerance, priceData, outcomes, out buyDist, out sellDist);
            }

            Logger.log("Loading outcome codes: " + outcomeCodePercent);

            double codeSuccessRatio;
            outcomeCodes = OutcomeGenerator.getOutcomeCode(priceData, outcomes, outcomeCodePercent, out codeSuccessRatio);
            if (codeSuccessRatio < 0.7)
                throw new TooLittleValidDataException("The outcome codes deliver to little data " + codeSuccessRatio);

            File.WriteAllText(resultFolderPath + "dist_" + outcomeCodePercent + "_" + outcomeTimeframe + ".txt", outcomeCodePercent + "% at b" + Math.Round(buyDist, 4) + " s" + Math.Round(sellDist, 4));
            Logger.log("SATTLE OPT. dist for " + outcomeCodePercent + "% at b" + Math.Round(buyDist, 4) + " s" + Math.Round(sellDist, 4));
        }

        private bool running = false;
        public void startRunningRandomIndicators(IndicatorGenerator generator)
        {
            if (running == true)
                throw new Exception("Already running!");

            if (outcomes == null || outcomeCodes == null)
                throw new Exception("Set outcomes and outcomeCodes first");      

            submitResults(LearningIndicator.getPredictivePowerArrayHeader());

            Logger.log("Start testing indicators");
            new Thread(delegate () {
                running = true;
                while(running)
                {
                    //How about a genetic algo?
                    try {
                        testIndicator(generator.getRandomIndicator(Convert.ToInt32(outcomeTimeframe / 1000 / 15), Convert.ToInt32(outcomeTimeframe * 100 / 1000)));
                    }
                    catch (TooLittleValidDataException e)
                    {
                        Logger.log("E:" + e.Message);
                    }
                    catch (Exception e)
                    {
                        Logger.log("FATAL:" + e.Message);
                    }
                }
            }).Start();
        }

        public void stop()
        {
            running = false;
        }

        public LearningIndicator testIndicator(WalkerIndicator indicator)
        {
            Logger.log("Testing Indicator: " + indicator.getName());

            LearningIndicator li = new LearningIndicator(indicator, priceData, outcomeCodes, outcomes, outcomeTimeframe, buyDist, sellDist, outcomeCodePercent);
            double[] pp = li.getPredictivePowerArray();

            //Results
            string output = "";
            foreach (double d in pp)
                output += d + ";";

            output += indicator.getName().Split('_')[0] + ";" + indicator.getName();

            Logger.log("Result: " + Math.Round(li.getPredictivePowerScore(), 4) + " " + li.getName());
            submitResults(output);

            return li;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void submitResults(string results)
        {
            string fileName = "outcomeIndicators_" + outcomeCodePercent + "_" + outcomeTimeframe + ".csv";
            File.AppendAllText(resultFolderPath + fileName, results + Environment.NewLine);
        }
        
        public Image visualizePriceAndOutcomeCodes(int width, int height)
        {
            Image priceImage = ArrayVisualizer.visualizePriceData(priceData, width, height / 2, 20);
            Image outcomeCodesImage = ArrayVisualizer.visualizeOutcomeCodeArray(outcomeCodes, width, height / 2);

            Image o = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(o);
            g.Clear(Color.White);
            g.DrawImage(priceImage, 0, 0);
            g.DrawImage(outcomeCodesImage, 0, priceImage.Height);
            g.DrawLine(new Pen(Color.Blue, 3), 0, priceImage.Height, priceImage.Width, priceImage.Height);

            return o;
        }
    }
}
