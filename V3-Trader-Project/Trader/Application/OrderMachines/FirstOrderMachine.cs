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

        public FirstOrderMachine(MarketModul mm, double outcomeCodePercentage, long outcomeCodeTimestpan) : base(mm)
        {
            this.outcomeCodePercentage = outcomeCodePercentage;
            this.outcomeCodeTimestpan = outcomeCodeTimestpan;
        }

        public int BuySignals = 0, SellSignals = 0;

        public override void doOrderTick(long timestamp, double[] signal)
        {
            bool buySignal = false;
            bool sellSignal = false;

            double outcomeCodesPropThreshold = 0.85;

            //aim for outcome codes
            if(signal[(int)SignalMachineSignal.BuySignal] > outcomeCodesPropThreshold)
            {
                buySignal = true;
            }
            else if (signal[(int)SignalMachineSignal.SellSignal] > outcomeCodesPropThreshold)
            {
                sellSignal = true;
            }

            //aim for prediction
            /*if (signal[(int)SignalMachineSignal.prediction] > 0.2)
            {
                buySignal = true;
            }
            else if (signal[(int)SignalMachineSignal.prediction] < -0.2)
            {
                sellSignal = true;
            }

            //aim for min max difference
            if (signal[(int)SignalMachineSignal.maxPrediction]
                + signal[(int)SignalMachineSignal.minPrediction] > 0.2)
            {
                buySignal = true;
            }
            else if(signal[(int)SignalMachineSignal.maxPrediction]
                + signal[(int)SignalMachineSignal.minPrediction] < -0.2)
            {
                sellSignal = true;
            }*/

            sellSignal = sellSignal && buySignal == false;
            buySignal = buySignal && sellSignal == false;

            if (sellSignal)
                SellSignals++;

            if (buySignal)
                BuySignals++;

            if (buySignal && mm.isPositionOpen(MarketModul.OrderType.Long) == false)
                mm.openPosition(1, timestamp, MarketModul.OrderType.Long);

            if (sellSignal && mm.isPositionOpen(MarketModul.OrderType.Short) == false)
                mm.openPosition(1, timestamp, MarketModul.OrderType.Short);

            if (buySignal == false && mm.isPositionOpen(MarketModul.OrderType.Long))
            {
                OpenPosition p = mm.getPosition(MarketModul.OrderType.Long);
                if(p.getProfitPercent(mm.getPriceData()) <= 15 * -outcomeCodePercentage || p.getProfitPercent(mm.getPriceData()) >= outcomeCodePercentage || p.getTimeInMarket(mm.getPriceData()) >= outcomeCodeTimestpan)
                    mm.closePosition(MarketModul.OrderType.Long, timestamp);
            }

            if (sellSignal == false && mm.isPositionOpen(MarketModul.OrderType.Short))
            {
                OpenPosition p = mm.getPosition(MarketModul.OrderType.Short);
                if (p.getProfitPercent(mm.getPriceData()) <= 15 * -outcomeCodePercentage || p.getProfitPercent(mm.getPriceData()) >= outcomeCodePercentage || p.getTimeInMarket(mm.getPriceData()) >= outcomeCodeTimestpan)
                    mm.closePosition(MarketModul.OrderType.Short, timestamp);
            }
        }
    }
}
