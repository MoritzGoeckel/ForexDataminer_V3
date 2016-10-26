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
    class TestingEnvironment
    {
        public double[][] priceData = null;

        public double outcomeCodePercent = double.NaN;
        public long outcomeTimeframe;

        public bool[][] outcomeCodes = null;
        public double[][] outcomes = null;

        public double buyDist, sellDist;

        public TestingEnvironment(string resultFolderPath, string dataPath, int dataDistanceInSeconds, long onlyTimeframe = 0)
        {
            Logger.log("Loading files ...");
            DataLoader dl = new DataLoader(dataPath);
            priceData = dl.getArray(1000 * dataDistanceInSeconds, onlyTimeframe);
            Logger.log("End loading files");
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

            Logger.log("SATTLE OPT. dist for " + outcomeCodePercent + "% at b" + Math.Round(buyDist, 4) + " s" + Math.Round(sellDist, 4));
        }

        public LearningIndicator testIndicator(WalkerIndicator indicator)
        {
            return new LearningIndicator(indicator, priceData, outcomeCodes, outcomes, outcomeTimeframe, buyDist, sellDist, outcomeCodePercent);
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
