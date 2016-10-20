using Microsoft.VisualStudio.TestTools.UnitTesting;
using NinjaTrader_Client.Trader.Analysis.Datamining.AI;
using NinjaTrader_Client.Trader.Datamining.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Tests
{
    [TestClass]
    public class MachineLearning_Test
    {
        public double TestMachineLearning(IMachineLearning learning)
        {
            double[][] input = new double[10][];
            input[0] = new double[] { 1, 2, 1 };
            input[1] = new double[] { 1, 2, 1 };
            input[2] = new double[] { 1, 2, 2 };
            input[3] = new double[] { 2, 2, 1 };
            input[4] = new double[] { 2, 2, 3 };
            input[5] = new double[] { 3, 2, 2 };
            input[6] = new double[] { 3, 2, 1 };
            input[7] = new double[] { 3, 2, 5 };
            input[8] = new double[] { 4, 3, 3 };
            input[9] = new double[] { 4, 3, 2 };

            double[][] output = new double[10][];
            output[0] = new double[] { 1, 0 };
            output[1] = new double[] { 1, 0 };
            output[2] = new double[] { 1, 0 };
            output[3] = new double[] { 1, 1 };
            output[4] = new double[] { 1, 1 };
            output[5] = new double[] { 1, 1 };
            output[6] = new double[] { 0, 1 };
            output[7] = new double[] { 0, 1 };
            output[8] = new double[] { 0, 1 };
            output[9] = new double[] { 0, 1 };

            learning.train(input, output, 1);

            double[] low = learning.getPrediction(new double[] { 0, 0, 0 });
            double[] mid = learning.getPrediction(new double[] { 2, 2, 2 });
            double[] high = learning.getPrediction(new double[] { 5, 5, 5 });

            double score = Math.Abs(low[0] - 1) + Math.Abs(low[1] - 0) +
                Math.Abs(mid[0] - 1) + Math.Abs(mid[1] - 1) +
                Math.Abs(high[0] - 0) + Math.Abs(high[1] - 1);
            score /= 6;

            Assert.IsTrue(false, "Low: " + low[0] + " " + low[1] + Environment.NewLine + "Mid: " + mid[0] + " " + mid[1] + Environment.NewLine + "Heigh: " + high[0] + " " + high[1]);
            
            return score;
        }

        [TestMethod]
        public void NeuronalNetwork_Test()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void LogisticRegression_Test()
        {
            Assert.Fail(); //Takes really long
            
            MyLogisticRegression mlr = new MyLogisticRegression(3);
            Assert.AreEqual(0d, TestMachineLearning(mlr), 0.4);
        }

        [TestMethod]
        public void Regression_Test()
        {
            Assert.Fail(); //Takes really long

            MyRegression r = new MyRegression(3);
            Assert.AreEqual(0d, TestMachineLearning(r), 0.4);
        }
    }
}
