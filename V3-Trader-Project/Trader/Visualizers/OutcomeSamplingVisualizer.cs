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
        public static Image visualizeOutcomeSamplingTable(double[][] table, int width, int height, double nowValue)
        {
            Image img = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(img);
            g.Clear(Color.LightGray);

            double minValue = double.MaxValue, maxValue = double.MinValue;
            double minSamples = double.MaxValue, maxSamples = double.MinValue;

            double maxMax = double.MinValue;
            double minMin = double.MaxValue;

            //Find min max's
            for (int rowId = 0; rowId < table.Length; rowId++)
            {
                if (table[rowId] != null && table[rowId][(int)SampleValuesOutcomeCodesIndices.SamplesCount] != 0)
                {
                    double value = table[rowId][(int)SampleValuesOutcomeIndices.Start];
                    double samples = table[rowId][(int)SampleValuesOutcomeIndices.SamplesCount];

                    double max = table[rowId][(int)SampleValuesOutcomeIndices.MaxAvg];
                    double min = table[rowId][(int)SampleValuesOutcomeIndices.MinAvg];

                    if (max > maxMax)
                        maxMax = max;
                    if (min < minMin)
                        minMin = min;

                    if (value > maxValue)
                        maxValue = value;
                    if (value < minValue)
                        minValue = value;

                    if (samples > maxSamples)
                        maxSamples = samples;
                    if (samples < minSamples)
                        minSamples = samples;
                }
            }

            Brush maxBrush = new SolidBrush(Color.Green);
            Brush minBrush = new SolidBrush(Color.Blue);
            Brush actualBrush = new SolidBrush(Color.Yellow);

            double norm;
            if (Math.Abs(minMin) > Math.Abs(maxMax))
                norm = Math.Abs(minMin);
            else
                norm = Math.Abs(maxMax);

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
                    double value = table[rowId][(int)SampleValuesOutcomeIndices.Start];
                    double samples = table[rowId][(int)SampleValuesOutcomeIndices.SamplesCount];

                    double min = table[rowId][(int)SampleValuesOutcomeIndices.MinAvg];
                    double max = table[rowId][(int)SampleValuesOutcomeIndices.MaxAvg];
                    double actual = table[rowId][(int)SampleValuesOutcomeIndices.ActualAvg];

                    double samplesColor = ((samples - minSamples) / (maxSamples - minSamples)) * 255;

                    double border = columnSize / 5d;

                    //Draw the samples amount
                    if (double.IsNaN(samplesColor))
                        samplesColor = 1;

                    g.FillRectangle(new SolidBrush(Color.FromArgb(Convert.ToInt32(samplesColor), Convert.ToInt32(samplesColor), Convert.ToInt32(samplesColor))), Convert.ToInt32(x), 0, Convert.ToInt32(columnSize), height);

                    double mid = height / 2d;

                    //Min Max Actual
                    g.FillRectangle(maxBrush, Convert.ToInt32(x), Convert.ToInt32(mid) - Convert.ToInt32(height / 2 * max / norm), Convert.ToInt32(columnSize), Convert.ToInt32(height / 2 * max / norm));
                    g.FillRectangle(minBrush, Convert.ToInt32(x), Convert.ToInt32(mid), Convert.ToInt32(columnSize), Convert.ToInt32(height / 2 * Math.Abs(min) / norm));

                    if(actual > 0)
                        g.FillRectangle(actualBrush, Convert.ToInt32(x + border), Convert.ToInt32(mid) - Convert.ToInt32(height / 2 * actual / norm), Convert.ToInt32(columnSize - border * 2), Convert.ToInt32(height / 2 * Math.Abs(actual) / norm));
                    else
                        g.FillRectangle(actualBrush, Convert.ToInt32(x + border), Convert.ToInt32(mid), Convert.ToInt32(columnSize - border * 2), Convert.ToInt32(height / 2 * Math.Abs(actual) / norm));
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

            Brush buyPen = new SolidBrush(Color.Green);
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
