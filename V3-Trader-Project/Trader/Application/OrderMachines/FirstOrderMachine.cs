using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V3_Trader_Project.Trader.SignalMachines;

namespace V3_Trader_Project.Trader.Application.OrderMachines
{
    class FirstOrderMachine : OrderMachine
    {
        public override void doOrderTick(long timestamp, double[] signal)
        {
            throw new NotImplementedException(); //Todo: !! :)

            //aim for outcome codes
            if(signal[(int)SignalMachineSignal.BuySignal] > 0.7)
            {

            }
            else if (signal[(int)SignalMachineSignal.SellSignal] > 0.7)
            {

            }

            //aim for prediction
            if(signal[(int)SignalMachineSignal.prediction] > 0.2)
            {

            }

            //aim for min max difference
            if (signal[(int)SignalMachineSignal.maxPrediction]
                + signal[(int)SignalMachineSignal.minPrediction] > 0.1)
            {

            }
        }
    }
}
