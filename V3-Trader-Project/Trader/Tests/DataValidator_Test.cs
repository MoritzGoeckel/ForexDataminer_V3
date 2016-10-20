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

            string msg;

            Assert.IsTrue(DataValidator.checkGeneralArrayIsValid(inputs, false, true, out msg));
            Assert.IsTrue(msg.StartsWith("OK"));
            Assert.IsTrue(DataValidator.checkGeneralArrayIsValid(inputs, true, true, out msg));
            Assert.IsTrue(msg.StartsWith("OK"));

            inputs[10][2] = double.NaN;
            Assert.IsTrue(DataValidator.checkGeneralArrayIsValid(inputs, true, true, out msg));
            Assert.IsTrue(msg.StartsWith("OK"));

            Assert.IsFalse(DataValidator.checkGeneralArrayIsValid(inputs, false, true, out msg));
            Assert.IsTrue(msg.StartsWith("Bad value:"));
            inputs[10][2] = 0;


            inputs[99][0] = double.MaxValue;
            Assert.IsFalse(DataValidator.checkGeneralArrayIsValid(inputs, false, true, out msg));
            Assert.IsTrue(msg.StartsWith("Bad value:"));
            Assert.IsFalse(DataValidator.checkGeneralArrayIsValid(inputs, true, true, out msg));
            Assert.IsTrue(msg.StartsWith("Bad value:"));
            inputs[99][0] = 0;

            inputs[15][1] = double.MinValue;
            Assert.IsFalse(DataValidator.checkGeneralArrayIsValid(inputs, false, true, out msg));
            Assert.IsTrue(msg.StartsWith("Bad value:"));
            inputs[15][1] = 0;

            inputs[12][2] = double.PositiveInfinity;
            Assert.IsFalse(DataValidator.checkGeneralArrayIsValid(inputs, false, true, out msg));
            Assert.IsTrue(msg.StartsWith("Bad value:"));
            inputs[12][2] = 0;

            inputs[50][1] = double.NegativeInfinity;
            Assert.IsFalse(DataValidator.checkGeneralArrayIsValid(inputs, false, true, out msg));
            Assert.IsTrue(msg.StartsWith("Bad value:"));
            inputs[50][1] = 0;

            Assert.IsTrue(DataValidator.checkGeneralArrayIsValid(inputs, false, true, out msg));
            Assert.IsTrue(msg.StartsWith("OK"));

            inputs[50] = null;
            Assert.IsTrue(DataValidator.checkGeneralArrayIsValid(inputs, false, true, out msg));
            Assert.IsTrue(msg.StartsWith("OK"));

            Assert.IsFalse(DataValidator.checkGeneralArrayIsValid(inputs, false, false, out msg));
            Assert.IsTrue(msg.StartsWith("Null row"));
        }

        [TestMethod]
        public void checkPriceDataArray_Test()
        {
            double[][] inputs = new double[100][];
            DateTime dt = DateTime.Now.ToUniversalTime();
            for (int i = 0; i < inputs.Length; i++)
            {
                //Date bid ask volume
                dt = dt.AddMilliseconds(1000);
                inputs[i] = new double[] { Timestamp.dateTimeToMilliseconds(dt), 2, 2, 0 };
            }

            string msg;
            Assert.IsTrue(DataValidator.checkPriceDataArray(inputs, out msg));
            Assert.IsTrue(msg.StartsWith("OK"));

            double okDate = inputs[30][0];

            inputs[30][0] = Timestamp.dateTimeToMilliseconds(DateTime.Now.ToUniversalTime());
            Assert.IsFalse(DataValidator.checkPriceDataArray(inputs, out msg));
            Assert.IsTrue(msg.StartsWith("Bad date"));

            inputs[30][0] = Timestamp.dateTimeToMilliseconds(DateTime.Now.ToUniversalTime().AddDays(10));
            Assert.IsFalse(DataValidator.checkPriceDataArray(inputs, out msg));
            Assert.IsTrue(msg.StartsWith("Time jump"));

            inputs[30][0] = okDate;
            double okBid = inputs[30][(int)PriceDataIndeces.Bid];

            inputs[30][(int)PriceDataIndeces.Bid] = inputs[30][(int)PriceDataIndeces.Bid] + (inputs[30][(int)PriceDataIndeces.Bid] / 100 * 16);
            Assert.IsFalse(DataValidator.checkPriceDataArray(inputs, out msg));
            Assert.IsTrue(msg.StartsWith("Bid jump"));

            inputs[30][(int)PriceDataIndeces.Bid] = okBid;
            inputs[30][(int)PriceDataIndeces.Bid] = inputs[30][(int)PriceDataIndeces.Bid] + (inputs[30][(int)PriceDataIndeces.Bid] / 100 * 10);
            Assert.IsTrue(DataValidator.checkPriceDataArray(inputs, out msg));
            Assert.IsTrue(msg.StartsWith("OK"));
        }
    }
}
