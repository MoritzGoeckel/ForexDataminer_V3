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
    public partial class OptimizeIndicatorForm : Form
    {
        TestingEnvironment env;
        IndicatorOptimizer op;

        public OptimizeIndicatorForm()
        {
            InitializeComponent();
        }

        private void OptimizeIndicatorForm_Load(object sender, EventArgs e)
        {
            env = new TestingEnvironment(Config.DataPath, Config.DataPath + "EURUSD", 60, 1000l * 60 * 60 * 24 * 30);
            env.loadOutcomeCodes(1 * 60 * 60 * 1000, 0.5);
            op = new IndicatorOptimizer(Config.DataPath, env);
            op.startRunningRandomIndicators(new IndicatorGenerator());

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = op.state;
        }
    }
}
