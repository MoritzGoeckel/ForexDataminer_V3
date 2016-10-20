using Microsoft.VisualStudio.TestTools.UnitTesting;
using NinjaTrader_Client.Trader.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Tests
{
    [TestClass]
    public class IndicatorRunner_Test
    {
        [TestMethod]
        public void getIndicatorValues_Test()
        {
            double[][] inputs = new double[100][];
            DateTime dt = DateTime.Now.ToUniversalTime();
            for (int i = 0; i < inputs.Length; i++)
            {
                //Date bid ask volume
                dt = dt.AddMilliseconds(1000);
                inputs[i] = new double[] { Timestamp.dateTimeToMilliseconds(dt), i, i, 0 };
            }

            double validRatio;
            double[] values = IndicatorRunner.getIndicatorValues(inputs, new TestIndicator(), out validRatio);
            Assert.AreEqual(1, validRatio);

            for (int i = 0; i < values.Length; i++)
            {
                Assert.AreEqual(i * 2, values[i]);
            }
        }
    }
}
