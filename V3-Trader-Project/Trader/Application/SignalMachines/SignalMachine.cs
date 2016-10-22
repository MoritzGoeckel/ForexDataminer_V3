using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.SignalMachines
{
    public enum SignalMachineSignal
    {
        BuySignal = 0, SellSignal = 1, minPrediction = 2, maxPrediction = 3, prediction = 4
    };

    public abstract class SignalMachine
    {
        public abstract double[] getSignal(long timestamp);
        public abstract void pushPrice(double[] price);
    }
}
