using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V3_Trader_Project.Trader.Visualizers;

namespace V3_Trader_Project.Trader.Tests
{
    [TestClass]
    public class OutcomeSamplingVisualizer_Test
    {
        [TestMethod]
        public void VisualizeOutcomeCodeSampling_Test()
        {
            double[] values = new double[300];
            bool[][] outcomeCodes = new bool[300][];
            Random z = new Random();
            for (int i = 0; i < values.Length; i++)
            {
                double d = z.NextDouble();
                if (d > 0.7)
                    outcomeCodes[i] = new bool[] { z.NextDouble() > 0.2 , z.NextDouble() > 0.1 };
                else
                    outcomeCodes[i] = new bool[] { z.NextDouble() > 0.6, z.NextDouble() > 0.7 };

                values[i] = d;
            }

            outcomeCodes[50] = null; 

            double validRatio;
            double[][] samples = IndicatorSampler.sampleValuesOutcomeCode(values, outcomeCodes, 0, 1, 10, out validRatio);

            samples[3] = null;

            Image img = OutcomeSamplingVisualizer.visualizeOutcomeCodeSamplingTable(samples, 500, 300, 0.3d);
            ArrayVisualizer.showImg(img);
        }
    }
}
