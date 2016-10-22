using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Application
{
    public enum SignalMachineSignal
    {
        BuySignal = 0, SellSignal = 1
    };

    public abstract class SignalMachine
    {
        public abstract double[] getSignal(long timestamp);
    }
}
