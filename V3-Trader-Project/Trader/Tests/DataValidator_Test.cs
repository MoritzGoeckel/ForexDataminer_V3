using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Tests
{
    [TestClass]
    public class DataValidator_Test
    {
        [TestMethod]
        public void checkGeneralArray_Test()
        {
            double[][] inputs = new double[100][];
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = new double[] { 0, 0, 0 };
            }

            Assert.IsTrue(DataValidator.checkGeneralArrayIsValid(inputs, false));
            Assert.IsTrue(DataValidator.checkGeneralArrayIsValid(inputs, true));

            inputs[10][2] = double.NaN;
            Assert.IsTrue(DataValidator.checkGeneralArrayIsValid(inputs, true));
            Assert.IsFalse(DataValidator.checkGeneralArrayIsValid(inputs, false));
            inputs[10][2] = 0;


            inputs[99][0] = double.MaxValue;
            Assert.IsFalse(DataValidator.checkGeneralArrayIsValid(inputs, false));
            Assert.IsFalse(DataValidator.checkGeneralArrayIsValid(inputs, true));
            inputs[99][0] = 0;

            inputs[15][1] = double.MinValue;
            Assert.IsFalse(DataValidator.checkGeneralArrayIsValid(inputs, false));
            inputs[15][1] = 0;

            inputs[12][2] = double.PositiveInfinity;
            Assert.IsFalse(DataValidator.checkGeneralArrayIsValid(inputs, false));
            inputs[12][2] = 0;

            inputs[50][1] = double.NegativeInfinity;
            Assert.IsFalse(DataValidator.checkGeneralArrayIsValid(inputs, false));
            inputs[50][1] = 0;

            Assert.IsTrue(DataValidator.checkGeneralArrayIsValid(inputs, false));
        }

        [TestMethod]
        public void checkPriceDataArray_Test()
        {
            Assert.Fail();
        }
    }
}
