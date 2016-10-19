using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;

namespace V3_Trader_Project.Trader
{
    public static class DistributionHelper
    {
        public static void getOutcomeCodeDistribution(bool[][] outcomeCodes, out double buyRatio, out double sellRatio)
        {
            int buys = 0, sells = 0;
            foreach (bool[] row in outcomeCodes)
            {
                buys += row[(int)OutcomeCodeMatrixIndices.Buy] ? 1 : 0;
                sells += row[(int)OutcomeCodeMatrixIndices.Sell] ? 1 : 0;
            }

            double length = outcomeCodes.Length;

            buyRatio = buys / length;
            sellRatio = sells / length;
        }

        public static void getMinMax(double[] input, int percentToDrop, out double min, out double max)
        {
            max = input.Percentile(100 - percentToDrop / 2);
            min = input.Percentile(percentToDrop / 2);
        }
    }
}
