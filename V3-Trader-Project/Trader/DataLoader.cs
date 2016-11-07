using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader
{
    public enum PriceDataIndeces : int
    {
        Date = 0, Bid = 1, Ask = 2, Volume = 3
    }

    public class DataLoader
    {
        private List<string> filenames;
        private string rootPath;

        public DataLoader(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            filenames = new List<string>();
            foreach(FileInfo file in dir.GetFiles())
                filenames.Add(file.Name);

            filenames.Sort();
            rootPath = dir.FullName + "/";
        }

        public List<string> getFiles()
        {
            List<string> output = new List<string>();
            output.AddRange(filenames);
            return output;
        }

        public double[][] getArray(long minDateDistance = 1, long onlyTimeframe = 0)
        {
            long lastAddedDate = 0;
            long firstTimestamp = 0;
            List<double[]> rows = new List<double[]>();
            foreach(string file in filenames)
            {
                foreach(string line in File.ReadAllLines(rootPath + file))
                {
                    string[] values = line.Split(',');
                    long dateL = Timestamp.getUTCMillisecondsDate(values[(int)PriceDataIndeces.Date]);

                    if (firstTimestamp == 0)
                        firstTimestamp = dateL;
                    
                    if (dateL - lastAddedDate > minDateDistance)
                    {
                        rows.Add(new double[] { dateL, double.Parse(values[(int)PriceDataIndeces.Bid].Replace(".", ",")), double.Parse(values[(int)PriceDataIndeces.Ask].Replace(".", ",")), double.Parse(values[(int)PriceDataIndeces.Volume].Replace(".", ",")) });
                        lastAddedDate = dateL;
                    }

                    if (onlyTimeframe != 0 && dateL > firstTimestamp + onlyTimeframe)
                        return rows.ToArray(); //Stop
                }
            }

            return rows.ToArray();
        }

        public double[][] getArray(long offset, long timeframe, long minDateDistance = 1)
        {
            long lastAddedDate = 0;
            long firstTimestamp = 0;
            List<double[]> rows = new List<double[]>();
            foreach (string file in filenames)
            {
                foreach (string line in File.ReadAllLines(rootPath + file))
                {
                    string[] values = line.Split(',');
                    long dateL = Timestamp.getUTCMillisecondsDate(values[(int)PriceDataIndeces.Date]);

                    if (firstTimestamp == 0)
                        firstTimestamp = dateL;

                    if (dateL - offset > firstTimestamp && dateL - lastAddedDate > minDateDistance)
                    {
                        rows.Add(new double[] { dateL, double.Parse(values[(int)PriceDataIndeces.Bid].Replace(".", ",")), double.Parse(values[(int)PriceDataIndeces.Ask].Replace(".", ",")), double.Parse(values[(int)PriceDataIndeces.Volume].Replace(".", ",")) });
                        lastAddedDate = dateL;
                    }

                    if (timeframe != 0 && dateL > firstTimestamp + offset + timeframe)
                        return rows.ToArray(); //Stop
                }
            }

            return rows.ToArray();
        }
    }
}
