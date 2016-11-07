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

        TestingEnvironment env;
        Image priceAndOutcomes;

        string pair = "EURUSD";
        string[] indicatorStrings = { "MovingAverageSubtractionIndicator_86324000_358872000",
            "MACDContinousIndicator_215409000_123126000_46108000",
            "StandartDeviationIndicator_149905000",
            "RangeIndicator_355669000",
            "MovingAveragePriceSubtractionIndicator_305560000"};

        private void BacktestForm_Load(object sender, EventArgs e)
        {
            //Set these args
            env = new TestingEnvironment(Config.DataPath, Config.DataPath + pair, 60, 1000l * 60 * 60 * 24 * 30);
            env.loadOutcomeCodes(1 * 60 * 60 * 1000, 0.5);
            this.BackgroundImage = priceAndOutcomes = env.visualizePriceAndOutcomeCodes(2000, 1000);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            List<LearningIndicator> indicators = new List<LearningIndicator>();

            foreach(string s in indicatorStrings)
                indicators.Add(env.createTrainedIndicator(IndicatorGenerator.getIndicatorByString(s)));

            SignalMachine sm = new LIWightedSignalMachine(indicators.ToArray());
            MarketModul mm = new MarketModul("EURUSD");
            FirstOrderMachine om = new FirstOrderMachine(mm, 0.06, 3600000);

            DataLoader dl = new DataLoader(Config.DataPath + pair);
            double[][] priceData = dl.getArray(31l * 24l * 60l * 60l * 1000l, 31l * 24l * 60l * 60l * 1000l, 60 * 1000);

            for (int i = 0; i < priceData.Length; i++)
            {
                long timestamp = Convert.ToInt64(priceData[i][(int)PriceDataIndeces.Date]);
                mm.pushPrice(priceData[i]);
                sm.pushPrice(priceData[i]);
                om.doOrderTick(timestamp, sm.getSignal(timestamp));

                double percent = Convert.ToDouble(i) / priceData.Length * 100d;
                this.Text = Math.Round(percent, 0) + "%";
            }

            mm.flatAll(Convert.ToInt64(priceData[priceData.Length - 1][(int)PriceDataIndeces.Date]));

            MessageBox.Show(om.BuySignals + " " + om.SellSignals);
            
            MessageBox.Show(mm.getStatisticsString());
            this.BackgroundImage = mm.getCapitalCurveVisualization(this.Width, this.Height);
        }
    }
}
