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

        IndicatorOptimizer op;
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            new IndicatorTestingForm().ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new OptimizeIndicatorForm().ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            new BacktestForm().ShowDialog();
        }
    }
}
