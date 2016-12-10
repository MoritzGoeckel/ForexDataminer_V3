using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V3_Trader_Project.Trader.Application;

namespace V3_Trader_Project.Trader.SignalMachines
{
    class AlternativeSignalMachine : SignalMachine
    {
        private LearningIndicator[] indicators;

        public AlternativeSignalMachine(LearningIndicator[] indicators)
        {
            this.indicators = indicators;
        }

        public override double[] getSignal(long timestamp)
        {
            double sumMax = 0;
            double sumMin = 0;
            double sumActual = 0;
            double buyPropSum = 0, sellPropSum = 0;

            double wight = 1d / indicators.Length;

            for (int i = 0; i < indicators.Length; i++)
            {
                double[] pred = indicators[i].getPrediction(timestamp);

                sumMax += pred[(int)LearningIndicatorPredictionIndecies.AvgOutcomeMax] * wight;
                sumMin += pred[(int)LearningIndicatorPredictionIndecies.AvgOutcomeMin] * wight;

                sumActual += pred[(int)LearningIndicatorPredictionIndecies.AvgOutcomeActual] * wight;

                buyPropSum += pred[(int)LearningIndicatorPredictionIndecies.BuyCodeProbability] > 0.5 ? wight : 0;
                sellPropSum += pred[(int)LearningIndicatorPredictionIndecies.SellCodeProbability] > 0.5 ? wight : 0;
            }

            return new double[] { buyPropSum, sellPropSum, sumMin, sumMax, sumActual };
        }

        public override void pushPrice(double[] price)
        {
            foreach (LearningIndicator i in this.indicators)
                i.setNewPrice(price);
        }
        
        //Todo: Untested
        public override Image visualize(int width, int inRow)
        {
            int height = indicators.Length / inRow * width / 2;
            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Black);

            int border = 10;

            int indexInRow = 0;
            int row = 0;

            int widthPerIndicator = width / inRow;
            int heightPerIndicator = height / indicators.Length;
            for(int i = 0; i < indicators.Length; i++)
            {
                if(indexInRow < inRow)
                {
                    g.DrawImage(indicators[i].visualizeTables(widthPerIndicator - border, heightPerIndicator - border), indexInRow * widthPerIndicator, row * heightPerIndicator);
                    indexInRow++;
                }
                else
                {
                    indexInRow = 0;
                    row++;
                }
            }

            return bmp;
        }

        public override string getStateMessage()
        {
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < indicators.Length; i++)
            {
                double[] pred = indicators[i].getPredictivePowerArray();

                if (pred != null)
                    foreach (double d in pred)
                        output.Append(Math.Round(d, 3) + " ");
                else
                    output.Append("No stats for learning indicator calculated");

                output.Append(Environment.NewLine);
            }

            return output.ToString();
        }
    }
}
