using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Visualizers
{
    public static class OutcomeSamplingVisualizer
    {
        /*public static Image visualizeOutcomeSamplingTable(double[][] table, int width, int height)
        {

        }*/

        public static Image visualizeOutcomeCodeSamplingTable(double[][] table, int width, int height, double nowValue = double.NaN)
        {
            Image img = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(img);
            g.Clear(Color.LightGray);

            double minValue = double.MaxValue, maxValue = double.MinValue;
            double minSamples = double.MaxValue, maxSamples = double.MinValue;

            double maxBuy = double.MinValue, maxSell = double.MinValue;

            //Find min max's
            for (int rowId = 0; rowId < table.Length; rowId++)
            {
                if (table[rowId] != null && table[rowId][(int)SampleValuesOutcomeCodesIndices.SamplesCount] != 0)
                { 
                    double value = table[rowId][(int)SampleValuesOutcomeCodesIndices.Start];
                    double samples = table[rowId][(int)SampleValuesOutcomeCodesIndices.SamplesCount];
                    double buy = table[rowId][(int)SampleValuesOutcomeCodesIndices.BuyRatio];
                    double sell = table[rowId][(int)SampleValuesOutcomeCodesIndices.SellRatio];

                    if (value > maxValue)
                        maxValue = value;
                    if (value < minValue)
                        minValue = value;

                    if (samples > maxSamples)
                        maxSamples = samples;
                    if (samples < minSamples)
                        minSamples = samples;

                    if (buy > maxBuy)
                        maxBuy = buy;

                    if (sell > maxSell)
                        maxSell = sell;
                }
            }

            Brush buyPen = new SolidBrush(Color.LightGreen);
            Brush sellPen = new SolidBrush(Color.Blue);

            double columnSize = Convert.ToDouble(width) / table.Length;
            for (int rowId = 0; rowId < table.Length; rowId++)
            {
                double x = columnSize * rowId;

                if (table[rowId] == null || table[rowId][(int)SampleValuesOutcomeCodesIndices.SamplesCount] == 0)
                {
                    g.FillRectangle(new SolidBrush(Color.LightSalmon), Convert.ToInt32(x), 0, Convert.ToInt32(columnSize), height);
                }
                else
                {
                    double value = table[rowId][(int)SampleValuesOutcomeCodesIndices.Start];
                    double samples = table[rowId][(int)SampleValuesOutcomeCodesIndices.SamplesCount];
                    double buy = table[rowId][(int)SampleValuesOutcomeCodesIndices.BuyRatio];
                    double sell = table[rowId][(int)SampleValuesOutcomeCodesIndices.SellRatio];

                    double samplesColor = ((samples - minSamples) / (maxSamples - minSamples)) * 255;

                    double border = columnSize / 5d;

                    //Draw the samples amount
                    g.FillRectangle(new SolidBrush(Color.FromArgb(Convert.ToInt32(samplesColor), Convert.ToInt32(samplesColor), Convert.ToInt32(samplesColor))), Convert.ToInt32(x), 0, Convert.ToInt32(columnSize), height);
                    g.FillRectangle(buyPen, Convert.ToInt32(x), height - Convert.ToInt32(height * buy), Convert.ToInt32(columnSize), Convert.ToInt32(height * buy));
                    g.FillRectangle(sellPen, Convert.ToInt32(x + border), height - Convert.ToInt32(height * sell), Convert.ToInt32(columnSize - border * 2), Convert.ToInt32(height * sell));

                    /*if (buy == maxBuy) //Todo: Maybe keep that? Show max for sell and buy
                    {
                        g.FillEllipse(buyPen, Convert.ToInt32(x), Convert.ToInt32(columnSize), Convert.ToInt32(columnSize), Convert.ToInt32(columnSize));
                        g.DrawEllipse(Pens.Red, Convert.ToInt32(x), Convert.ToInt32(columnSize), Convert.ToInt32(columnSize), Convert.ToInt32(columnSize));
                    }

                    if (sell == maxSell)
                    {
                        g.FillEllipse(sellPen, Convert.ToInt32(x), Convert.ToInt32(columnSize), Convert.ToInt32(columnSize), Convert.ToInt32(columnSize));
                        g.DrawEllipse(Pens.Red, Convert.ToInt32(x), Convert.ToInt32(columnSize), Convert.ToInt32(columnSize), Convert.ToInt32(columnSize));
                    }*/
                }
            }

            if (double.IsNaN(nowValue) == false)
            {
                Pen currentValuePen = new Pen(Color.Red, 5);
                int nowValueX = Convert.ToInt32(((nowValue - minValue) / (maxValue - minValue)) * Convert.ToDouble(width));
                g.DrawLine(currentValuePen, nowValueX, 0, nowValueX, height);
            }

            return img;
        }
    }
}
