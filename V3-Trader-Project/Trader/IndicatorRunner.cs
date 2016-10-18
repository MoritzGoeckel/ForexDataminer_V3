using NinjaTrader_Client.Trader.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader
{
    public class IndicatorRunner
    {
        public static double[] getIndicatorValues(double[][] priceData, WalkerIndicator indicator)
        {
            double[] output = new double[priceData.Length];
            for(int i = 0; i < priceData.Length; i++)
            {
                double mid = (priceData[i][(int)DataIndeces.Ask] + priceData[i][(int)DataIndeces.Bid]) / 2d;
                output[i] = indicator.setNextDataAndGetIndicator(Convert.ToInt64(priceData[i][(int)DataIndeces.Date]), mid);
            }
            return output;
        }
    }
}
