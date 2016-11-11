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
    class IndicatorOptimizerExcel
    {
        private string resultFolderPath;

        private double[][] priceData, outcomeData;
        private bool[][] outcomeCodeData;
        private double buyDist, sellDist, outcomeCodePercent;
        private long outcomeTimeframe;

        public string state;

        public IndicatorOptimizerExcel(string resultFolderPath, double[][] priceData, double[][] outcomeData, bool[][] outcomeCodeData, long outcomeTimeframe, double buyDist, double sellDist, double outcomeCodePercent)
        {
            this.resultFolderPath = resultFolderPath;
            this.priceData = priceData;
            this.outcomeData = outcomeData;
            this.outcomeCodeData = outcomeCodeData;

            this.buyDist = buyDist;
            this.sellDist = sellDist;
            this.outcomeCodePercent = outcomeCodePercent;
            this.outcomeTimeframe = outcomeTimeframe;
        }

        private bool running = false;
        public void startRunningRandomIndicators(IndicatorGenerator generator)
        {
            if (running == true)
                throw new Exception("Already running!");

            submitResults(LearningIndicator.getPredictivePowerArrayHeader()+"usedValues;name;id");

            Logger.log("Start testing indicators");
            new Thread(delegate () {
                running = true;
                while(running)
                {
                    //How about a genetic algo?
                    try {
                        testAndSubmitResult(generator.getRandomIndicator(Convert.ToInt32(outcomeTimeframe / 1000 / 15), Convert.ToInt32(outcomeTimeframe * 100 / 1000)));
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

            LearningIndicator li = new LearningIndicator(indicator, priceData, outcomeCodeData, outcomeData, outcomeTimeframe, outcomeCodePercent, 0.5);
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
            string fileName = "outcomeIndicators_" + outcomeCodePercent + "_" + outcomeTimeframe + ".csv";
            File.AppendAllText(resultFolderPath + fileName, results + Environment.NewLine);
        }
    }
}
