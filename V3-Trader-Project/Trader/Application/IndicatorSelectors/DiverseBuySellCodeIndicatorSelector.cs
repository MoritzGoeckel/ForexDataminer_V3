using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Application.IndicatorSelectors
{
    class DiverseBuySellCodeIndicatorSelector : IndicatorSelector
    {
        struct ValueAndIDPair { public double _value; public string _id; };

        Dictionary<string, ValueAndIDPair> candidates = new Dictionary<string, ValueAndIDPair>();

        private int targetCount;
        private int runs;

        public DiverseBuySellCodeIndicatorSelector(int targetCount, int runs)
        {
            this.targetCount = targetCount;
            this.runs = runs;
        }

        public override string[] getResultingCandidates()
        {
            List<string> indicators = new List<string>();

            while (indicators.Count < targetCount && candidates.Count > 0)
            {
                //Find best one in candidates
                string biggestId = null;
                double biggestValue = double.MinValue;
                string biggestKey = null;
                foreach (KeyValuePair<string, ValueAndIDPair> pair in candidates)
                {
                    if(biggestValue < pair.Value._value)
                    {
                        biggestValue = pair.Value._value;
                        biggestId = pair.Value._id;
                        biggestKey = pair.Key;
                    }
                }

                //Add it
                if (biggestId == null)
                    throw new Exception("How can it be null?");

                indicators.Add(biggestId);
                candidates.Remove(biggestKey);
            }
            
            return indicators.ToArray();
        }

        public override void pushIndicatorStatistics(LearningIndicator li)
        {
            string id = li.getName();
            string algo = li.getName().Split('_')[0];

            double[] pp = li.getPredictivePowerArray();

            double buySellCodeScore = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.maxBuyCode] + pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.maxSellCode];

            double tmp = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.spBuy];
            buySellCodeScore += double.IsNaN(tmp) ? 0 : Math.Abs(tmp);

            tmp = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.spSell];
            buySellCodeScore += double.IsNaN(tmp) ? 0 : Math.Abs(tmp);

            tmp = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.pBuy];
            buySellCodeScore += double.IsNaN(tmp) ? 0 : Math.Abs(tmp);

            tmp = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.pSell];
            buySellCodeScore += double.IsNaN(tmp) ? 0 : Math.Abs(tmp);

            tmp = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.spActual];
            buySellCodeScore += double.IsNaN(tmp) ? 0 : Math.Abs(tmp);

            tmp = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.pActual];
            buySellCodeScore += double.IsNaN(tmp) ? 0 : Math.Abs(tmp);

            tmp = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.maxMinMaxDistancePercent] / 100;
            buySellCodeScore += double.IsNaN(tmp) ? 0 : Math.Abs(tmp);

            tmp = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.pMax];
            buySellCodeScore += double.IsNaN(tmp) ? 0 : Math.Abs(tmp);

            tmp = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.pMin];
            buySellCodeScore += double.IsNaN(tmp) ? 0 : Math.Abs(tmp);

            tmp = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.pBuy];
            buySellCodeScore += double.IsNaN(tmp) ? 0 : Math.Abs(tmp);

            tmp = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.pSell];
            buySellCodeScore += double.IsNaN(tmp) ? 0 : Math.Abs(tmp);

            tmp = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.spMax];
            buySellCodeScore += double.IsNaN(tmp) ? 0 : Math.Abs(tmp);

            tmp = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.spMin];
            buySellCodeScore += double.IsNaN(tmp) ? 0 : Math.Abs(tmp);

            tmp = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.spBuy];
            buySellCodeScore += double.IsNaN(tmp) ? 0 : Math.Abs(tmp);

            tmp = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.spSell];
            buySellCodeScore += double.IsNaN(tmp) ? 0 : Math.Abs(tmp);

            ValueAndIDPair pair = new ValueAndIDPair() { _id = id, _value = buySellCodeScore };

            if (candidates.ContainsKey(algo) == false)
                candidates.Add(algo, pair);
            else if (candidates[algo]._value < buySellCodeScore)
                candidates[algo] = pair;

            analysedIndicators++;
        }

        private int analysedIndicators = 0;

        private int okayOnes = 0;
        public override bool isSatisfied()
        {
            /*try {
                okayOnes = 0;
                foreach (KeyValuePair<string, ValueAndIDPair> pair in candidates)
                {
                    if (pair.Value._value > 1.8)
                        okayOnes++;
                }
            }
            catch (Exception e) { }*/

            return analysedIndicators > runs || okayOnes >= targetCount;
        }

        public override string getState()
        {
            string s = "";
            foreach (KeyValuePair<string, ValueAndIDPair> pair in candidates)
            {
                s += pair.Key + " " + pair.Value._value + Environment.NewLine;
            }

            s += "Todo: " + (analysedIndicators - runs) + Environment.NewLine +
                "Okay: " + okayOnes + "/" + targetCount;

            return s;
        }
    }
}
