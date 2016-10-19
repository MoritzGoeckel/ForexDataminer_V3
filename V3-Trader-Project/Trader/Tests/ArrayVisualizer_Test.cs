using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V3_Trader_Project.Trader.Visualizers;

namespace V3_Trader_Project.Trader.Tests
{
    [TestClass]
    public class ArrayVisualizer_Test
    {
        [TestMethod]
        public void visualizeOutcomeCodeArray_Test()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void visualizeArray_Test()
        {
            Random z = new Random();
            double[] inputs = new double[1000];
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = i + 500;
            }

            Image img = ArrayVisualizer.visualizeArray(inputs, 500, 200, 10);
            //ArrayVisualizer.showImg(img);
            Bitmap bmp = new Bitmap(img);
            Assert.AreEqual(Color.FromArgb(255, 0, 128, 0), bmp.GetPixel(1, bmp.Height - 1));
            Assert.AreEqual(Color.FromArgb(255, 0, 128, 0), bmp.GetPixel(bmp.Width - 1, 1));
            Assert.AreEqual(Color.FromArgb(255, 0, 128, 0), bmp.GetPixel(bmp.Width / 2, bmp.Height / 2));
            Assert.AreEqual(Color.FromArgb(255, 211, 211, 211), bmp.GetPixel(bmp.Width / 2 - 20, bmp.Height / 2 + 20));
        }
    }
}
