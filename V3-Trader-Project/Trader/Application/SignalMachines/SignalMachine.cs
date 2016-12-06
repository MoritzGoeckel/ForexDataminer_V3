using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.SignalMachines
{
    public enum SignalMachineSignal
    {
        BuyProbability = 0, SellProbability = 1, minPrediction = 2, maxPrediction = 3, prediction = 4
    };

    public abstract class SignalMachine
    {
        public abstract double[] getSignal(long timestamp);
        public abstract void pushPrice(double[] price);
        public abstract string getStateMessage();

        public string getInfo(long timestamp)
        {
            double[] s = getSignal(timestamp);

            return "BuySignal: " + s[(int)SignalMachineSignal.BuyProbability] + Environment.NewLine
                + "SellSignal: " + s[(int)SignalMachineSignal.SellProbability] + Environment.NewLine
                + "minPrediction: " + s[(int)SignalMachineSignal.minPrediction] + Environment.NewLine
                + "maxPrediction: " + s[(int)SignalMachineSignal.maxPrediction] + Environment.NewLine
                + "Prediction: " + s[(int)SignalMachineSignal.prediction] + Environment.NewLine;
        }
    }
}
