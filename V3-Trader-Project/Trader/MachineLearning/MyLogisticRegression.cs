﻿using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;
using NinjaTrader_Client.Trader.Datamining.AI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NinjaTrader_Client.Trader.Analysis.Datamining.AI
{
    class MyLogisticRegression : IMachineLearning
    {
        LogisticRegression logisticBuy;
        IterativeReweightedLeastSquares teacherBuy;

        LogisticRegression logisticSell;
        IterativeReweightedLeastSquares teacherSell;

        int inputsCount;

        public MyLogisticRegression(int inputsCount)
        {
            this.inputsCount = inputsCount;
            logisticBuy = new LogisticRegression(inputs: inputsCount);
            teacherBuy = new IterativeReweightedLeastSquares(logisticBuy);

            logisticSell = new LogisticRegression(inputs: inputsCount);
            teacherSell = new IterativeReweightedLeastSquares(logisticSell);
        }

        public double getError()
        {
            return error;
        }

        public string[] getInfo(string[] inputFieldName, string outputFieldName)
        {
            throw new Exception("No info for the logistic regression. They are all the same :)");
        }

        public string getInfoString()
        {
            throw new Exception("No info for the logistic regression. They are all the same :)");
        }

        public double[] getPrediction(double[] input)
        {
            return new double[] { logisticBuy.Compute(input), logisticSell.Compute(input) };
        }

        private class BuySellLogisticPair
        {
            public LogisticRegression buy, sell;
        }

        public void load(string path)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            string xmlString = xmlDocument.OuterXml;

            using (StringReader read = new StringReader(xmlString))
            {
                Type outType = typeof(BuySellLogisticPair);

                XmlSerializer serializer = new XmlSerializer(outType);
                using (XmlReader reader = new XmlTextReader(read))
                {
                    BuySellLogisticPair pair = (BuySellLogisticPair)serializer.Deserialize(reader);

                    logisticBuy = pair.buy;
                    logisticSell = pair.sell;

                    reader.Close();
                }

                read.Close();
            }
        }

        public void save(string path)
        {
            BuySellLogisticPair pair = new BuySellLogisticPair();
            pair.buy = logisticBuy;
            pair.sell = logisticSell;

            XmlDocument xmlDocument = new XmlDocument();
            XmlSerializer serializer = new XmlSerializer(pair.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, pair);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(path);
                stream.Close();
            }
        }

        double error;
        public void train(double[][] input, double[][] output, int epochs = 1)
        {
            if (input[0].Length != inputsCount)
                throw new Exception("Input has a unexpected length: " + input[0].Length + "!=" + inputsCount);

            double[] buyOutput = new double[output.Length];
            double[] sellOutput = new double[output.Length];

            for(int i = 0; i < output.Length; i++)
            {
                buyOutput[i] = output[i][0];
                sellOutput[i] = output[i][1];
            }

            for (int i = 0; i < epochs; i++)
            {
                teacherBuy.Learn(input, buyOutput);
                teacherSell.Learn(input, sellOutput);
            }
        }

        public double getPredictionErrorFromData(double[][] input, double[][] output)
        {
            double[] buyOutput = new double[output.Length];
            double[] sellOutput = new double[output.Length];

            for (int i = 0; i < output.Length; i++)
            {
                buyOutput[i] = output[i][0];
                sellOutput[i] = output[i][1];
            }

            return (teacherBuy.ComputeError(input, buyOutput) + teacherSell.ComputeError(input, sellOutput)) / 2;
        }
    }
}
