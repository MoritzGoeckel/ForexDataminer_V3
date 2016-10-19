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
                if (double.IsNaN(row[(int)OutcomeMatrixIndices.Actual]) == false && double.IsNaN(row[(int)OutcomeMatrixIndices.Max]) == false && double.IsNaN(row[(int)OutcomeMatrixIndices.Min]) == false)
                {
                    Assert.AreEqual(2, row[(int)OutcomeMatrixIndices.Actual]);
                    Assert.AreEqual(2, row[(int)OutcomeMatrixIndices.Min]);
                    Assert.AreEqual(2, row[(int)OutcomeMatrixIndices.Max]);
                }
                else
                    foundNaNs++;
            }

            Assert.AreEqual(10, foundNaNs);
            Assert.AreEqual(double.NaN, outcomes[outcomes.Length - 1][(int)OutcomeMatrixIndices.Actual]);
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
                if (double.IsNaN(outcomes[i][(int)OutcomeMatrixIndices.Actual]) == false && double.IsNaN(outcomes[i][(int)OutcomeMatrixIndices.Max]) == false && double.IsNaN(outcomes[i][(int)OutcomeMatrixIndices.Min]) == false)
                {
                    Assert.AreEqual(i + 9, outcomes[i][(int)OutcomeMatrixIndices.Actual]);
                    Assert.AreEqual(i + 1, outcomes[i][(int)OutcomeMatrixIndices.Min]);
                    Assert.AreEqual(i + 9, outcomes[i][(int)OutcomeMatrixIndices.Max]);
                }
                else
                    foundNaNs++;
            }

            Assert.AreEqual(10, foundNaNs);
            Assert.AreEqual(double.NaN, outcomes[outcomes.Length - 1][(int)OutcomeMatrixIndices.Actual]);
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
                if (double.IsNaN(outcomes[i][(int)OutcomeMatrixIndices.Actual]) == false && double.IsNaN(outcomes[i][(int)OutcomeMatrixIndices.Max]) == false && double.IsNaN(outcomes[i][(int)OutcomeMatrixIndices.Min]) == false)
                {
                    Assert.AreEqual(inputs.Length - (i + 9), outcomes[i][(int)OutcomeMatrixIndices.Actual]);
                    Assert.AreEqual(inputs.Length - (i + 9), outcomes[i][(int)OutcomeMatrixIndices.Min]);
                    Assert.AreEqual(inputs.Length - (i + 1), outcomes[i][(int)OutcomeMatrixIndices.Max]);
                }
                else
                    foundNaNs++;
            }

            Assert.AreEqual(10, foundNaNs);
            Assert.AreEqual(double.NaN, outcomes[outcomes.Length - 1][(int)OutcomeMatrixIndices.Actual]);
        }

        [TestMethod]
        public void getOutcomeCode_Test()
        {
            Assert.Fail();
        }
    }
}
