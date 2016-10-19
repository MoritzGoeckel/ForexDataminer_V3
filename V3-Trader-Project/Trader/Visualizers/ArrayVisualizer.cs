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
    public class ArrayVisualizer
    {
        public Image visualizeOutcomeCodeArray(bool[][] outcomeCodes, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);

            double stepSize = Convert.ToDouble(outcomeCodes.Length) / Convert.ToDouble(width);

            for (int x = 0; x < width; x++)
            {
                int index = Convert.ToInt32(stepSize * x);

                Color buyColor = outcomeCodes[index][(int)OutcomeCodeMatrixIndices.Buy] ? Color.Green : Color.Gray;
                Color sellColor = outcomeCodes[index][(int)OutcomeCodeMatrixIndices.Sell] ? Color.Blue : Color.Gray;

                for (int y = 0; y < height / 2; y++)
                    bmp.SetPixel(x, y, buyColor);

                for (int y = height / 2; y < height; y++)
                    bmp.SetPixel(x, y, sellColor);
            }
            
            return bmp;
        }

        public Image visualizeArray(double[] input, int width, int height, int lineSize = 3)
        {
            Bitmap bmp = new Bitmap(width, height);

            double stepSize = Convert.ToDouble(input.Length) / Convert.ToDouble(width);

            double min = input.Min(), max = input.Max();

            int oldX = -1, oldY = -1;

            Color cUp = Color.Green;
            Color cDown = Color.Blue;

            for (int x = 0; x < width; x++)
            {
                int index = Convert.ToInt32(stepSize * x);
                int y = Convert.ToInt32((max - min) * (input[index] - min) * height);

                if (oldX != -1)
                    for (int yOffset = -(lineSize / 2); yOffset < (lineSize / 2); yOffset++)
                        if(y + yOffset > 0 && y + yOffset < height)
                            bmp.SetPixel(x, y + yOffset, y > oldY ? cDown : cUp);

                oldX = x;
                oldY = y;
            }

            return bmp;
        }

        public void showImg(Image img)
        {
            img.Save("tmp.png");
            Process.Start("tmp.png");
            try {
                File.Delete("tmp.png"); //Not sure whether that works
            }
            catch { }
        }
    }
}
