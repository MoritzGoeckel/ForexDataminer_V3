using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V3_Trader_Project.Trader.Application;

namespace V3_Trader_Project.Trader.SignalMachines
{
    class LIAggresiveSignalMachine : SignalMachine
    {
        private LearningIndicator[] indicators;

        public LIAggresiveSignalMachine(LearningIndicator[] indicators)
        {
            this.indicators = indicators;
        }

        public override double[] getSignal(long timestamp)
        {
            double maxMax = double.MinValue;
            double minMin = double.MaxValue;
            double sumActual = 0;
            double maxBuyProp = 0, maxSellProp = 0;

            for(int i = 0; i < indicators.Length; i++)
            {
                double[] pred = indicators[i].getPrediction(timestamp);

                if(maxMax < pred[(int)LearningIndicatorPredictionIndecies.AvgOutcomeMax])
                    maxMax = pred[(int)LearningIndicatorPredictionIndecies.AvgOutcomeMax];

                if(minMin > pred[(int)LearningIndicatorPredictionIndecies.AvgOutcomeMin])
                    minMin = pred[(int)LearningIndicatorPredictionIndecies.AvgOutcomeMin];

                sumActual += pred[(int)LearningIndicatorPredictionIndecies.AvgOutcomeActual];

                if(maxBuyProp < pred[(int)LearningIndicatorPredictionIndecies.BuyCodeProbability])
                    maxBuyProp = pred[(int)LearningIndicatorPredictionIndecies.BuyCodeProbability];

                if(maxSellProp < pred[(int)LearningIndicatorPredictionIndecies.SellCodeProbability])
                    maxSellProp = pred[(int)LearningIndicatorPredictionIndecies.SellCodeProbability];
            }

            return new double[] { maxBuyProp, maxSellProp, minMin, maxMax, sumActual / indicators.Length };
        }

        public override void pushPrice(double[] price)
        {
            foreach (LearningIndicator i in this.indicators)
                i.setNewPrice(price);
        }

        //Todo: Untested
        public Image visualize(int width, int inRow)
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
    }
}
