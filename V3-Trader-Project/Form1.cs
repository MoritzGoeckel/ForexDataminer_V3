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

namespace V3_Trader_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        IndicatorOptimizer op;
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;

            this.BackgroundImageLayout = ImageLayout.Zoom;

            op = new IndicatorOptimizer(Config.DataPath, Config.DataPath + "EURUSD", 1 * 60 * 60 * 1000, 60, 0.5);
            op.start();
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;

            string pair = "EURUSD";

            IndicatorOptimizer longer = new IndicatorOptimizer(Config.DataPath, Config.DataPath + pair, 8 * 60 * 60 * 1000, 60);
            IndicatorOptimizer shorter = new IndicatorOptimizer(Config.DataPath, Config.DataPath + pair, 3 * 60 * 60 * 1000, 60);
            IndicatorOptimizer veryShort = new IndicatorOptimizer(Config.DataPath, Config.DataPath + pair, 60 * 60 * 1000, 30);
            IndicatorOptimizer veryLong = new IndicatorOptimizer(Config.DataPath, Config.DataPath + pair, 48 * 60 * 60 * 1000, 60);

            longer.start();
            shorter.start();
            veryShort.start();
            veryLong.start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.BackgroundImage = op.lastImage;
            this.Text = op.lastIndicatorName;
        }
    }
}
