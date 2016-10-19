using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Tests
{
    [TestClass]
    public class Timestamp_Test
    {
        [TestMethod]
        public void TimestampGeneral_Test()
        {
            string timestampStr = "20150101 130021493";
            DateTime timestampDt = new DateTime(2015, 1, 1, 13, 00, 21, 493, DateTimeKind.Utc);

            Assert.AreEqual(Timestamp.dateTimeToMilliseconds(timestampDt), Timestamp.getUTCMillisecondsDate(timestampStr));
            Assert.AreEqual(DateTime.Now.ToUniversalTime().ToString("YYYYMMDDhhmmss"), Timestamp.getDate(Timestamp.getNow()).ToString("YYYYMMDDhhmmss"));
        }
    }
}
