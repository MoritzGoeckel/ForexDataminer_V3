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
        private double[][] data;

        private double outcomeCodePercent;
        private double outcomeTimeframe;

        private double[][] outcome;
        private double[][] outcomeCodes;

        private IndicatorGenerator generator = new IndicatorGenerator();

        public string resultFilePath;

        public IndicatorOptimizer(string resultFilePath, string dataPath)
        {
            DataLoader dl = new DataLoader(dataPath);
            data = dl.getArray();

            this.resultFilePath = resultFilePath;

            //Reduce data?
        }

        private bool running = false;
        public void start()
        {
            if (running == true)
                throw new Exception("Already running!");

            //Todo provide neccessary variables
            findOutcomeCode();

            new Thread(delegate () {
                running = true;
                while(running)
                {
                    testIndicator();
                }
            }).Start();
        }

        public void stop()
        {
            running = false;
        }

        private double findOutcomeCode(double desiredDistribution, long timeframeMs )
        {
            //Todo optimize
            return 0; //Percent gain
        }

        private void testIndicator()
        {
            //todo test it
            submitResults();
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
