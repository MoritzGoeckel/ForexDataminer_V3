using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            double[][] outcomes = OutcomeGenerator.getOutcome(inputs, 1000 * 10);

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

            double[][] outcomes = OutcomeGenerator.getOutcome(inputs, 1000 * 10);

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

            double[][] outcomes = OutcomeGenerator.getOutcome(inputs, 1000 * 10);

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

            bool[][] outcomeCodes = OutcomeGenerator.getOutcomeCode(inputs, outcomes, 100);

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

            bool[][] outcomeCodes = OutcomeGenerator.getOutcomeCode(inputs, outcomes, 100);

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

            bool[][] outcomeCodes = OutcomeGenerator.getOutcomeCode(inputs, outcomes, 101);

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
