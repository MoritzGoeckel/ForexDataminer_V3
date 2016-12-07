using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        string pair;

        public BacktestForm(string pair, long outcomeTimeframe, double outcomeCodePercent, double minPercentThreshold, 
            int samplingSteps, long updateFrequency, long updateLookback, long indicatorInitTime, int indicatorsToChooseCount, long monthsToTest, long minTimestep)
        {
            this.outcomeTimeframe = outcomeTimeframe;
            this.outcomeCodePercent = outcomeCodePercent;
            this.minPercentThreshold = minPercentThreshold;
            this.samplingSteps = samplingSteps;
            this.updateFrequency = updateFrequency;
            this.updateLookback = updateLookback;
            this.indicatorInitTime = indicatorInitTime;
            this.indicatorsToChooseCount = indicatorsToChooseCount;
            this.pair = pair;
            this.monthsToTest = monthsToTest;
            this.minTimestep = minTimestep;

            InitializeComponent();
        }

        long outcomeTimeframe;
        double outcomeCodePercent;
        double minPercentThreshold;
        int samplingSteps;
        long updateFrequency;
        long updateLookback;
        long indicatorInitTime;
        int indicatorsToChooseCount;
        long monthsToTest;
        long minTimestep;

        private void BacktestForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataLoader dl = new DataLoader(Config.DataPath + pair);
            double[][] priceData = dl.getArray(0,
                31l * 24l * 60l * 60l * 1000l * monthsToTest + updateLookback,
                minTimestep);

            MarketModul mm = new MarketModul(pair);
            OrderMachine om = new FirstOrderMachine(mm, outcomeCodePercent, outcomeTimeframe);

            List<string> okayIndicators;
            string okayIndicatorsFile = "okayIndicators" + outcomeTimeframe + ".txt";
            if (File.Exists(okayIndicatorsFile))
                okayIndicators = File.ReadAllText(okayIndicatorsFile).Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            else
                throw new Exception("Okay indicators not found!");

            StreamingStrategy strategy = new StreamingStrategy(outcomeCodePercent, outcomeTimeframe, mm, om, minPercentThreshold, samplingSteps, okayIndicators, "#cache");

            string lastMessage = "";

            long beginningTimestamp = Convert.ToInt64(priceData[0][(int)PriceDataIndeces.Date]);
            long lastUpdateTimestamp = 0;
            for (int i = 0; i < priceData.Length; i++)
            {
                long timestampNow = Convert.ToInt64(priceData[i][(int)PriceDataIndeces.Date]);
                mm.pushPrice(priceData[i]);
                strategy.pushPrice(priceData[i]);

                if (lastUpdateTimestamp == 0)
                    lastUpdateTimestamp = timestampNow - updateFrequency + updateLookback;

                if (timestampNow - updateFrequency > lastUpdateTimestamp)
                {
                    Logger.log("Updateing indicators...");
                    strategy.updateIndicators(updateLookback,
                        indicatorInitTime, 
                        new StDIndicatorSelector(indicatorsToChooseCount));

                    strategy.getSignalMachine().visualize(1500, 2).Save("SignalMachineVis" + timestampNow + ".png");
                    //Todo: save that somehow

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
            
            string report = "NOT removed: " + Environment.NewLine
                + mm.getStatisticsString() + Environment.NewLine
                + om.getStatistics() + Environment.NewLine
                + mm.getProfitabilityByInfoString();

            mm.removeInvalidClosedPositions(outcomeTimeframe * 3, 100, -100);

            report += Environment.NewLine + Environment.NewLine
                + "Removed: " + Environment.NewLine
                + mm.getStatisticsString() + Environment.NewLine 
                + om.getStatistics() + Environment.NewLine 
                + mm.getProfitabilityByInfoString();

            Clipboard.SetText(report);

            MessageBox.Show(report);

            this.BackgroundImage = mm.getCapitalCurveVisualization(this.Width, this.Height);
        }
    }
}
