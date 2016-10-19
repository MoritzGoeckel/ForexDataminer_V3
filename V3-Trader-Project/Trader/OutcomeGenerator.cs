using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader
{
    public enum OutcomeMatrixIndices
    {
        Min = 0, Max = 1, Actual = 2
    };

    public enum OutcomeCodeMatrixIndices
    {
        Buy = 0, Sell = 1
    };

    public static class OutcomeGenerator
    {
        public static double[][] getOutcome(double[][] dataInput, long msTimeframe)
        {
            double[][] output = new double[dataInput.Length][];
            int futureElement = 0;
            double min = double.MaxValue, max = double.MinValue, actual = double.NaN;
            List<double[]> elementsInTimeframe = new List<double[]>();

            for(int currentElement = 0; currentElement < dataInput.Length; currentElement++)
            {
                double[] outputLine = new double[3];
                
                while (futureElement < dataInput.Length && dataInput[futureElement][(int)PriceDataIndeces.Date] - dataInput[currentElement][(int)PriceDataIndeces.Date] < msTimeframe)
                {
                    elementsInTimeframe.Add(dataInput[futureElement]);

                    double mid = (dataInput[futureElement][(int)PriceDataIndeces.Ask] + dataInput[futureElement][(int)PriceDataIndeces.Bid]) / 2d;
                    if (mid < min)
                        min = mid;

                    if (mid > max)
                        max = mid;

                    actual = mid;

                    futureElement++;
                }

                bool invalidated = false;
                while(elementsInTimeframe.Count > 0 && elementsInTimeframe[0][(int)PriceDataIndeces.Date] <= dataInput[currentElement][(int)PriceDataIndeces.Date])
                {
                    double mid = (elementsInTimeframe[0][(int)PriceDataIndeces.Ask] + elementsInTimeframe[0][(int)PriceDataIndeces.Bid]) / 2d;
                    if (mid == min || mid == max)
                        invalidated = true;

                    elementsInTimeframe.RemoveAt(0);
                }

                if(invalidated)
                {
                    min = double.MaxValue;
                    max = double.MinValue;

                    foreach(double[] row in elementsInTimeframe)
                    {
                        double mid = (row[(int)PriceDataIndeces.Ask] + row[(int)PriceDataIndeces.Bid]) / 2d;
                        if (mid < min)
                            min = mid;

                        if (mid > max)
                            max = mid;
                    }
                }

                if (futureElement != dataInput.Length && (min == double.MaxValue || max == double.MinValue || double.IsNaN(actual)))
                    throw new Exception("Invalid value!");

                if(futureElement != dataInput.Length)
                    output[currentElement] = new double[]{ min, max, actual };
                else
                    output[currentElement] = new double[] { double.NaN, double.NaN, double.NaN };
            }

            return output;
        }

        public static bool[][] getOutcomeCode(double[][] pricesInput, double[][] outcomeInput, double percent)
        {
            if (pricesInput.Length != outcomeInput.Length)
                throw new Exception("Arrays have to be the same size: " + pricesInput.Length + " != " + outcomeInput.Length);

            bool[][] output = new bool[pricesInput.Length][];
            for(int i = 0; i < pricesInput.Length; i++)
            {
                double mid = pricesInput[i][(int)PriceDataIndeces.Ask] + pricesInput[i][(int)PriceDataIndeces.Bid] / 2;
                double gain = ((outcomeInput[i][(int)OutcomeMatrixIndices.Max] / mid) - 1) * 100;
                double fall = ((outcomeInput[i][(int)OutcomeMatrixIndices.Min] / mid) - 1) * 100;

                output[i] = new bool[] { gain >= percent, fall <= -percent};
            }

            return output;
        }
    }
}
