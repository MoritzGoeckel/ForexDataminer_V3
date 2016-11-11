using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V3_Trader_Project.Trader.Application;

namespace V3_Trader_Project.Trader.SignalMachines
{
    class MLSignalMachine : SignalMachine
    {
        public override double[] getSignal(long timestamp)
        {
            throw new NotImplementedException();
        }

        public override string getStateMessage()
        {
            throw new NotImplementedException();
        }

        public override void pushPrice(double[] price)
        {
            throw new NotImplementedException();
        }
    }
}
