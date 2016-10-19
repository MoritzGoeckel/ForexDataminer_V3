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
        public static double[] getIndicatorValues(double[][] priceData, WalkerIndicator indicator, out double indicatorValidRatio)
        {
            indicatorValidRatio = 0;
            double[] output = new double[priceData.Length];

            for (int i = 0; i < priceData.Length; i++)
            {
                long timestamp = Convert.ToInt64(priceData[i][(int)DataIndeces.Date]);
                double mid = (priceData[i][(int)DataIndeces.Ask] + priceData[i][(int)DataIndeces.Bid]) / 2d;
                double value = indicator.setNextDataAndGetIndicator(timestamp, mid);

                if (value != double.NaN && double.IsInfinity(value) == false && indicator.isValid(timestamp))
                {
                    output[i] = value;
                    indicatorValidRatio++;
                }
                else
                    output[i] = double.NaN;
            }

            indicatorValidRatio /= output.Length;

            return output;
        }
    }
}
