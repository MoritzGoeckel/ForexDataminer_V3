using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V3_Trader_Project.Trader.Market;
using V3_Trader_Project.Trader.SignalMachines;

namespace V3_Trader_Project.Trader.Application.OrderMachines
{
    class FirstOrderMachine : OrderMachine
    {
        private double outcomeCodePercentage;
        private long outcomeCodeTimestpan;

        //Todo: Outside accessable
        private double tp = 1;
        private double sl = 1;

        //private double predictionMulitplyer = 8;
        private double outcomeCodesPropThreshold = 0.6;
        private double negativeOutcomeCodesPropThreshold = 0.6;

        private double amount = 10 * 1000;
        private bool hedge = true;

        //private double predictionDifferenceMutliplyer = 6;
        //private double buySellDifferenceThreshold = 0.3;

        private bool enableInverse = false;
        private int inverseFrequency = 10;
        private double inverseThreshold = 0.3;

        private bool invert = false;

        public FirstOrderMachine(MarketModul mm, double outcomeCodePercentage, long outcomeCodeTimestpan) : base(mm)
        {
            this.outcomeCodePercentage = outcomeCodePercentage;
            this.outcomeCodeTimestpan = outcomeCodeTimestpan;
            this.cleanedHistory = mm.getCleanedClosedPositions(outcomeCodeTimestpan * 3, 100, -100);
        }

        private int BuySignals = 0, SellSignals = 0;

        private int tradeNum = 0;
        private int tradeNumAtReverse = 0;

        private List<ClosedPosition> cleanedHistory;

        public override void doOrderTick(long timestamp, double[] signal)
        {
            string tags = ";";

            bool buySignal = false;
            bool sellSignal = false;

            //aim for outcome codes
            if(signal[(int)SignalMachineSignal.BuyProbability] >= outcomeCodesPropThreshold 
                && signal[(int)SignalMachineSignal.SellProbability] <= negativeOutcomeCodesPropThreshold)
            {
                buySignal = true;
                tags += "codeprop;";
            }

            if (signal[(int)SignalMachineSignal.SellProbability] >= outcomeCodesPropThreshold
                && signal[(int)SignalMachineSignal.BuyProbability] <= negativeOutcomeCodesPropThreshold)
            {
                sellSignal = true;
                tags += "codeprop;";
            }

            //aim for outcomecodes difference
            /*if (signal[(int)SignalMachineSignal.BuyProbability] - signal[(int)SignalMachineSignal.SellProbability] >= buySellDifferenceThreshold)
            {
                buySignal = true;
                tags += "codediff;";
            }

            if (signal[(int)SignalMachineSignal.SellProbability] - signal[(int)SignalMachineSignal.BuyProbability] >= buySellDifferenceThreshold)
            {
                sellSignal = true;
                tags += "codediff;";
            }*/

            //aim for prediction
            /*if (signal[(int)SignalMachineSignal.prediction] > outcomeCodePercentage * predictionMulitplyer)
            {
                buySignal = true;
                tags += "pred;";
            }

            if (signal[(int)SignalMachineSignal.prediction] < -outcomeCodePercentage * predictionMulitplyer)
            {
                sellSignal = true;
                tags += "pred;";
            }*/

            //aim for min max difference
            /*if (signal[(int)SignalMachineSignal.maxPrediction]
                + signal[(int)SignalMachineSignal.minPrediction] > outcomeCodePercentage * predictionDifferenceMutliplyer)
            {
                buySignal = true;
                tags += "preddiff;";
            }

            if (signal[(int)SignalMachineSignal.maxPrediction]
                + signal[(int)SignalMachineSignal.minPrediction] < -outcomeCodePercentage * predictionDifferenceMutliplyer)
            {
                sellSignal = true;
                tags += "preddiff;";
            }*/

            if (enableInverse && tradeNum - tradeNumAtReverse > inverseFrequency
                && OrderHistoryStreakAnalysis.getWinRateLastTrades(inverseFrequency, cleanedHistory) < inverseThreshold)
            {
                invert = !invert;
                tradeNumAtReverse = tradeNum;
            }

            if (invert)
                tags += "inv;";

            if (invert && buySignal != sellSignal)
            {
                sellSignal = !sellSignal;
                buySignal = !buySignal;
            }
            
            if(hedge == false && buySignal == sellSignal)
            {
                buySignal = sellSignal = false;
            }

            //IN

            if (sellSignal)
                SellSignals++;

            if (buySignal)
                BuySignals++;

            if (buySignal && mm.isPositionOpen(MarketModul.OrderType.Long) == false)
                mm.openPosition(OrderHistoryTimeAnalysis.getHistoricProfitabilityWight(cleanedHistory, timestamp)
                    * amount,
                    timestamp,
                    MarketModul.OrderType.Long, tags);

            if (sellSignal && mm.isPositionOpen(MarketModul.OrderType.Short) == false)
                mm.openPosition(
                    OrderHistoryTimeAnalysis.getHistoricProfitabilityWight(cleanedHistory, timestamp)
                    * amount,
                    timestamp,
                    MarketModul.OrderType.Short, tags);

            //OUT

            if (buySignal == false && mm.isPositionOpen(MarketModul.OrderType.Long)) 
            {
                OpenPosition p = mm.getPosition(MarketModul.OrderType.Long);
                if (p.getProfitPercent(mm.getPriceData()) <= sl * -outcomeCodePercentage || p.getProfitPercent(mm.getPriceData()) >= outcomeCodePercentage * tp || p.getTimeInMarket(mm.getPriceData()) >= outcomeCodeTimestpan)
                {
                    tradeNum++;
                    mm.closePosition(MarketModul.OrderType.Long, timestamp);

                    cleanedHistory = mm.getCleanedClosedPositions(outcomeCodeTimestpan * 3, 100, -100);
                }
            }

            if (sellSignal == false && mm.isPositionOpen(MarketModul.OrderType.Short))
            {
                OpenPosition p = mm.getPosition(MarketModul.OrderType.Short);
                if (p.getProfitPercent(mm.getPriceData()) <= sl * -outcomeCodePercentage || p.getProfitPercent(mm.getPriceData()) >= outcomeCodePercentage * tp || p.getTimeInMarket(mm.getPriceData()) >= outcomeCodeTimestpan)
                {
                    tradeNum++;
                    mm.closePosition(MarketModul.OrderType.Short, timestamp);

                    cleanedHistory = mm.getCleanedClosedPositions(outcomeCodeTimestpan * 3, 100, -100);
                }
            }
        }

        public override string getStatistics()
        {
            string sep = Environment.NewLine;
            StringBuilder s = new StringBuilder();
            //Nothin to say :)
            return s.ToString();
        }
    }
}
