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

        public List<ClosedPosition> getPositionHistory()
        {
            return closedPositions;
        }

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

        public bool openPosition(double amount, long timestamp, OrderType type, string info = null)
        {
            OpenPosition p = new OpenPosition(amount, timestamp, (type == OrderType.Long ? ask : bid), type, info);

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

        public double getStatistics(out double standartDeviation, out double profit, out double profitIngoreAmount, out double sharpe, out double trades, out double tradesPerDay, out double profitPerTrade, out double tradesWinningRatio, out double winningTradesAvg, out double loosingTradesAvg, out double maxProfit, out double maxLoss, out double avgTimeframe, out double standartDeviationTimeframes, out double volume)
        {
            double[] profits = new double[closedPositions.Count];
            double[] timeframes = new double[closedPositions.Count];
            profitIngoreAmount = 0;

            volume = 0;

            for (int i = 0; i < closedPositions.Count; i++)
            {
                profitIngoreAmount += (closedPositions[i].getProfitIngoreAmount() * 10 * 1000);
                profits[i] = closedPositions[i].getProfit();
                timeframes[i] = closedPositions[i].getTimeDuration();
                volume += closedPositions[i].amount;
            }

            standartDeviation = profits.StandardDeviation();
            profit = profits.Sum();
            sharpe = profit / standartDeviation;
            trades = closedPositions.Count;
            tradesPerDay = trades / ((closedPositions[closedPositions.Count - 1].timestampClose - closedPositions[0].timestampOpen) / 1000 / 60 / 60 / 24);
            profitPerTrade = profit / trades;
            tradesWinningRatio = profits.Count((p => p > 0)) / Convert.ToDouble(closedPositions.Count);
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
                sb.Append(p.getProfitIngoreAmount() + " with " + (p.type == OrderType.Long ? "Long" : "Short") + " " + p.getTimeDuration() / 1000d / 60d + "min" + Environment.NewLine);

            return sb.ToString();
        }

        //Sanity check
        public void removeInvalidTimeFramePositions(long maxTimeframeToConsider, double maxProfit, double maxLoss)
        {
            List<ClosedPosition> newClosedPositions = new List<ClosedPosition>();
            foreach (ClosedPosition c in closedPositions)
            {
                double profitPercent = c.getProfitPercent();
                if (c.getTimeDuration() <= maxTimeframeToConsider && profitPercent < maxProfit && profitPercent > maxLoss)
                    newClosedPositions.Add(c);
            }

            closedPositions = newClosedPositions;
        }

        public string getStatisticsString()
        {
            double standartDeviation, profit, profitIngoreAmount, sharpe, trades, tradesPerDay, profitPerTrade, tradesWinningRatio, winningTradesAvg, loosingTradesAvg, maxProfit, maxLoss, avgTimeframe, standartDeviationTimeframes, volume;
            getStatistics(out standartDeviation, out profit, out profitIngoreAmount, out sharpe, out trades, out tradesPerDay, out profitPerTrade, out tradesWinningRatio, out winningTradesAvg, out loosingTradesAvg, out maxProfit, out maxLoss, out avgTimeframe, out standartDeviationTimeframes, out volume);

            string sep = Environment.NewLine;

            return "Sharpe: " + sharpe + sep
                + "PnL: " + profit + sep
                + "PnL noA: " + profitIngoreAmount + sep
                + "stD: " + standartDeviation + sep
                + "Trades: " + trades + sep
                + "Trades/d: " + tradesPerDay + sep
                + "PnL/Trade: " + profitPerTrade + sep
                + "Winning: " + tradesWinningRatio + sep
                + "WinningAvg: " + winningTradesAvg + sep
                + "LoosingAvg: " + loosingTradesAvg + sep
                + "Max+: " + maxProfit + sep
                + "Max-: " + maxLoss + sep
                + "AvgTimeInMarket: " + avgTimeframe + "min" + sep
                + "stDTimeInMarket: " + standartDeviationTimeframes + "min" + sep
                + "Volume: " + volume;
        }

        private struct ValueCountPair { public double value; public int count; };

        public string getProfitabilityByInfoString()
        {
            Dictionary<string, ValueCountPair> infoDict = new Dictionary<string, ValueCountPair>();
            foreach (ClosedPosition c in closedPositions)
            {
                if(c.info != null)
                {
                    if (infoDict.ContainsKey(c.info) == false)
                        infoDict.Add(c.info, new ValueCountPair() { value = 0, count = 0 });

                    ValueCountPair p = infoDict[c.info];
                    p.value += c.getProfitPercent();
                    p.count++;

                    infoDict[c.info] = p;
                }
            }

            StringBuilder s = new StringBuilder();
            foreach(KeyValuePair<string, ValueCountPair> pair in infoDict)
            {
                s.Append(pair.Key + ": " + pair.Value.value / Convert.ToDouble(pair.Value.count) + "%/t (" + pair.Value.count + ")" + Environment.NewLine);
            }

            return s.ToString();
        }

        public Image getCapitalCurveVisualization(int width, int heigth)
        {
            double[] capital = new double[closedPositions.Count];
            double cumulativeCapital = 1000;
            for (int i = 0; i < closedPositions.Count; i++)
            {
                cumulativeCapital += closedPositions[i].getProfitIngoreAmount();
                capital[i] = cumulativeCapital;
            }

            return ArrayVisualizer.visualizeArray(capital, width, heigth, 5);
        }
    }
}
