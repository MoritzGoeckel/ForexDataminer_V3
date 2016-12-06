using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V3_Trader_Project.Trader.Market;

namespace V3_Trader_Project.Trader
{
    public static class OrderHistoryStreakAnalysis
    {
        public static string getLossWinStreakString(List<ClosedPosition> closedPositions)
        {
            StringBuilder output = new StringBuilder();
            int streak = 0;
            bool win = true;
            foreach (ClosedPosition c in closedPositions)
            {
                if (c.getProfit() > 0 == win)
                    streak++;
                else
                {
                    output.Append(streak + ":");
                    streak = 1;
                    win = c.getProfit() > 0;
                }
            }

            return output.ToString();
        }

        public static double getWinRateLastTrades(int trades, List<ClosedPosition> _closedPositions)
        {
            int profitable = 0;
            int regarded = 0;

            for (int i = _closedPositions.Count - 1; i > 0 && i > _closedPositions.Count - 1 - trades; i--)
            {
                if (_closedPositions[i].getProfit() > 0)
                    profitable++;
                regarded++;
            }

            if (regarded != 0)
                return Convert.ToDouble(profitable) / Convert.ToDouble(regarded);
            else
                return 1;
        }

        public class WinLossStreak { public bool win; public int streak; public WinLossStreak(bool win, int streak) { this.win = win; this.streak = streak; } }

        public static WinLossStreak getLastStreak(List<ClosedPosition> closedPositions)
        {
            WinLossStreak currentStreak = new WinLossStreak(closedPositions[closedPositions.Count - 1].getProfit() >= 0, 0);
            for (int i = closedPositions.Count - 1; i > 0; i--)
            {
                bool win = closedPositions[i].getProfit() > 0;
                if (currentStreak.win == win)
                    currentStreak.streak++;
                else
                    break;
            }

            return currentStreak;
        }
    }
}
