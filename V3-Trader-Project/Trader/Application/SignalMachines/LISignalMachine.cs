﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V3_Trader_Project.Trader.Application;

namespace V3_Trader_Project.Trader.SignalMachines
{
    class LISignalMachine : SignalMachine
    {
        private LearningIndicator[] indicators;

        public LISignalMachine(LearningIndicator[] indicators)
        {
            this.indicators = indicators;
        }

        public override double[] getSignal(long timestamp)
        {
            double sumMax = 0;
            double sumMin = 0;
            double sumActual = 0;
            double buyPropSum = 0, sellPropSum = 0;

            foreach (LearningIndicator i in this.indicators)
            {
                double[] pred = i.getPrediction(timestamp);
                sumMax += pred[(int)LearningIndicatorResult.AvgOutcomeMax];
                sumMin += pred[(int)LearningIndicatorResult.AvgOutcomeMin];
                sumActual += pred[(int)LearningIndicatorResult.AvgOutcomeActual];
                buyPropSum += pred[(int)LearningIndicatorResult.BuyCodeProbability];
                sellPropSum += pred[(int)LearningIndicatorResult.SellCodeProbability];
            }

            return new double[] { buyPropSum / indicators.Length, sellPropSum / indicators.Length, sumMin / indicators.Length, sumMax / indicators.Length, sumActual / indicators.Length };
        }

        public override void pushPrice(double[] price)
        {
            foreach (LearningIndicator i in this.indicators)
                i.setNewPrice(price);
        }

        public Image visualize()
        {
            throw new NotImplementedException(); //Todo: !! :)
        }
    }
}
