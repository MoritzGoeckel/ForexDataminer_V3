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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            IndicatorOptimizer op = new IndicatorOptimizer(Config.DataPath, Config.DataPath + "EURUSD", 4 * 60 * 60 * 1000, 60);
            op.start();
        }
    }
}
