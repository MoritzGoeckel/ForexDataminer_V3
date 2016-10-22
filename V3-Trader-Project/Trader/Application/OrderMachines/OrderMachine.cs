using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V3_Trader_Project.Trader.SignalMachines;

namespace V3_Trader_Project.Trader.Application.OrderMachines
{
    public abstract class OrderMachine
    {
        public OrderMachine() //Todo: give a market module to exec orders
        {

        }

        public abstract void doOrderTick(long timestamp, double[] signal);
    }
}
