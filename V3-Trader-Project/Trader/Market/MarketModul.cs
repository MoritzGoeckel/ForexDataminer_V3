using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;
using System.Drawing;
using V3_Trader_Project.Trader.Visualizers;

namespace V3_Trader_Project.Trader.Market
{
    class MarketModul
    {
        public enum OrderType : int
        {
            Long = 0, Short = 1
        }

        public MarketModul(string pair)
        {

        }

        private double bid, ask;
        private long timestamp = -1;

        private List<OpenPosition> openPositions = new List<OpenPosition>();
        private List<ClosedPosition> closedPositions = new List<ClosedPosition>();

        public void pushPrice(double bid, double ask, long timestamp)
        {
            this.bid = bid;
            this.ask = ask;
            this.timestamp = timestamp;
        }

        public bool openPosition(double amount, long timestamp, OrderType type)
        {
            openPositions.Add(new OpenPosition(amount, timestamp, (type == OrderType.Long ? ask : bid), type));
            return true;
        }
        
        public bool closePosition(OrderType type, long timestamp)
        {
            bool didSomething = false;
            for(int i = 0; i < openPositions.Count; i++)
            {
                if(openPositions[i].type == type)
                {
                    closedPositions.Add(new ClosedPosition(openPositions[i], timestamp, (type == OrderType.Long ? bid : ask)));
                    openPositions.RemoveAt(i);
                    didSomething = true;
                    i--;
                }
            }

            return didSomething;
        }

        public bool flatAll()
        {
            bool didSomething = false;
            while(openPositions.Count > 0)
            {
                closedPositions.Add(new ClosedPosition(openPositions[0], timestamp, (openPositions[0].type == OrderType.Long ? bid : ask)));
                openPositions.RemoveAt(0);
                didSomething = true;
            }

            return didSomething;
        }

        public double getStatistics(out double standartDeviation, out double profit, out double sharpe, out double trades, out double tradesPerDay, out double profitPerTrade, out double tradesWinningRatio, out double winningTradesAvg, out double loosingTradesAvg, out double maxProfit, out double maxLoss, out double avgTimeframe, out double standartDeviationTimeframes)
        {
            double[] profits = new double[closedPositions.Count];
            double[] timeframes = new double[closedPositions.Count];

            for (int i = 0; i < closedPositions.Count; i++)
            {
                profits[i] = closedPositions[i].getProfit();
                timeframes[i] = closedPositions[i].getTimeDuration();
            }

            standartDeviation = profits.StandardDeviation();
            profit = profits.Sum();
            sharpe = profit / standartDeviation;
            trades = profits.Length;
            tradesPerDay = trades / ((closedPositions[closedPositions.Count - 1].timestampClose - closedPositions[0].timestampOpen) / 1000 / 60 / 60 / 24);
            profitPerTrade = profit / trades;
            tradesWinningRatio = profits.Count((p => p > 0)) / Convert.ToDouble(profits.Count());
            winningTradesAvg = profits.Sum((p => (p > 0 ? p : 0))) / Convert.ToDouble(profits.Count((p => p > 0)));
            loosingTradesAvg = profits.Sum((p => (p < 0 ? p : 0))) / Convert.ToDouble(profits.Count((p => p < 0)));
            maxProfit = profits.Maximum();
            maxLoss = profits.Minimum();
            avgTimeframe = timeframes.Average();
            standartDeviationTimeframes = timeframes.StandardDeviation();

            return standartDeviation;
        }

        public string getStatisticsString()
        {
            double standartDeviation, profit, sharpe, trades, tradesPerDay, profitPerTrade, tradesWinningRatio, winningTradesAvg, loosingTradesAvg, maxProfit, maxLoss, avgTimeframe, standartDeviationTimeframes;
            getStatistics(out standartDeviation, out profit, out sharpe, out trades, out tradesPerDay, out profitPerTrade, out tradesWinningRatio, out winningTradesAvg, out loosingTradesAvg, out maxProfit, out maxLoss, out avgTimeframe, out standartDeviationTimeframes);

            string sep = Environment.NewLine;

            return "Sharpe: " + sharpe + sep
                + "PnL: " + profit + sep
                + "stD: " + standartDeviation + sep
                + "Trades: " + trades + sep
                + "Trades/d: " + tradesPerDay + sep
                + "PnL/Trade: " + profitPerTrade + sep
                + "Winning: " + tradesWinningRatio + sep
                + "WinningAvg: " + winningTradesAvg + sep
                + "LoosingAvg: " + loosingTradesAvg + sep
                + "Max+: " + maxProfit + sep
                + "Max-: " + maxLoss + sep
                + "AvgTimeInMarket: " + avgTimeframe + sep
                + "stDTimeInMarket: " + standartDeviationTimeframes;
        }

        public Image getCapitalCurveVisualization(int width, int heigth)
        {
            double[] capital = new double[closedPositions.Count];
            double cumulativeCapital = 0;
            for (int i = 0; i < closedPositions.Count; i++)
            {
                cumulativeCapital += closedPositions[i].getProfit();
                capital[i] = cumulativeCapital;
            }

            return ArrayVisualizer.visualizeArray(capital, width, heigth, 5);
        }
    }
}
