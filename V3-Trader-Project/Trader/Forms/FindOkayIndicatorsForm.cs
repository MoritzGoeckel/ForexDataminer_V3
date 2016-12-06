using NinjaTrader_Client.Trader.Indicators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using V3_Trader_Project.Trader.Application;

namespace V3_Trader_Project.Trader.Forms
{
    public partial class FindOkayIndicatorsForm : Form
    {
        public FindOkayIndicatorsForm(long outcomeTimeframe, double outcomeCodePercent, double minPercentThreshold, int samplingSteps, string pair, long timeframeToTest)
        {
            InitializeComponent();
            this.outcomeTimeframe = outcomeTimeframe;
            this.outcomeCodePercent = outcomeCodePercent;
            this.minPercentThreshold = minPercentThreshold;
            this.learningIndicatorSteps = samplingSteps;
            this.pair = pair;
            this.timeframeToTest = timeframeToTest;
        }

        long outcomeTimeframe;
        double outcomeCodePercent;
        double minPercentThreshold;
        int learningIndicatorSteps;
        long timeframeToTest;

        string pair;
        Dictionary<string, bool> found = new Dictionary<string, bool>();

        int tried = 0;

        private void FindOkayIndicatorsForm_Load(object sender, EventArgs e)
        {
            timer1.Start();

            string okayIndicatorsFile = "okayIndicators" + outcomeTimeframe + ".txt";

            if(File.Exists(okayIndicatorsFile))
            {
                List<string> lines = File.ReadLines(okayIndicatorsFile).ToList();
                foreach(string line in lines)
                {
                    if (line != "" && line != null && line != " ")
                        found.Add(line, true);
                }
            }

            DataLoader dl = new DataLoader(Config.DataPath + pair);
            double[][] priceData = dl.getArray(1000 * 60 * 60 * 24 * 30l,
                timeframeToTest,
                10 * 1000); //One month, but the second. Every 10 secs

            double success;
            bool[][] outcomeCodeFirstData = OutcomeGenerator.getOutcomeCodeFirst(priceData, outcomeTimeframe, outcomeCodePercent, out success);

            if (success < 0.7)
                throw new Exception("OutcomeCode low success: " + success);

            double[][] outcomeData = OutcomeGenerator.getOutcome(priceData, outcomeTimeframe, out success);

            if (success < 0.7)
                throw new Exception("Outcome low success: " + success);

            IndicatorGenerator generator = new IndicatorGenerator();

            new Thread(delegate() {
                while (true)
                {
                    WalkerIndicator ind = generator.getGeneratedIndicator(Convert.ToInt32((outcomeTimeframe / 1000) / 10), Convert.ToInt32((outcomeTimeframe / 1000) * 100));

                    try
                    {
                        if (found.ContainsKey(ind.getName()) == false)
                        {
                            tried++;
                            new LearningIndicator(ind, priceData, outcomeCodeFirstData, outcomeData, outcomeTimeframe, outcomeCodePercent, minPercentThreshold, learningIndicatorSteps, false);
                            File.AppendAllText(okayIndicatorsFile, ind.getName() + Environment.NewLine);
                            found.Add(ind.getName(), true);
                        }
                    }
                    catch (Exception) { }
                }
            }).Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = "Tried: " + tried + " / Found: " + found.Count() + " :)";
        }
    }
}
