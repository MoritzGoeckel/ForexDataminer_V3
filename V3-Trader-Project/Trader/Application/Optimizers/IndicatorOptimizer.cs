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
        private string resultFolderPath;
        private TestingEnvironment env;

        public string state;

        public IndicatorOptimizer(string resultFolderPath, TestingEnvironment env)
        {
            this.env = env;
            this.resultFolderPath = resultFolderPath;
        }
        
        private bool running = false;
        public void startRunningRandomIndicators(IndicatorGenerator generator)
        {
            if (running == true)
                throw new Exception("Already running!");

            if (env.outcomes == null || env.outcomeCodes == null)
                throw new Exception("Set outcomes and outcomeCodes first");      

            submitResults(LearningIndicator.getPredictivePowerArrayHeader()+"usedValues;name;id");

            Logger.log("Start testing indicators");
            new Thread(delegate () {
                running = true;
                while(running)
                {
                    //How about a genetic algo?
                    try {
                        testAndSubmitResult(generator.getRandomIndicator(Convert.ToInt32(env.outcomeTimeframe / 1000 / 15), Convert.ToInt32(env.outcomeTimeframe * 100 / 1000)));
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
        
        private void testAndSubmitResult(WalkerIndicator indicator)
        {
            Logger.log("Testing Indicator: " + indicator.getName());

            LearningIndicator li = env.createTrainedIndicator(indicator);
            double[] pp = li.getPredictivePowerArray();

            //Results
            string output = "";
            foreach (double d in pp)
                output += d + ";";

            output += li.getUsedValues() + ";";
            output += indicator.getName().Split('_')[0] + ";" + indicator.getName();

            Logger.log("Result: " + Math.Round(li.getPredictivePowerScore(), 4) + " " + li.getName());
            state = Math.Round(li.getPredictivePowerScore(), 4) + " " + li.getName();
            submitResults(output);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void submitResults(string results)
        {
            string fileName = "outcomeIndicators_" + env.outcomeCodePercent + "_" + env.outcomeTimeframe + ".csv";
            File.AppendAllText(resultFolderPath + fileName, results + Environment.NewLine);
        }
    }
}
