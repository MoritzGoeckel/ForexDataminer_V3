using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Application.IndicatorSelectors
{
    public abstract class IndicatorSelector
    {
        public abstract void pushIndicatorStatistics(LearningIndicator li);
        public abstract string[] getResultingCandidates();
        public abstract bool isSatisfied();
        public abstract string getState();
    }
}
