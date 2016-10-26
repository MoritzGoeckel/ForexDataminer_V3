using NinjaTrader_Client.Trader.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader
{
    public class IndicatorGenerator
    {
        private Dictionary<string, bool> doneIndicators = new Dictionary<string, bool>();
        private Random z = new Random();
        public WalkerIndicator getRandomIndicator(int minTimeFrameSeconds, int maxTimeframeSeconds)
        {
            //Todo: Optimze the generator, make it smarter so less indicators fail
            WalkerIndicator theIndicator = null;

            long timeframeOne = z.Next(minTimeFrameSeconds, maxTimeframeSeconds) * 1000l;
            long timeframeTwo = z.Next(minTimeFrameSeconds, maxTimeframeSeconds) * 1000l;
            long timeFrameThree = z.Next(minTimeFrameSeconds, maxTimeframeSeconds) * 1000l;
            long timeFrameSmaller = z.Next(minTimeFrameSeconds, maxTimeframeSeconds / 2) * 1000l;

            switch (z.Next(0, 18)) //Todo: set max value for choosing the indicator
            {
                case 0:
                    theIndicator = new BolingerBandsIndicator(timeframeOne, getRanDouble(0.5d, 5d));
                    break;

                case 1:
                    theIndicator = new MACDContinousIndicator(timeframeOne, timeframeTwo, timeFrameSmaller);
                    break;

                case 2:
                    theIndicator = new MACDIndicator(timeframeOne, timeframeTwo, timeFrameSmaller);
                    break;

                case 3:
                    theIndicator = new MovingAveragePriceSubtractionIndicator(timeframeOne);
                    break;

                case 4:
                    theIndicator = new MovingAverageSubtractionCrossoverIndicator(timeframeOne, timeframeTwo);
                    break;

                case 5:
                    theIndicator = new MovingAverageSubtractionIndicator(timeframeOne, timeframeTwo);
                    break;

                case 6:
                    theIndicator = new RangeIndicator(timeframeOne);
                    break;

                case 7:
                    theIndicator = new RSIBorderCrossoverIndicator(timeframeOne, getRanDouble(0.1, 0.4));
                    break;

                case 8:
                    theIndicator = new RSIBorderIndicator(timeframeOne, getRanDouble(0.1, 0.4));
                    break;

                case 9:
                    theIndicator = new RSIIndicator(timeframeOne);
                    break;

                case 10:
                    theIndicator = new RSIMACrossoverContinousIndicator(timeframeOne, timeFrameSmaller);
                    break;

                case 11:
                    theIndicator = new RSIMACrossoverIndicator(timeframeOne, timeFrameSmaller);
                    break;

                case 12:
                    theIndicator = new StandartDeviationIndicator(timeframeOne);
                    break;

                case 13:
                    theIndicator = new StochBorderCrossoverIndicator(timeframeOne, getRanDouble(0.1, 0.4));
                    break;

                case 14:
                    theIndicator = new StochBorderIndicator(timeframeOne, getRanDouble(0.1, 0.4));
                    break;

                case 15:
                    theIndicator = new StochIndicator(timeframeOne);
                    break;

                case 16:
                    theIndicator = new VolumeAtPriceIndicator(timeframeOne, getRanDouble(0.0003, 0.002), z.Next(1000 * 30, 1000 * 60 * 10));
                    break; //Not sure about stepsize todo

                case 17:
                    theIndicator = new TimeOfDayIndicator(); //Only once?
                    break;

                case 18:
                    theIndicator = new TimeDayOfWeekIndicator(); //Todo: Only once?
                    break;

                default:
                    throw new Exception("Fired a unexpected random value");
            }

            if (doneIndicators.ContainsKey(theIndicator.getName()) == false)
            {
                doneIndicators.Add(theIndicator.getName(), true);
                return theIndicator;
            }
            else
                return getRandomIndicator(minTimeFrameSeconds, maxTimeframeSeconds);
        }

        private double getRanDouble(double min, double max)
        {
            return min + z.NextDouble() * (max - min); 
        }

        public WalkerIndicator getIndicatorByString(string input)
        {
            string[] args = input.Split('_');

            if (args[0] == BolingerBandsIndicator.Name)
                return new BolingerBandsIndicator(long.Parse(args[1]), double.Parse(args[2].Replace(',', '.')));

            if (args[0] == MACDContinousIndicator.Name)
                return new MACDContinousIndicator(long.Parse(args[1]), long.Parse(args[2]), long.Parse(args[3]));

            if (args[0] == MACDIndicator.Name)
                return new MACDIndicator(long.Parse(args[1]), long.Parse(args[2]), long.Parse(args[3]));

            if (args[0] == MovingAverageIndicator.Name)
                return new MovingAverageIndicator(long.Parse(args[1]));

            if (args[0] == MovingAveragePriceSubtractionIndicator.Name)
                return new MovingAveragePriceSubtractionIndicator(long.Parse(args[1]));

            if (args[0] == MovingAverageSubtractionIndicator.Name)
                return new MovingAverageSubtractionIndicator(long.Parse(args[1]), long.Parse(args[2]));

            if (args[0] == MovingAverageSubtractionCrossoverIndicator.Name)
                return new MovingAverageSubtractionCrossoverIndicator(long.Parse(args[1]), long.Parse(args[2]));

            if (args[0] == RangeIndicator.Name)
                return new RangeIndicator(long.Parse(args[1]));

            if (args[0] == RSIBorderCrossoverIndicator.Name)
                return new RSIBorderCrossoverIndicator(long.Parse(args[1]), double.Parse(args[2].Replace(',', '.')));

            if (args[0] == RSIBorderIndicator.Name)
                return new RSIBorderIndicator(long.Parse(args[1]), double.Parse(args[2].Replace(',', '.')));

            if (args[0] == RSIIndicator.Name)
                return new RSIIndicator(long.Parse(args[1]));

            if (args[0] == RSIMACrossoverContinousIndicator.Name)
                return new RSIMACrossoverContinousIndicator(long.Parse(args[1]), long.Parse(args[2]));

            if (args[0] == RSIMACrossoverIndicator.Name)
                return new RSIMACrossoverIndicator(long.Parse(args[1]), long.Parse(args[2]));

            if (args[0] == StandartDeviationIndicator.Name)
                return new StandartDeviationIndicator(long.Parse(args[1]));

            if (args[0] == StochBorderCrossoverIndicator.Name)
                return new StochBorderCrossoverIndicator(long.Parse(args[1]), double.Parse(args[2].Replace(',', '.')));

            if (args[0] == StochBorderIndicator.Name)
                return new StochBorderIndicator(long.Parse(args[1]), double.Parse(args[2].Replace(',', '.')));

            if (args[0] == StochIndicator.Name)
                return new StochIndicator(long.Parse(args[1]));

            if (args[0] == TestIndicator.Name)
                return new TestIndicator();

            if (args[0] == TimeDayOfWeekIndicator.Name)
                return new TimeDayOfWeekIndicator();

            if (args[0] == TimeOfDayIndicator.Name)
                return new TimeOfDayIndicator();

            if (args[0] == TimeOpeningHoursIndicator.Name)
                return new TimeOpeningHoursIndicator();

            if (args[0] == VolumeAtPriceIndicator.Name)
                return new VolumeAtPriceIndicator(long.Parse(args[1]), double.Parse(args[2].Replace(',', '.')), long.Parse(args[3]));

            throw new Exception("Name not found: " + args[0]);
        }
    }
}
