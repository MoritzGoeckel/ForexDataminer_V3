using NinjaTrader_Client.Trader.Indicators;
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

namespace V3_Trader_Project.Trader.Forms
{
    public partial class IndicatorTestingForm : Form
    {
        public IndicatorTestingForm()
        {
            InitializeComponent();
        }

        IndicatorOptimizer op;
        Image priceAndOutcomes;

        private void button_run_Click(object sender, EventArgs e)
        {
            LearningIndicator li = op.testIndicator(new VolumeAtPriceIndicator(24 * 60 * 60 * 1000, 0.001, 10)); //Decide indicator somehow
            Image samplings = li.visualizeTables(2000, 1500);
            Image values = li.visualizeIndicatorValues(2000, 500, op.getPriceData());

            this.Name = li.getName();

            Image o = new Bitmap(values.Width + samplings.Width, priceAndOutcomes.Height + values.Height);
            Graphics g = Graphics.FromImage(o);
            g.Clear(Color.White);

            //g.DrawImage(samplings, 0, 0);
            g.DrawImage(priceAndOutcomes, 0, values.Height);
            g.DrawImage(values, 0, 0);

            g.DrawLine(new Pen(Color.Blue, 3), 0, values.Height, values.Width, values.Height);
            g.DrawImage(samplings, values.Width, 0);

            this.BackgroundImage = o;
        }

        private void IndicatorTestingForm_Load(object sender, EventArgs e)
        {
            op = new IndicatorOptimizer(Config.DataPath, Config.DataPath + "EURUSD", 60, 1000l * 60 * 60 * 24 * 30);
            op.loadOutcomeCodes(1 * 60 * 60 * 1000, 0.5);
            this.BackgroundImage = priceAndOutcomes = op.visualizePriceAndOutcomeCodes(2000, 1000);
        }
    }
}
