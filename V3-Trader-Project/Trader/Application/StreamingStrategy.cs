﻿using NinjaTrader_Client.Trader.Indicators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V3_Trader_Project.Trader.Application.IndicatorSelectors;
using V3_Trader_Project.Trader.Application.OrderMachines;
using V3_Trader_Project.Trader.Market;
using V3_Trader_Project.Trader.SignalMachines;

namespace V3_Trader_Project.Trader.Application
{
    class StreamingStrategy
    {
        double outcomeCodePercent;
        long outcomeTimeframe;
        OrderMachine orderMachine;

        SignalMachine signalMachine;
        double minPercentThreshold;

        string cachePath;
        string cachedIndicatorsFile;

        List<string> okayIndicators = new List<string>();

        public StreamingStrategy(double outcomeCodePercent, long outcomeTimeframe, MarketModul mm, OrderMachine om, double minPercentThreshold, string cachePath = null)
        {
            this.outcomeCodePercent = outcomeCodePercent;
            this.outcomeTimeframe = outcomeTimeframe;

            this.minPercentThreshold = minPercentThreshold;
            this.cachePath = cachePath;

            if (cachePath != null && Directory.Exists(cachePath) == false)
            {
                Directory.CreateDirectory(cachePath);
                Logger.log("Created log directory: " + cachePath);
            }

            this.cachedIndicatorsFile = cachePath + "/" + "cachedIndicators"+ outcomeTimeframe +".txt";
            if (cachePath != null && File.Exists(cachedIndicatorsFile))
                okayIndicators = File.ReadAllText(cachedIndicatorsFile).Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            else
                okayIndicators = new List<string>();
            
            this.orderMachine = om;
        }

        public void updateIndicators(long timeframeToLookBack, long timeframeToLookBackForIndicatorInit, IndicatorSelector indicatorSelector)
        {
            List<double[]> selectedPriceData = new List<double[]>();
            for (int i = priceData.Count - 1; i > 0; i--)
            {
                if (Convert.ToInt64(priceData[i][(int)PriceDataIndeces.Date]) > timestampNow - timeframeToLookBack)
                    selectedPriceData.Insert(0, priceData[i]); //Todo: List direction correct?
                else
                    break;
            }

            double[][] selectedPriceDataArray = selectedPriceData.ToArray();
            double s;
            double[][] outcomeData = OutcomeGenerator.getOutcome(selectedPriceDataArray, outcomeTimeframe, out s);
            if (s < 0.6) throw new Exception("s < o.6: " + s);
            bool[][] outcomeCodeData = OutcomeGenerator.getOutcomeCode(selectedPriceDataArray, outcomeData, outcomeCodePercent, out s);
            if (s < 0.6) throw new Exception("s < o.6: " + s);

            string[] indicatorIds;

            //This part can be skipped by caching
            double hash = outcomeTimeframe + selectedPriceData[0].Sum() + selectedPriceData[selectedPriceData.Count - 1].Sum() + selectedPriceData[selectedPriceData.Count / 2].Sum();
            string optimalIndicatorsFileName = cachePath + "/" + "optimalIndicatorsIn_" + hash + "_" + selectedPriceData[selectedPriceData.Count - 1][(int)PriceDataIndeces.Date] + "_" + timeframeToLookBack + "_" + outcomeCodePercent + ".txt";
            if (cachePath != null && File.Exists(optimalIndicatorsFileName))
            {
                indicatorIds = File.ReadAllText(optimalIndicatorsFileName).Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                Logger.log("Loaded optimal indicators from file: " + optimalIndicatorsFileName);
            }
            else
            {
                //Shuffle okay indicators? todo:
                Logger.log("Generated optimal indicators");
                IndicatorOptimizer optimizer = new IndicatorOptimizer(selectedPriceDataArray, outcomeData, outcomeCodeData, outcomeTimeframe, outcomeCodePercent, minPercentThreshold);
                IndicatorGenerator generator = new IndicatorGenerator(okayIndicators);
                indicatorIds = optimizer.getOptimizedIndicators(generator, indicatorSelector, 7);

                List<string> newGoodIndicators = generator.getGoodGeneratedIndicators();
                if (cachePath != null)
                {
                    File.AppendAllLines(cachedIndicatorsFile, newGoodIndicators);
                    File.WriteAllLines(optimalIndicatorsFileName, indicatorIds);
                }

                okayIndicators.AddRange(newGoodIndicators);
            }

            Logger.log("Selected indicators: ");
            List<LearningIndicator> lis = new List<LearningIndicator>();
            foreach (string str in indicatorIds)
            {
                Logger.log(str);

                WalkerIndicator ind = IndicatorGenerator.getIndicatorByString(str);
                if (ind.getName() != str)
                    throw new Exception(str + "!=" + ind.getName());

                lis.Add(new LearningIndicator(ind, selectedPriceDataArray, outcomeCodeData, outcomeData, outcomeTimeframe, outcomeCodePercent, minPercentThreshold));
            }

            SignalMachine sm = new LIWightedSignalMachine(lis.ToArray());
            Logger.log("SM STATE: ##################" + Environment.NewLine + sm.getStateMessage());

            //Make them up to date
            List<double[]> selectedPriceDataForIndicatorInit = new List<double[]>();
            for (int i = priceData.Count - 1; i > 0; i--)
            {
                if (Convert.ToInt64(priceData[i][(int)PriceDataIndeces.Date]) > timestampNow - timeframeToLookBackForIndicatorInit)
                    selectedPriceDataForIndicatorInit.Insert(0, priceData[i]); //Todo: List direction correct?
                else
                    break;
            }

            foreach (double[] row in selectedPriceDataForIndicatorInit)
                sm.pushPrice(row);

            this.signalMachine = sm;
        }

        long timestampNow;
        private List<double[]> priceData = new List<double[]>();
        public void pushPrice(double[] newPriceData)
        {
            if (timestampNow > newPriceData[(int)PriceDataIndeces.Date])
                throw new Exception("Not new data");

            priceData.Add(newPriceData);
            timestampNow = Convert.ToInt64(newPriceData[(int)PriceDataIndeces.Date]);

            if(signalMachine != null)
            {
                signalMachine.pushPrice(newPriceData);
                double[] signal = signalMachine.getSignal(timestampNow);

                orderMachine.doOrderTick(timestampNow, signal);
            }
        }
    }
}