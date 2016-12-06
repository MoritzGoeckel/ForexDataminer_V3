using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static V3_Trader_Project.Trader.OutcomeGenerator;

namespace V3_Trader_Project.Trader.Tests
{
    [TestClass]
    public class OutcomeGenerator_Test
    {
        [TestMethod]
        public void getOutcome_Test_OnlyOneValue()
        {
            //Generate a input
            double[][] inputs = new double[100][];
            DateTime dt = DateTime.Now.ToUniversalTime();
            for(int i = 0; i < inputs.Length; i++)
            {
                //Date bid ask volume
                dt = dt.AddMilliseconds(1000);
                inputs[i] = new double[] { Timestamp.dateTimeToMilliseconds(dt), 2, 2, 0 };
            }

            double successRate;
            double[][] outcomes = OutcomeGenerator.getOutcome(inputs, 1000 * 10, out successRate);

            int foundNaNs = 0;
            foreach(double[] row in outcomes)
            {
                if (row != null)
                {
                    Assert.AreEqual(2, row[(int)OutcomeMatrixIndices.Actual]);
                    Assert.AreEqual(2, row[(int)OutcomeMatrixIndices.Min]);
                    Assert.AreEqual(2, row[(int)OutcomeMatrixIndices.Max]);
                }
                else
                    foundNaNs++;
            }

            Assert.AreEqual(10, foundNaNs);
            Assert.AreEqual(null, outcomes[outcomes.Length - 1]);
        }

        [TestMethod]
        public void getOutcome_Test_MoreComplex()
        {
            //Generate a input
            double[][] inputs = new double[100][];
            DateTime dt = DateTime.Now.ToUniversalTime();
            for (int i = 0; i < inputs.Length; i++)
            {
                //Date bid ask volume
                dt = dt.AddMilliseconds(1000);
                inputs[i] = new double[] { Timestamp.dateTimeToMilliseconds(dt), i, i, 0 };
            }

            double successRate;
            double[][] outcomes = OutcomeGenerator.getOutcome(inputs, 1000 * 10, out successRate);

            int foundNaNs = 0;
            for (int i = 0; i < outcomes.Length; i++)
            {
                if (outcomes[i] != null)
                {
                    Assert.AreEqual(i + 9, outcomes[i][(int)OutcomeMatrixIndices.Actual]);
                    Assert.AreEqual(i + 1, outcomes[i][(int)OutcomeMatrixIndices.Min]);
                    Assert.AreEqual(i + 9, outcomes[i][(int)OutcomeMatrixIndices.Max]);
                }
                else
                    foundNaNs++;
            }

            Assert.AreEqual(10, foundNaNs);
            Assert.AreEqual(null, outcomes[outcomes.Length - 1]);
        }

        [TestMethod]
        public void getOutcome_Test_MoreComplex_Negative()
        {
            //Generate a input
            double[][] inputs = new double[100][];
            DateTime dt = DateTime.Now.ToUniversalTime();
            for (int i = 0; i < inputs.Length; i++)
            {
                //Date bid ask volume
                dt = dt.AddMilliseconds(1000);
                inputs[i] = new double[] { Timestamp.dateTimeToMilliseconds(dt), inputs.Length - i, inputs.Length - i, 0 };
            }

            double successRate;
            double[][] outcomes = OutcomeGenerator.getOutcome(inputs, 1000 * 10, out successRate);

            int foundNaNs = 0;
            for (int i = 0; i < outcomes.Length; i++)
            {
                if (outcomes[i] != null)
                {
                    Assert.AreEqual(inputs.Length - (i + 9), outcomes[i][(int)OutcomeMatrixIndices.Actual]);
                    Assert.AreEqual(inputs.Length - (i + 9), outcomes[i][(int)OutcomeMatrixIndices.Min]);
                    Assert.AreEqual(inputs.Length - (i + 1), outcomes[i][(int)OutcomeMatrixIndices.Max]);
                }
                else
                    foundNaNs++;
            }

            Assert.AreEqual(10, foundNaNs);
            Assert.AreEqual(null, outcomes[outcomes.Length - 1]);
        }

        [TestMethod]
        public void getOutcomeCode_Test()
        {
            double[][] inputs = new double[100][];
            DateTime dt = DateTime.Now.ToUniversalTime();
            for (int i = 0; i < inputs.Length; i++)
            {
                //Date bid ask volume
                dt = dt.AddMilliseconds(1000);
                inputs[i] = new double[] { Timestamp.dateTimeToMilliseconds(dt), 2, 2, 0 };
            }

            double[][] outcomes = new double[100][];
            for (int i = 0; i < outcomes.Length; i++)
            {
                //Min Max Actual
                outcomes[i] = new double[] { 0, 4, 3 };
            }

            double successRatio;
            bool[][] outcomeCodes = OutcomeGenerator.getOutcomeCode(inputs, outcomes, 100, out successRatio);

            int notAssignedCount = 0;
            foreach (bool[] row in outcomeCodes)
            {
                if (row != null)
                {
                    Assert.IsTrue(row[(int)OutcomeCodeMatrixIndices.Buy]);
                    Assert.IsTrue(row[(int)OutcomeCodeMatrixIndices.Sell]);
                }
                else
                    notAssignedCount++;
            }
            Assert.AreEqual(0, notAssignedCount);
        }

