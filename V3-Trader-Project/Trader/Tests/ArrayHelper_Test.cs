using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Tests
{
    [TestClass]
    public class ArrayHelper_Test
    {
        [TestMethod]
        public void combineArraysHorizontal_Test()
        {
            double[] a = new double[] { 1, 2, 3, 4, 5 };

            double[] b1 = new double[] { 11, 22, 33, 44, 55 };
            double[] b2 = new double[] { 111, 222, 333, 444, 555 };
            double[] b3 = new double[] { 1111, 2222, 3333, 4444, 5555 };

            double[][] output = ArrayHelper.combineArraysHorizontal(a, new double[][] { b1, b2, b3 });

            Assert.AreEqual(1, output[0][0]);
            Assert.AreEqual(11, output[0][1]);
            Assert.AreEqual(111, output[0][2]);
            Assert.AreEqual(1111, output[0][3]);

            Assert.AreEqual(4, output[3][0]);
            Assert.AreEqual(2222, output[1][3]);
            
            Assert.AreEqual(5, output[4][0]);
            Assert.AreEqual(55, output[4][1]);
            Assert.AreEqual(555, output[4][2]);
            Assert.AreEqual(5555, output[4][3]);

            Assert.AreEqual(5, output.Length); //Zeilen
            Assert.AreEqual(4, output[0].Length); //Spalten
        }
    }
}
