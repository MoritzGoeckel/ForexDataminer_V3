using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader
{
    public class ArrayHelper
    {
        public static double[][] combineArraysHorizontal(double[] a, double[][] inputArrays)
        {
            double[][] output = new double[a.Length][];
            for(int i = 0; i < output.Length; i++)
            {
                output[i] = new double[1 + inputArrays.Length];
                output[i][0] = a[i];
                for (int b = 0; b < inputArrays.Length; b++)
                    output[i][b + 1] = inputArrays[b][i];
            }

            return output;
        }
    }
}