        [TestMethod]
        public void getOutcomeCodeFirst_Negative_Test()
        {
            double[][] inputs = new double[100][];
            DateTime dt = DateTime.Now.ToUniversalTime();
            for (int i = 0; i < inputs.Length; i++)
            {
                //Date bid ask volume
                dt = dt.AddMilliseconds(1000);
                inputs[i] = new double[] { Timestamp.dateTimeToMilliseconds(dt), 2, 2, 0 };
            }
            
            double successRatio;
            bool[][] outcomeCodes = OutcomeGenerator.getOutcomeCodeFirst(inputs, 1000 * 10, 100, out successRatio);

            int notAssignedCount = 0;
            foreach (bool[] row in outcomeCodes)
            {
                if (row != null)
                {
                    Assert.IsFalse(row[(int)OutcomeCodeMatrixIndices.Buy]);
                    Assert.IsFalse(row[(int)OutcomeCodeMatrixIndices.Sell]);
                }
                else
                    notAssignedCount++;
            }
            Assert.AreEqual(10, notAssignedCount);
        }

        [TestMethod]
        public void getOutcomeCodeFirst_Positive_Test()
        {
            double[][] inputs = new double[100][];
            DateTime dt = DateTime.Now.ToUniversalTime();
            for (int i = 0; i < inputs.Length; i++)
            {
                //Date bid ask volume
                dt = dt.AddMilliseconds(1000);
                inputs[i] = new double[] { Timestamp.dateTimeToMilliseconds(dt), 2, 2, 0 };
            }

            inputs[50][1] = inputs[50][2] = 5;

            double successRatio;
            bool[][] outcomeCodes = OutcomeGenerator.getOutcomeCodeFirst(inputs, 1000 * 10, 100, out successRatio);

            int notAssignedCount = 0;
            for (int i = 0; i < outcomeCodes.Length; i++)
            {
                if (outcomeCodes[i] != null)
                {
                    if(i > 40 && i < 50)
                        Assert.IsTrue(outcomeCodes[i][(int)OutcomeCodeMatrixIndices.Buy], i + " Buy: " + outcomeCodes[i][(int)OutcomeCodeMatrixIndices.Buy]);
                    else
                        Assert.IsFalse(outcomeCodes[i][(int)OutcomeCodeMatrixIndices.Buy], i + " Buy: " + outcomeCodes[i][(int)OutcomeCodeMatrixIndices.Buy]);

                    Assert.IsFalse(outcomeCodes[i][(int)OutcomeCodeMatrixIndices.Sell], i + " Sell: " + outcomeCodes[i][(int)OutcomeCodeMatrixIndices.Sell]);
                }
                else
                    notAssignedCount++;
            }
            Assert.AreEqual(10, notAssignedCount);
        }

        [TestMethod]
        public void getOutcomeCode_Test_Negative()
        {
            double[][] inputs = new double[100][];
            DateTime dt = DateTime.Now.ToUniversalTime();
            for (int i = 0; i < inputs.Length; i++)
            {
                //Date bid ask volume
                dt = dt.AddMilliseconds(1000);
                inputs[i] = new double[] { Timestamp.dateTimeToMilliseconds(dt), 2, 2, 0 };
            }

            double[][] outcomes = new double[100][];
            for (int i = 0; i < outcomes.Length; i++)
            {
                //Min Max Actual
                outcomes[i] = new double[] { 1, 4, 3 };
            }

            double successRatio;
            bool[][] outcomeCodes = OutcomeGenerator.getOutcomeCode(inputs, outcomes, 100, out successRatio);

            int notAssignedCount = 0;
            foreach (bool[] row in outcomeCodes)
            {
                if (row != null)
                {
                    Assert.IsTrue(row[(int)OutcomeCodeMatrixIndices.Buy]);
                    Assert.IsFalse(row[(int)OutcomeCodeMatrixIndices.Sell]);
                }
                else
                    notAssignedCount++;
            }
            Assert.AreEqual(0, notAssignedCount);
        }

        [TestMethod]
        public void getOutcomeCode_Test_NegativePercent()
        {
            double[][] inputs = new double[100][];
            DateTime dt = DateTime.Now.ToUniversalTime();
            for (int i = 0; i < inputs.Length; i++)
            {
                //Date bid ask volume
                dt = dt.AddMilliseconds(1000);
                inputs[i] = new double[] { Timestamp.dateTimeToMilliseconds(dt), 2, 2, 0 };
            }

            double[][] outcomes = new double[100][];
            for (int i = 0; i < outcomes.Length; i++)
            {
                //Min Max Actual
                outcomes[i] = new double[] { 2, 4, 3 };
            }

            double successRatio;
            bool[][] outcomeCodes = OutcomeGenerator.getOutcomeCode(inputs, outcomes, 101, out successRatio);

            int notAssignedCount = 0;
            foreach (bool[] row in outcomeCodes)
            {
                if (row != null)
                {
                    Assert.IsFalse(row[(int)OutcomeCodeMatrixIndices.Buy]);
                    Assert.IsFalse(row[(int)OutcomeCodeMatrixIndices.Sell]);
                }
                else
                    notAssignedCount++;
            }
            Assert.AreEqual(0, notAssignedCount);
        }
    }
}
