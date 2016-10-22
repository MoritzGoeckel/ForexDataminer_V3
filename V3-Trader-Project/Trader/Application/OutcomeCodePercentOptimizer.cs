using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Application
{
    public static class OutcomeCodePercentOptimizer
    {
        public static double findOutcomeCodeForDesiredDistribution(double desiredDistribution, double tolerance, double[][] priceData, double[][] outcomeMatrix, out double buyDist, out double sellDist)
        {
            double outcomeCodePercent = 0.5;
            int round = 0;
            while (true)
            {
                double successRatioCode;
                bool[][] outcomeCodes = OutcomeGenerator.getOutcomeCode(priceData, outcomeMatrix, outcomeCodePercent, out successRatioCode);

                if (successRatioCode < 0.9)
                    throw new TooLittleValidDataException("Too few outcome codes: " + successRatioCode);

                DistributionHelper.getOutcomeCodeDistribution(outcomeCodes, out buyDist, out sellDist);

                double score = (buyDist + sellDist) / 2;
                if (score > desiredDistribution - tolerance && score < desiredDistribution + tolerance)
                    break;
                else if (score > desiredDistribution + tolerance)
                    outcomeCodePercent += (outcomeCodePercent / (10 + round));
                else if (score < desiredDistribution - tolerance)
                    outcomeCodePercent -= (outcomeCodePercent / (10 + round));

                Logger.log("SetDist OPT. Round " + round + " -> " + outcomeCodePercent + "% = b" + Math.Round(buyDist, 4) + " s" + Math.Round(sellDist, 4) + " =" + Math.Round(score, 4));

                round++;
            }

            return outcomeCodePercent;
        }

        public static double optimizeOutcomeCodePercentage(int rounds, out double outcomeCodePercent, double[][] priceData, double[][] outcomes, out double buyDist, out double sellDist)
        {
            outcomeCodePercent = 0.5;

            buyDist = double.NaN;
            sellDist = double.NaN;

            double lastScore = double.MinValue;
            double direction = -0.01;

            int round;
            for (round = 1; round < rounds; round++)
            {
                double successRatioCode;
                bool[][] outcomeCodes = OutcomeGenerator.getOutcomeCode(priceData, outcomes, outcomeCodePercent, out successRatioCode);

                if (successRatioCode < 0.9)
                    throw new TooLittleValidDataException("Too low success ratio: " + successRatioCode);

                DistributionHelper.getOutcomeCodeDistribution(outcomeCodes, out buyDist, out sellDist);

                double score = ((buyDist + sellDist) / 2) * outcomeCodePercent;
                if (score < lastScore) //Wrong direction
                {
                    direction = direction * (-1);
                    Logger.log("New opt. direction: " + direction);
                }

                if (outcomeCodePercent <= 0 && direction <= 0)
                    direction = Math.Abs(direction);

                outcomeCodePercent += (direction / (1 + (round / 20)));

                Logger.log("PercDist OPT. Round " + round + " -> " + outcomeCodePercent + "% = |s" + Math.Round(score, 4) + "| b" + Math.Round(buyDist, 4) + " s" + Math.Round(sellDist, 4));

                lastScore = score;
                round++;
            }

            return outcomeCodePercent;
        }
    }
}
