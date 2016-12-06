using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Application.IndicatorSelectors
{
    class StDIndicatorSelector : IndicatorSelector
    {
        struct ValueAndIDPair { public double _value; public string _id; };

        Dictionary<string, ValueAndIDPair> candidates = new Dictionary<string, ValueAndIDPair>();

        private int targetCount;

        public StDIndicatorSelector(int targetCount)
        {
            this.targetCount = targetCount;
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

            double score = 0;

            score = pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.buySellCodeDistanceStD] //Codeprop
                + pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.buyCodeStD] //Codeprop
                + pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.sellCodeStD] //Codeprop
                //+ pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.minStD] //Diff
                //+ pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.maxStD] //Diff
                //+ pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.minMaxDistanceStd] //Diff
                + pp[(int)LearningIndicator.LearningIndicatorPredictivePowerIndecies.actualStD]; //Pred

            ValueAndIDPair pair = new ValueAndIDPair() { _id = id, _value = score };

            if (candidates.ContainsKey(algo) == false)
                candidates.Add(algo, pair);
            else if (candidates[algo]._value < score)
                candidates[algo] = pair;
        }

        public override string getState()
        {
            string s = "";
            foreach (KeyValuePair<string, ValueAndIDPair> pair in candidates)
            {
                s += pair.Key + " " + pair.Value._value + Environment.NewLine;
            }

            return s;
        }
    }
}
