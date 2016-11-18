using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Market
{
    public class OpenPosition
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

        public double getProfitNoAmount(double[] priceData)
        {
            if (type == MarketModul.OrderType.Long)
                return priceData[(int)PriceDataIndeces.Bid] - price;
            else
                return price - priceData[(int)PriceDataIndeces.Ask];
        }

        public double getProfit(double[] priceData)
        {
            return getProfitNoAmount(priceData) * amount;
        }

        public double getProfitPercent(double[] priceData)
        {
            double p = getProfitNoAmount(priceData);
            return ((price + p / price) - 1d) * 100d;
        }

        public long getTimeInMarket(double[] priceData)
        {
            return Convert.ToInt64(priceData[(int)PriceDataIndeces.Date] - timestamp);
        }
    }
}
