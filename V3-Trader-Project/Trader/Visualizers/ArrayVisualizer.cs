using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Visualizers
{
    public static class ArrayVisualizer
    {
        public static Image visualizeOutcomeCodeArray(bool[][] outcomeCodes, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.LightGray);

            double stepSize = Convert.ToDouble(outcomeCodes.Length) / Convert.ToDouble(width);

            for (int x = 0; x < width; x++)
            {
                int index = Convert.ToInt32(stepSize * x);

                for (int y = 0; outcomeCodes[index][(int)OutcomeCodeMatrixIndices.Buy] && y < height / 2; y++)
                    bmp.SetPixel(x, y, Color.Green);

                for (int y = height / 2; outcomeCodes[index][(int)OutcomeCodeMatrixIndices.Sell] && y < height; y++)
                    bmp.SetPixel(x, y, Color.Blue);
            }
            
            return bmp;
        }

        public static Image visualizeArray(double[] input, int width, int height, int lineSize = 3)
        {
            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.LightGray);

            double stepSize = Convert.ToDouble(input.Length) / Convert.ToDouble(width);

            double min = input.Min(), max = input.Max();

            int oldX = -1, oldY = -1;

            Color cUp = Color.Green;
            Color cDown = Color.Blue;

            for (int x = 0; x < width; x++)
            {
                int index = Convert.ToInt32(stepSize * x);
                int y = height - Convert.ToInt32((input[index] - min) / (max - min) * height);

                if (oldX != -1)
                    for (int yOffset = -(lineSize / 2); yOffset < (lineSize / 2); yOffset++)
                    {
                        if (y + yOffset > 0 && y + yOffset < height)
                            bmp.SetPixel(x, y + yOffset, y > oldY ? cDown : cUp);
                    }

                oldX = x;
                oldY = y;
            }

            return bmp;
        }

        public static void showImg(Image img)
        {
            img.Save("tmp.png");
            Process.Start("tmp.png");
        }
    }
}
