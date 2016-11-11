using NinjaTrader_Client.Trader.Indicators;
using System;
using System.Collections.Generic;
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

        List<string> provenIndicators = new List<string>();

        public StreamingStrategy(double outcomeCodePercent, long outcomeTimeframe, MarketModul mm, double minPercentThreshold)
        {
            this.outcomeCodePercent = outcomeCodePercent;
            this.outcomeTimeframe = outcomeTimeframe;

            this.minPercentThreshold = minPercentThreshold;

            //Make this accessable from outside? todo:
            this.orderMachine = new FirstOrderMachine(mm, outcomeCodePercent, outcomeTimeframe);
        }

        public void updateIndicators(long timeframeToLookBack, IndicatorSelector indicatorSelector)
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

            IndicatorOptimizer optimizer = new IndicatorOptimizer(selectedPriceDataArray, outcomeData, outcomeCodeData, outcomeTimeframe, outcomeCodePercent, minPercentThreshold);
            string[] indicatorIds = optimizer.getOptimizedIndicators(new IndicatorGenerator(provenIndicators.ToArray()), indicatorSelector);

            foreach (string ind in indicatorIds)
            {
                if(provenIndicators.Contains(ind) == false)
                provenIndicators.Add(ind);
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

            foreach (double[] row in selectedPriceDataArray)
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
