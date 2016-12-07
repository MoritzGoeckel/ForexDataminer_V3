using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using V3_Trader_Project.Trader;
using V3_Trader_Project.Trader.Application;
using V3_Trader_Project.Trader.Forms;

namespace V3_Trader_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        long outcomeTimeframe = 1000 * 60 * 60 * 4;
        double outcomeCodePercent = 0.06;
        double minPercentThreshold = 1;
        int samplingSteps = 20;
        long updateFrequency = 5l * 24 * 60 * 60 * 1000l;
        long updateLookback = 1000l * 60 * 60 * 24 * 15;
        long indicatorInitTime = 1000l * 60 * 60 * 24 * 3;
        int indicatorsToChooseCount = 8;
        long monthsToTest = 3;
        string pair = "EURUSD";
        long minTimestep = 1000 * 30;

        private void findIndicators_btn_Click(object sender, EventArgs e)
        {
            FindOkayIndicatorsForm form = new FindOkayIndicatorsForm(outcomeTimeframe, outcomeCodePercent, minPercentThreshold, samplingSteps, pair, updateLookback, minTimestep);
            form.ShowDialog();
        }

        private void backtest_btn_Click(object sender, EventArgs e)
        {
            BacktestForm form = new BacktestForm(pair, outcomeTimeframe, outcomeCodePercent, minPercentThreshold, samplingSteps, updateFrequency, updateLookback, indicatorInitTime, indicatorsToChooseCount, monthsToTest, minTimestep);
            form.ShowDialog();
        }
    }
}
