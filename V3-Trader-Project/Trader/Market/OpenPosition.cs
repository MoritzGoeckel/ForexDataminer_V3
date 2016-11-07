using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Market
{
    class OpenPosition
    {
        public double price;
        public long timestamp;
        public MarketModul.OrderType type;
        public double amount;

        public OpenPosition(double amount, long timestamp, double price, MarketModul.OrderType type)
        {
            this.timestamp = timestamp;
            this.price = price;
            this.type = type;
            this.amount = amount;
        }
    }
}
