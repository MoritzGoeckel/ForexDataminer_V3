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
    public class MarketModul
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

        private OpenPosition openPositionsLong, openPositionShort;
        private List<ClosedPosition> closedPositions = new List<ClosedPosition>();

        private double[] currentPriceData;

        public void pushPrice(double[] price)
        {
            this.bid = price[(int)PriceDataIndeces.Bid];
            this.ask = price[(int)PriceDataIndeces.Ask];
            this.timestamp = Convert.ToInt64(price[(int)PriceDataIndeces.Date]);

            this.currentPriceData = (double[])price.Clone();
        }

        public double[] getPriceData()
        {
            return currentPriceData;
        }

        public bool isPositionOpen(OrderType type)
        {
            if (type == OrderType.Long)
                return openPositionsLong != null;
            else
                return openPositionShort != null;
        }

        public OpenPosition getPosition(OrderType type)
        {
            if (type == OrderType.Long)
                return openPositionsLong;
            else
                return openPositionShort;
        }

        public bool openPosition(double amount, long timestamp, OrderType type)
        {
            OpenPosition p = new OpenPosition(amount, timestamp, (type == OrderType.Long ? ask : bid), type);

            if (type == OrderType.Long)
            {
                if (openPositionsLong == null)
                    openPositionsLong = p;
                else
                    return false;
            }
            else
            {
                if (openPositionShort == null)
                    openPositionShort = p;
                else
                    return false;
            }

            return true;
        }
        
        public bool closePosition(OrderType type, long timestamp)
        {
            OpenPosition p = null;

            if (type == OrderType.Long)
                p = openPositionsLong;
            else
                p = openPositionShort;

            if (p != null)
            {
                closedPositions.Add(new ClosedPosition(p, timestamp, (type == OrderType.Long ? bid : ask)));

                if (type == OrderType.Long)
                    openPositionsLong = null;
                else
                    openPositionShort = null;

                return true;
            }
            else
                return false;
        }

        public bool flatAll(long timestamp)
        {
            closePosition(OrderType.Long, timestamp);
            closePosition(OrderType.Short, timestamp);

            return true;
        }

        //Todo: does not regard amount yet

        public static double leverage = 1000 * 10;

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
            avgTimeframe = timeframes.Average() / 1000d / 60d;
            standartDeviationTimeframes = timeframes.StandardDeviation() / 1000d / 60d;

            return standartDeviation;
        }

        public string getTradesString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (ClosedPosition p in closedPositions)
                sb.Append(p.getProfit() + " with " + (p.type == OrderType.Long ? "Long" : "Short") + " " + p.getTimeDuration() / 1000d / 60d + "min" + Environment.NewLine);

            return sb.ToString();
        }

        public string getStatisticsString()
        {
            double standartDeviation, profit, sharpe, trades, tradesPerDay, profitPerTrade, tradesWinningRatio, winningTradesAvg, loosingTradesAvg, maxProfit, maxLoss, avgTimeframe, standartDeviationTimeframes;
            getStatistics(out standartDeviation, out profit, out sharpe, out trades, out tradesPerDay, out profitPerTrade, out tradesWinningRatio, out winningTradesAvg, out loosingTradesAvg, out maxProfit, out maxLoss, out avgTimeframe, out standartDeviationTimeframes);

            string sep = Environment.NewLine;

            return "Sharpe: " + sharpe + sep
                + "PnL: " + profit * leverage + sep
                + "stD: " + standartDeviation * leverage + sep
                + "Trades: " + trades + sep
                + "Trades/d: " + tradesPerDay + sep
                + "PnL/Trade: " + profitPerTrade * leverage + sep
                + "Winning: " + tradesWinningRatio + sep
                + "WinningAvg: " + winningTradesAvg * leverage + sep
                + "LoosingAvg: " + loosingTradesAvg * leverage + sep
                + "Max+: " + maxProfit * leverage + sep
                + "Max-: " + maxLoss * leverage + sep
                + "AvgTimeInMarket: " + avgTimeframe +"min"+ sep
                + "stDTimeInMarket: " + standartDeviationTimeframes + "min" + sep
                + "(assumed leverage: " + leverage / 1000 + "K)";
        }

        public Image getCapitalCurveVisualization(int width, int heigth)
        {
            double[] capital = new double[closedPositions.Count];
            double cumulativeCapital = 1000;
            for (int i = 0; i < closedPositions.Count; i++)
            {
                cumulativeCapital += closedPositions[i].getProfit();
                capital[i] = cumulativeCapital;
            }

            return ArrayVisualizer.visualizeArray(capital, width, heigth, 5);
        }
    }
}
