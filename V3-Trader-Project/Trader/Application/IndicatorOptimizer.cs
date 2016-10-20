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

        public string resultFilePath;

        public IndicatorOptimizer(string resultFilePath, string dataPath, long outcomeTimeframe)
        {
            DataLoader dl = new DataLoader(dataPath);
            data = dl.getArray();

            this.outcomeTimeframe = outcomeTimeframe;
            this.resultFilePath = resultFilePath;

            //Reduce data?
        }

        private bool running = false;
        public void start()
        {
            if (running == true)
                throw new Exception("Already running!");

            if(outcomeCodes == null || double.IsNaN(outcomeCodePercent))
                findOutcomeCode(0.5);

            new Thread(delegate () {
                running = true;
                while(running)
                {
                    testRandomIndicator();
                }
            }).Start();
        }

        public void stop()
        {
            running = false;
        }

        private void findOutcomeCode(double desiredDistribution)
        {
            double[][] outcomeMatrix = OutcomeGenerator.getOutcome(data, outcomeTimeframe);
            outcomeCodePercent = 0.5;

            double buyDist, sellDist;
            
            int round = 0;
            while(true)
            {
                outcomeCodes = OutcomeGenerator.getOutcomeCode(data, outcomeMatrix, outcomeCodePercent);
                DistributionHelper.getOutcomeCodeDistribution(outcomeCodes, out buyDist, out sellDist);

                double score = (buyDist + sellDist) / 2;
                if (score > 0.4 && score < 0.6)
                    break;
                else if(score > 0.6)
                    outcomeCodePercent -= (outcomeCodePercent / (10 + round));
                else if(score < 0.4)
                    outcomeCodePercent += (outcomeCodePercent / (10 + round));

                Logger.log("b" + buyDist + " s" + sellDist + " =" + score, "Optimize to " + desiredDistribution);

                round++;
            }

            Logger.log("Sattle dist with: b" + buyDist + " s" + sellDist + " after " + round + " rounds", "Optimize to " + desiredDistribution);
        }

        private void testRandomIndicator()
        {
            //Create the random Indicator and retrive values
            WalkerIndicator indicator = generator.getRandomIndicator();
            double validR;
            double[] values = IndicatorRunner.getIndicatorValues(data, indicator, out validR);

            //Get min / max -10% of indicator
            double min, max;
            DistributionHelper.getMinMax(values, 10, out min, out max);

            //Sample the indicator with min max
            double[][] samplesMatrix = IndicatorSampler.sampleValuesOutcomeCode(values, outcomeCodes, min, max, 20);

            //Retrive the max for buy and sell
            double maxBuy, maxSell;
            DistributionHelper.getSampleCodesMinMax(samplesMatrix, out maxBuy, out maxSell);
            maxBuy -= 0.5;
            maxSell -= 0.5;

            //Retrive the correalations
            double spBuy, spSell, pBuy, pSell;
            IndicatorSampler.getStatistics(values, outcomeCodes, out spBuy, out spSell, out pBuy, out pSell);
            
            //Submit the results
            submitResults(spBuy + "," + spSell + "," + pBuy + "," + pSell + "," + maxBuy + "," + maxSell + "," + indicator.getName());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void submitResults(string results)
        {
            File.AppendAllText(resultFilePath, results + Environment.NewLine);
            Logger.log(results, "RESULT SUBMITTED");
            Logger.sendImportantMessage(results);
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
