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
        Buy = 0, Sell = 0
    };

    public class OutcomeGenerator
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
                
                while (futureElement < dataInput.Length && dataInput[futureElement][(int)DataIndeces.Date] - dataInput[currentElement][(int)DataIndeces.Date] < msTimeframe)
                {
                    elementsInTimeframe.Add(dataInput[futureElement]);

                    double mid = (dataInput[futureElement][(int)DataIndeces.Ask] + dataInput[futureElement][(int)DataIndeces.Bid]) / 2d;
                    if (mid < min)
                        min = mid;

                    if (mid > max)
                        max = mid;

                    actual = mid;

                    futureElement++;
                }

                bool invalidated = false;
                while(elementsInTimeframe[0][(int)DataIndeces.Date] <= dataInput[currentElement][(int)DataIndeces.Date])
                {
                    double mid = (elementsInTimeframe[0][(int)DataIndeces.Ask] + elementsInTimeframe[0][(int)DataIndeces.Bid]) / 2d;
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
                        double mid = (row[(int)DataIndeces.Ask] + row[(int)DataIndeces.Bid]) / 2d;
                        if (mid < min)
                            min = mid;

                        if (mid > max)
                            max = mid;
                    }
                }

                if (min == double.MaxValue || max == double.MinValue || actual == double.NaN)
                    throw new Exception("Invalid value!");

                output[currentElement] = new double[]{ min, max, actual };
            }

            return output;
        }

        public bool[][] getOutcomeCode(double[][] dataInput, double[][] outcomeInput, double percent)
        {
            bool[][] output = new bool[dataInput.Length][];
            for(int i = 0; i < dataInput.Length; i++)
            {
                double mid = dataInput[i][(int)DataIndeces.Ask] + dataInput[i][(int)DataIndeces.Bid] / 2;
                double gain = ((outcomeInput[i][(int)OutcomeMatrixIndices.Max] / mid) - 1) * 100;
                double fall = ((outcomeInput[i][(int)OutcomeMatrixIndices.Min] / mid) - 1) * 100;

                output[i] = new bool[] { gain >= percent, fall <= -percent};
            }

            return output;
        }
    }
}
