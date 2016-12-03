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
        private double sl = 1 * 2;
        private double predictionMulitplyer = 3;
        private double differenceMutliplyer = 3;
        private double outcomeCodesPropThreshold = 0.8;
        private double negativeCodesPropThreshold = 0.3;
        private double amount = 10 * 1000;
        private bool hedge = true;

        private bool invert = false;

        public FirstOrderMachine(MarketModul mm, double outcomeCodePercentage, long outcomeCodeTimestpan) : base(mm)
        {
            this.outcomeCodePercentage = outcomeCodePercentage;
            this.outcomeCodeTimestpan = outcomeCodeTimestpan;
        }

        private int BuySignals = 0, SellSignals = 0;
        private int BuyByPrediction = 0, SellByPrediction = 0, BuyByCodeProb = 0, SellByCodeProb = 0, BuyByDifference = 0, SellByDifference = 0;
        
        public override void doOrderTick(long timestamp, double[] signal)
        {
            string info = "";

            bool buySignal = false;
            bool sellSignal = false;

            //aim for outcome codes
            if(signal[(int)SignalMachineSignal.BuySignal] >= outcomeCodesPropThreshold
                && signal[(int)SignalMachineSignal.SellSignal] <= negativeCodesPropThreshold)
            {
                buySignal = true;
                BuyByCodeProb++;
                info += "codeprop";
            }

            if (signal[(int)SignalMachineSignal.SellSignal] >= outcomeCodesPropThreshold 
                && signal[(int)SignalMachineSignal.BuySignal] <= negativeCodesPropThreshold)
            {
                sellSignal = true;
                SellByCodeProb++;
                info += "codeprop";
            }

            //aim for prediction
            if (signal[(int)SignalMachineSignal.prediction] > outcomeCodePercentage * predictionMulitplyer)
            {
                buySignal = true;
                BuyByPrediction++;
                info += "pred";
            }

            if (signal[(int)SignalMachineSignal.prediction] < -outcomeCodePercentage * predictionMulitplyer)
            {
                sellSignal = true;
                SellByPrediction++;
                info += "pred";
            }

            //aim for min max difference
            if (signal[(int)SignalMachineSignal.maxPrediction]
                + signal[(int)SignalMachineSignal.minPrediction] > outcomeCodePercentage * differenceMutliplyer)
            {
                buySignal = true;
                BuyByDifference++;
                info += "diff";
            }

            if (signal[(int)SignalMachineSignal.maxPrediction]
                + signal[(int)SignalMachineSignal.minPrediction] < -outcomeCodePercentage * differenceMutliplyer)
            {
                sellSignal = true;
                SellByDifference++;
                info += "diff";
            }

            if (mm.getLastStreak() != null && mm.getLastStreak().win == false && mm.getLastStreak().streak > 4)
                invert = !invert;

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
                mm.openPosition(OrderHistoryTimeAnalysis.getHistoricProfitabilityWight(mm.getPositionHistory(), timestamp)
                    * amount,
                    timestamp,
                    MarketModul.OrderType.Long, info);

            if (sellSignal && mm.isPositionOpen(MarketModul.OrderType.Short) == false)
                mm.openPosition(OrderHistoryTimeAnalysis.getHistoricProfitabilityWight(mm.getPositionHistory(), timestamp)
                    * amount,
                    timestamp,
                    MarketModul.OrderType.Short, info);

            //OUT

            if (buySignal == false && mm.isPositionOpen(MarketModul.OrderType.Long))
            {
                OpenPosition p = mm.getPosition(MarketModul.OrderType.Long);
                if (p.getProfitPercent(mm.getPriceData()) <= sl * -outcomeCodePercentage || p.getProfitPercent(mm.getPriceData()) >= outcomeCodePercentage * tp || p.getTimeInMarket(mm.getPriceData()) >= outcomeCodeTimestpan)
                {
                    mm.closePosition(MarketModul.OrderType.Long, timestamp);
                    mm.removeInvalidTimeFramePositions(outcomeCodeTimestpan * 10, outcomeCodePercentage + 0.05, -(outcomeCodePercentage + 0.05));
                    //Just for simulation to remove the invalid positions todo:
                }
            }

            if (sellSignal == false && mm.isPositionOpen(MarketModul.OrderType.Short))
            {
                OpenPosition p = mm.getPosition(MarketModul.OrderType.Short);
                if (p.getProfitPercent(mm.getPriceData()) <= sl * -outcomeCodePercentage || p.getProfitPercent(mm.getPriceData()) >= outcomeCodePercentage * tp || p.getTimeInMarket(mm.getPriceData()) >= outcomeCodeTimestpan)
                {
                    mm.closePosition(MarketModul.OrderType.Short, timestamp);
                    mm.removeInvalidTimeFramePositions(outcomeCodeTimestpan * 10, outcomeCodePercentage + 0.05, -(outcomeCodePercentage + 0.05));
                    //Just for simulation to remove the invalid positions todo:
                }
            }
        }

        public override string getStatistics()
        {
            string sep = Environment.NewLine;
            StringBuilder s = new StringBuilder();
            s.Append("CodeProp:" + sep + "  B: " + BuyByCodeProb + sep + "  S: " + SellByCodeProb + sep);
            s.Append("Prediction:" + sep + "  B: " + BuyByPrediction + sep + "  S: " + SellByPrediction + sep);
            s.Append("Difference:" + sep + "  B: " + BuyByDifference + sep + "  S: " + SellByDifference + sep);
            s.Append("Signals:" + sep + "  B: " + BuySignals + sep + "  S: " + SellSignals);

            return s.ToString();
        }
    }
}
