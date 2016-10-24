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
                inputs[i] = new bool[] { true, true };
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
        public void getSampleCodesBuyMaxSellMax_Test()
        {
            double[][] samples = new double[10][];
            for(int i = 0; i < samples.Length; i++)
            {
                samples[i] = new double[] { i * 0.1, 0.2, 0.2, 10 };
            }

            samples[3] = null;
            samples[4][1] = 0.4; //Buy
            samples[5][2] = 0.41; //Sell

            double buy, sell;
            DistributionHelper.getSampleOutcomeCodesBuyMaxSellMax(samples, out buy, out sell);
            Assert.AreEqual(0.4, buy);
            Assert.AreEqual(0.41, sell);
        }

        [TestMethod]
        public void getOutcomeMinMax_Test()
        {
            double[][] samples = new double[10][];
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = new double[] { i * 0.1, -0.2, 0.2, 0.3, 10 };
            }

            samples[3] = null;
            samples[4][1] = -0.4; //MinMIn
            samples[5][2] = 0.41; //MaxMax

            samples[6][3] = 0.42; //MaxActual
            samples[5][3] = -0.2; //MinActual


            double maxMax, minMin, maxActual, minActual;
            DistributionHelper.getSampleOutcomesMinMax(samples, out maxMax, out minMin, out maxActual, out minActual);

            Assert.AreEqual(0.41, maxMax);
            Assert.AreEqual(-0.4, minMin);
            Assert.AreEqual(0.42, maxActual);
            Assert.AreEqual(-0.2, minActual);
        }
    }
}
