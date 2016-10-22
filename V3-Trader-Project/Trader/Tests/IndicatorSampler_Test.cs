using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Tests
{
    [TestClass]
    public class IndicatorSampler_Test
    {
        [TestMethod]
        public void indicatorSamper_getStatistics_Test_BuyCorrealation()
        {
            double[] values = new double[100];
            bool[][] outcomeCodes = new bool[100][];
            Random z = new Random();
            for(int i = 0; i < values.Length; i++)
            {
                double d = z.NextDouble();
                if(d > 0.7)
                    outcomeCodes[i] = new bool[]{ true, z.NextDouble() > 0.5 };
                else
                    outcomeCodes[i] = new bool[] { false, z.NextDouble() > 0.5 };

                values[i] = d;
            }

            double spBuy, spSell, pBuy, pSell;
            IndicatorSampler.getStatisticsOutcomeCodes(values, outcomeCodes, out spBuy, out spSell, out pBuy, out pSell);

            Assert.IsTrue(spBuy > spSell - 0.2);
            Assert.IsTrue(pBuy > pSell - 0.2);
        }

        [TestMethod]
        public void indicatorSamper_getStatistics_Test_SellCorrealation()
        {
            double[] values = new double[100];
            bool[][] outcomeCodes = new bool[100][];
            Random z = new Random();
            for (int i = 0; i < values.Length; i++)
            {
                double d = z.NextDouble();
                if (d > 0.7)
                    outcomeCodes[i] = new bool[] { z.NextDouble() > 0.5, true };
                else
                    outcomeCodes[i] = new bool[] { z.NextDouble() > 0.5, z.NextDouble() > 0.5 };

                values[i] = d;
            }

            double spBuy, spSell, pBuy, pSell;
            IndicatorSampler.getStatisticsOutcomeCodes(values, outcomeCodes, out spBuy, out spSell, out pBuy, out pSell);

            Assert.IsTrue(spSell > spBuy - 0.2);
            Assert.IsTrue(pSell > pBuy - 0.2);
        }

        [TestMethod]
        public void sampleValuesOutcomeCode_Test()
        {
            double[] values = new double[100];
            bool[][] outcomeCodes = new bool[100][];
            Random z = new Random();
            for (int i = 0; i < values.Length; i++)
            {
                double d = z.NextDouble();
                if (d > 0.7)
                    outcomeCodes[i] = new bool[] { true, true };
                else
                    outcomeCodes[i] = new bool[] { true, false };

                values[i] = d;
            }

            double validRatio;
            double[][] samples = IndicatorSampler.sampleValuesOutcomeCode(values, outcomeCodes, 0, 1, 10, out validRatio);
            Assert.AreEqual(1, validRatio);

            for(int i = 0; i < 10; i++)
            {
                if (samples[i] != null)
                {
                    Assert.AreEqual(1d, samples[i][((int)SampleValuesOutcomeCodesIndices.BuyRatio)]);
                    Assert.AreEqual(0.1d * i, samples[i][((int)SampleValuesOutcomeCodesIndices.Start)], 0.0001);

                    if (i < 7)
                        Assert.AreEqual(0d, samples[i][((int)SampleValuesOutcomeCodesIndices.SellRatio)]);
                    else
                        Assert.AreEqual(1d, samples[i][((int)SampleValuesOutcomeCodesIndices.SellRatio)]);
                }
            }
        }

        [TestMethod]
        public void sampleValuesOutcome_Test()
        {
            double[] values = new double[100];
            double[][] outcomes = new double[100][];
            double[][] prices = new double[100][];
            DateTime dt = DateTime.Now.ToUniversalTime();

            Random z = new Random();
            for (int i = 0; i < values.Length; i++)
            {
                //Min max actual
                double d = z.NextDouble();
                if (d > 0.7)
                    outcomes[i] = new double[] { 0, 2, 3 };
                else
                    outcomes[i] = new double[] { 0.8, 1.2, 4 };

                values[i] = d;

                dt = dt.AddMilliseconds(1000);
                prices[i] = new double[] { Timestamp.dateTimeToMilliseconds(dt), 1, 1, 0 };
            }

            double validOut;
            double[][] samples = IndicatorSampler.sampleValuesOutcome(values, prices, outcomes, 0, 1, out validOut, 10);

            Assert.AreEqual(1d, validOut);

            for (int i = 0; i < 10; i++)
            {
                if (samples[i] != null)
                {
                    Assert.AreEqual(0.1d * i, samples[i][((int)SampleValuesOutcomeIndices.Start)], 0.0001);

                    if (i >= 7)
                    {
                        Assert.AreEqual(100d, samples[i][((int)SampleValuesOutcomeIndices.MaxAvg)], 0.01);
                        Assert.AreEqual(-100d, samples[i][((int)SampleValuesOutcomeIndices.MinAvg)], 0.01);
                        Assert.AreEqual(200d, samples[i][((int)SampleValuesOutcomeIndices.ActualAvg)], 0.01);
                    }
                    else
                    {
                        Assert.AreEqual(20d, samples[i][((int)SampleValuesOutcomeIndices.MaxAvg)], 0.01);
                        Assert.AreEqual(-20d, samples[i][((int)SampleValuesOutcomeIndices.MinAvg)], 0.01);
                        Assert.AreEqual(300d, samples[i][((int)SampleValuesOutcomeIndices.ActualAvg)], 0.01);
                    }
                }
            }
        }
    }
}
