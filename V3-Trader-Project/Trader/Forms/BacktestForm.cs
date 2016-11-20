using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using V3_Trader_Project.Trader.Application;
using V3_Trader_Project.Trader.Application.IndicatorSelectors;
using V3_Trader_Project.Trader.Application.OrderMachines;
using V3_Trader_Project.Trader.Market;
using V3_Trader_Project.Trader.SignalMachines;

namespace V3_Trader_Project.Trader.Forms
{
    public partial class BacktestForm : Form
    {
        public BacktestForm()
        {
            InitializeComponent();
        }

        string pair = "EURUSD";
 
        private void BacktestForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataLoader dl = new DataLoader(Config.DataPath + pair);
            double[][] priceData = dl.getArray(0,
                31l * 24l * 60l * 60l * 1000l * 11,
                10 * 1000);

            long outcomeTimeframe = 1000 * 60 * 60 * 4;
            double outcomeCodePercent = 0.15;

            MarketModul mm = new MarketModul(pair);
            OrderMachine om = new FirstOrderMachine(mm, outcomeCodePercent, outcomeTimeframe);

            StreamingStrategy strategy = new StreamingStrategy(outcomeCodePercent, outcomeTimeframe, mm, om, 0.3, "#cache");

            string lastMessage = "";

            long lastUpdateTimestamp = 0;
            for (int i = 0; i < priceData.Length; i++)
            {
                long timestampNow = Convert.ToInt64(priceData[i][(int)PriceDataIndeces.Date]);
                mm.pushPrice(priceData[i]);
                strategy.pushPrice(priceData[i]);

                if (lastUpdateTimestamp == 0)
                    lastUpdateTimestamp = timestampNow;

                if (timestampNow - (2l * 24 * 60 * 60 * 1000) > lastUpdateTimestamp)
                {
                    Logger.log("Updateing indicators...");
                    strategy.updateIndicators(1000l * 60 * 60 * 24 * 10,
                        1000l * 60 * 60 * 24 * 5, 
                        new DiverseIndicatorSelector(8, 1000));

                    lastUpdateTimestamp = timestampNow;
                    Logger.log("End updateing indicators.");
                }

                double percent = Convert.ToDouble(i) / priceData.Length * 100d;

                string msg = "Progress: " + Math.Round(percent, 0) + "%";
                if (lastMessage != msg)
                {
                    this.Text = msg;
                    Logger.log(msg);
                    lastMessage = msg;
                }
            }

            mm.flatAll(Convert.ToInt64(priceData[priceData.Length - 1][(int)PriceDataIndeces.Date]));

            mm.removeInvalidTimeFramePositions(outcomeTimeframe * 2, outcomeCodePercent + 0.01, -(outcomeCodePercent + 0.01));

            string report = mm.getStatisticsString() + Environment.NewLine 
                + om.getStatistics() + Environment.NewLine 
                + mm.getProfitabilityByInfoString();

            MessageBox.Show(report);

            this.BackgroundImage = mm.getCapitalCurveVisualization(this.Width, this.Height);
        }
    }
}
