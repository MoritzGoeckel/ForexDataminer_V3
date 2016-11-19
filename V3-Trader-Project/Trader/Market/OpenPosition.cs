using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Market
{
    public class OpenPosition
    {
        public double priceOpen;
        public long timestampOpen;
        public MarketModul.OrderType type;
        public double amount;

        public OpenPosition(double amount, long timestampOpen, double priceOpen, MarketModul.OrderType type)
        {
            this.timestampOpen = timestampOpen;
            this.priceOpen = priceOpen;
            this.type = type;
            this.amount = amount;
        }

        public double getProfitNoAmount(double[] priceData)
        {
            if (type == MarketModul.OrderType.Long)
                return priceData[(int)PriceDataIndeces.Bid] - priceOpen;
            else
                return priceOpen - priceData[(int)PriceDataIndeces.Ask];
        }

        public double getProfit(double[] priceData)
        {
            return getProfitNoAmount(priceData) * amount;
        }

        public double getProfitPercent(double[] priceData)
        {
            double p = getProfitNoAmount(priceData);
            return ((priceOpen + p / priceOpen) - 1d) * 100d;
        }

        public long getTimeInMarket(double[] priceData)
        {
            return Convert.ToInt64(priceData[(int)PriceDataIndeces.Date] - timestampOpen);
        }
    }
}
