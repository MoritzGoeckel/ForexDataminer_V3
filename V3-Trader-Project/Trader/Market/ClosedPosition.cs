using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Market
{
    public class ClosedPosition
    {
        public long timestampOpen, timestampClose;
        public double priceOpen, priceClose;
        public MarketModul.OrderType type;
        public double amount;

        public ClosedPosition(OpenPosition openPosition, long timestampClose, double priceClose)
        {
            this.priceOpen = openPosition.priceOpen;
            this.timestampOpen = openPosition.timestampOpen;
            this.amount = openPosition.amount;

            this.priceClose = priceClose;
            this.timestampClose = timestampClose;
        }

        public double getProfitPercent()
        {
            return ((priceClose / priceOpen) - 1d) * 100d;
        }

        public double getProfitIngoreAmount()
        {
            if (type == MarketModul.OrderType.Long)
                return priceClose - priceOpen;
            else
                return priceClose - priceOpen;
        }

        public long getTimeDuration()
        {
            return timestampClose - timestampOpen;
        }

        public double getProfit()
        {
            return getProfitIngoreAmount() * amount;
        }
    }
}
