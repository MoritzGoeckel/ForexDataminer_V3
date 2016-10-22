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

            if(double.IsNaN(desiredOutcomeCodeDistribution) == false && (outcomeCodes == null || double.IsNaN(outcomeCodePercent)))
                findOutcomeCodeForDesiredDistribution(desiredOutcomeCodeDistribution);

            if (double.IsNaN(desiredOutcomeCodeDistribution) && (outcomeCodes == null || double.IsNaN(outcomeCodePercent)))
                optimizeOutcomeCodePercentage(300);

            Logger.log("Start testing indicators");
            new Thread(delegate () {
                running = true;
                while(running)
                {
                    try { testRandomIndicator(); } catch { }
                    //testRandomIndicator();
                }
            }).Start();
        }

        public void stop()
        {
            running = false;
        }

        private void findOutcomeCodeForDesiredDistribution(double desiredDistribution)
        {
            Logger.log("Find outcome percent for " + desiredDistribution);
            double successRatio;
            double[][] outcomeMatrix = OutcomeGenerator.getOutcome(data, outcomeTimeframe, out successRatio);
            outcomeCodePercent = 0.5;

            double desiredDistributionTolerance = desiredDistribution / 100d;

            if (successRatio < 0.9)
                throw new Exception("Way too low success rate: " + successRatio);

            double buyDist, sellDist;
            
            int round = 0;
            while(true)
            {
                double successRatioCode;
                outcomeCodes = OutcomeGenerator.getOutcomeCode(data, outcomeMatrix, outcomeCodePercent, out successRatioCode);

                if (successRatioCode < 0.9)
                    throw new Exception("Too few outcome codes: " + successRatioCode);

                DistributionHelper.getOutcomeCodeDistribution(outcomeCodes, out buyDist, out sellDist);

                double score = (buyDist + sellDist) / 2;
                if (score > desiredDistribution - desiredDistributionTolerance && score < desiredDistribution + desiredDistributionTolerance)
                    break;
                else if(score > desiredDistribution + desiredDistributionTolerance)
                    outcomeCodePercent += (outcomeCodePercent / (10 + round));
                else if(score < desiredDistribution - desiredDistributionTolerance)
                    outcomeCodePercent -= (outcomeCodePercent / (10 + round));

                Logger.log("SetDist OPT. Round " + round + " -> " + outcomeCodePercent + "% = b" + Math.Round(buyDist, 4) + " s" + Math.Round(sellDist, 4) + " =" + Math.Round(score, 4));

                round++;
            }

            File.WriteAllText(resultFolderPath + "dist_" + outcomeCodePercent + "_" + outcomeTimeframe + ".txt", outcomeCodePercent + "% at b" + Math.Round(buyDist, 4) + " s" + Math.Round(sellDist, 4));
            Logger.log("SATTLE OPT. dist for "+ outcomeCodePercent + "% at b" + Math.Round(buyDist, 4) + " s" + Math.Round(sellDist, 4) + " after " + round + " rounds");
        }

        private void optimizeOutcomeCodePercentage(int rounds)
        {
            Logger.log("Optimizing outcomecode percentage");
            double successRatio;
            double[][] outcomeMatrix = OutcomeGenerator.getOutcome(data, outcomeTimeframe, out successRatio);
            outcomeCodePercent = 0.5;

            if (successRatio < 0.9)
                throw new Exception("Way too low success rate: " + successRatio);

            double buyDist = double.NaN, sellDist = double.NaN;
            double lastScore = double.MinValue;
            double direction = -0.01;

            int round;
            for (round = 1;  round < rounds; round++)
            {
                double successRatioCode;
                outcomeCodes = OutcomeGenerator.getOutcomeCode(data, outcomeMatrix, outcomeCodePercent, out successRatioCode);

                if (successRatioCode < 0.9)
                    throw new Exception("Too low success ratio: " + successRatioCode);

                DistributionHelper.getOutcomeCodeDistribution(outcomeCodes, out buyDist, out sellDist);

                double score = ((buyDist + sellDist) / 2) * outcomeCodePercent;
                if (score < lastScore) //Wrong direction
                {
                    direction = direction * (-1);
                    Logger.log("New opt. direction: " + direction);
                }

                if(outcomeCodePercent <= 0 && direction <= 0)
                    direction = Math.Abs(direction);

                outcomeCodePercent += (direction / (1 + (round / 20)));

                Logger.log("PercDist OPT. Round " + round + " -> " + outcomeCodePercent + "% = |s"+ Math.Round(score, 4) + "| b" + Math.Round(buyDist, 4) + " s" + Math.Round(sellDist, 4));

                lastScore = score;
                round++;
            }

            File.WriteAllText(resultFolderPath + "dist_" + outcomeCodePercent + "_" + outcomeTimeframe + ".txt", outcomeCodePercent + "% at b" + Math.Round(buyDist, 4) + " s" + Math.Round(sellDist, 4));
            Logger.log("SATTLE OPT. for " + outcomeCodePercent + "% at b" + Math.Round(buyDist, 4) + " s" + Math.Round(sellDist, 4) + " after " + round + " rounds");
        }
        
        private void testRandomIndicator()
        {
            //Create the random Indicator and retrive values
            WalkerIndicator indicator = generator.getRandomIndicator();

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
            IndicatorSampler.getStatistics(values, outcomeCodes, out spBuy, out spSell, out pBuy, out pSell);

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
