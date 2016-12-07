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
using V3_Trader_Project.Trader.Application.IndicatorSelectors;
using V3_Trader_Project.Trader.Visualizers;

namespace V3_Trader_Project.Trader.Application
{
    public class IndicatorOptimizer
    {
        private double[][] priceData, outcomeData;
        private bool[][] outcomeCodeData;
        private double outcomeCodePercent;
        private long outcomeTimeframe;
        private double minPercentThreshold;

        private int learningIndicatorSteps;

        public IndicatorOptimizer(double[][] priceData, double[][] outcomeData, bool[][] outcomeCodeData, long outcomeTimeframe, double outcomeCodePercent, double minPercentThreshold, int learningIndicatorSteps)
        {
            this.learningIndicatorSteps = learningIndicatorSteps;

            this.priceData = priceData;
            this.outcomeData = outcomeData;
            this.outcomeCodeData = outcomeCodeData;

            this.minPercentThreshold = minPercentThreshold;

            this.outcomeCodePercent = outcomeCodePercent;
            this.outcomeTimeframe = outcomeTimeframe;
        }

        bool ended = false;
        double indicatorsToTryCount;

        public string[] getOptimizedIndicators(List<string> indicatorsToTry, IndicatorSelector selector, int threads)
        {
            ended = false;
            indicatorToTryIndex = 0;
            indicatorsToTryCount = indicatorsToTry.Count;

            Logger.log("Start testing indicators");
            List<Thread> ths = new List<Thread>();
            
            for(int i = 0; i < threads; i++)
                ths.Add(new Thread(delegate () { optimizeInternally(indicatorsToTry, selector); }));

            foreach (Thread t in ths)
                t.Start();

            foreach(Thread t in ths)
            {
                if(t.IsAlive)
                    t.Join();
            }

            return selector.getResultingCandidates();
        }

        private int indicatorToTryIndex = 0;

        [MethodImpl(MethodImplOptions.Synchronized)]
        private int getNextIndex()
        {
            Logger.log("Trying Indicators: " + Math.Round((indicatorToTryIndex / indicatorsToTryCount) * 100d, 2) + "%");
            return indicatorToTryIndex++;
        }

        private void optimizeInternally(List<string> indicatorsToTry, IndicatorSelector selector)
        {
            while (ended == false)
            {
                try
                {
                    //generator.getGeneratedIndicator(Convert.ToInt32(outcomeTimeframe / 1000 / 15), Convert.ToInt32(outcomeTimeframe * 100 / 1000));
                    int index = getNextIndex();
                    if (index >= indicatorsToTry.Count)
                        break;

                    WalkerIndicator wi = IndicatorGenerator.getIndicatorByString(indicatorsToTry[index]);
                    LearningIndicator li = new LearningIndicator(wi, priceData, outcomeCodeData, outcomeData, outcomeTimeframe, outcomeCodePercent, minPercentThreshold, learningIndicatorSteps, true);

                    selector.pushIndicatorStatistics(li);
                }
                catch (TooLittleValidDataException e)
                {
                    //Logger.log("E:" + e.Message);
                }
                catch (TooLittleStatesException e)
                {
                    //Logger.log("E:" + e.Message);
                }
                catch (Exception e)
                {
                    Logger.log("FATAL:" + e.Message);
                }
            }

            ended = true;
        }
    }
}
