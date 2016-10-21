using MathNet.Numerics.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Tests
{
    [TestClass]
    public class DistributionHelper_Test
    {
        [TestMethod]
        public void getOutcomeCodeDistribution_Test()
        {
            bool[][] inputs = new bool[100][];
            for (int i = 0; i < 50; i++)
            {
                inputs[i] = new bool[]{ true, true };
            }

            for (int i = 50; i < 100; i++)
            {
                inputs[i] = new bool[] { false, true };
            }

            for (int i = 50; i < 75; i++)
            {
                inputs[i][(int)OutcomeCodeMatrixIndices.Sell] = false;
            }

            double buyR, sellR;
            DistributionHelper.getOutcomeCodeDistribution(inputs, out buyR, out sellR);
            Assert.AreEqual(0.5, buyR);
            Assert.AreEqual(0.75, sellR);
        }

        [TestMethod]
        public void getMinMax_Test()
        {
            double[] inputs = new double[100];
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = i;
            }

            double min, max;
            DistributionHelper.getMinMax(inputs, 10, out min, out max);

            Assert.AreEqual(5, min, 1);
            Assert.AreEqual(95, max, 1);
        }

        [TestMethod]
        public void getMinMax_NAN_Test()
        {
            double[] inputs = new double[200];
            int v = 0;
            for (int i = 0; i < inputs.Length; i += 2)
            {
                inputs[i] = v++;
            }

            for (int i = 1; i < inputs.Length; i += 2)
            {
                inputs[i] = double.NaN;
            }

            Assert.AreEqual(inputs[0], 0);
            Assert.AreEqual(inputs[2], 1);
            Assert.AreEqual(inputs[4], 2);
            Assert.AreEqual(inputs[6], 3);

            Assert.AreEqual(inputs[1], double.NaN);
            Assert.AreEqual(inputs[3], double.NaN);
            Assert.AreEqual(inputs[5], double.NaN);
            Assert.AreEqual(inputs[7], double.NaN);

            double min, max;
            DistributionHelper.getMinMax(inputs, 10, out min, out max);

            Assert.AreEqual(5, min, 1);
            Assert.AreEqual(95, max, 1);
        }

        [TestMethod]
        public void getMinMax_Test_SmallValues()
        {
            double[] inputs = new double[100];
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = i * 0.001;
            }

            double min, max;
            DistributionHelper.getMinMax(inputs, 10, out min, out max);

            Assert.AreEqual(5 * 0.001, min, 1 * 0.001);
            Assert.AreEqual(95 * 0.001, max, 1 * 0.001);
        }

        [TestMethod]
        public void getMinMax_Test_MoreComplex()
        {
            double[] inputs = new double[200];
            for (int i = 0; i < 100; i++)
            {
                inputs[i] = i;
            }

            for (int i = 100; i < 200; i++)
            {
                inputs[i] = 100;
            }

            double min, max;
            DistributionHelper.getMinMax(inputs, 10, out min, out max);
            Assert.AreEqual(10, min, 1);
            Assert.AreEqual(100, max, 1);
        }

        [TestMethod]
        public void getSampleCodesMinMax_Test()
        {
            Assert.Fail();
        }
    }
}
