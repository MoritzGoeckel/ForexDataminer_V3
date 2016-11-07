using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Tests
{
    [TestClass]
    public class DataLoader_Test
    {
        [TestMethod]
        public void DataLoader_FilenamesOrder()
        {
            DataLoader dl = new DataLoader(Config.DataPath + "TESTDATA");
            List<string> filenames = dl.getFiles();
            Assert.AreEqual("DAT_ASCII_EURUSD_T_201501.csv", filenames[0]);
            Assert.AreEqual("DAT_ASCII_EURUSD_T_201502.csv", filenames[1]);
            Assert.AreEqual("DAT_ASCII_EURUSD_T_201509.csv", filenames[2]);
            Assert.AreEqual("DAT_ASCII_EURUSD_T_201510.csv", filenames[3]);
            Assert.AreEqual("DAT_ASCII_EURUSD_T_201511.csv", filenames[4]);
            Assert.AreEqual("DAT_ASCII_EURUSD_T_201512.csv", filenames[5]);
            Assert.AreEqual("DAT_ASCII_EURUSD_T_201601.csv", filenames[6]);
            Assert.AreEqual(7, filenames.Count);
        }

        [TestMethod]
        public void DataLoader_LoadArray()
        {
            DataLoader dl = new DataLoader(Config.DataPath + "TESTDATA");
            List<string> filenames = dl.getFiles();
            double[][] array = dl.getArray();

            Assert.AreEqual(23d, array.Length);
            Assert.AreEqual(1.209650d, array[0][1]);
            Assert.AreEqual(1.210140d, array[1][2]);
            Assert.AreEqual(0d, array[1][3]);

            DateTime dt = new DateTime(2015, 1, 1, 13, 0, 21, 493);
            Assert.AreEqual(array[0][0], Timestamp.dateTimeToMilliseconds(dt)); //YYYYMMDD HHMMSSNNN

            //Third file
            Assert.AreEqual(1.209640, array[5][1]); 
        }

        [TestMethod] //Probably okay
        public void DataLoader_LoadArrayOffset()
        {
            DataLoader dl = new DataLoader(Config.DataPath + "TESTDATA");
            List<string> filenames = dl.getFiles();
            double[][] array = dl.getArray(25 * 1000, 50000, 1);

            DateTime dt = new DateTime(2015, 1, 1, 13, 0, 44, 493);
            Assert.AreEqual(array[0][0], Timestamp.dateTimeToMilliseconds(dt)); //YYYYMMDD HHMMSSNNN

            DateTime dt2 = new DateTime(2015, 1, 1, 13, 02, 40, 743);
            Assert.AreEqual(array[array.Length - 1][0], Timestamp.dateTimeToMilliseconds(dt2)); //YYYYMMDD HHMMSSNNN
        }
    }
}
