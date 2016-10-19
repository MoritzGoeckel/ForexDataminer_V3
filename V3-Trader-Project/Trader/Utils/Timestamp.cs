using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader
{
    public static class Timestamp
    {
        public static long getNow()
        {
            return (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))).TotalMilliseconds;
        }

        public static DateTime getDate(long timestamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(timestamp);
            return dtDateTime;
        }

        public static long getUTCMillisecondsDate(string input)
        {
            DateTime dtDateTime = new DateTime(Convert.ToInt32(input.Substring(0, 4)), Convert.ToInt32(input.Substring(4, 2)), Convert.ToInt32(input.Substring(6, 2)), Convert.ToInt32(input.Substring(9, 2)), Convert.ToInt32(input.Substring(11,2)), Convert.ToInt32(input.Substring(13, 2)), Convert.ToInt32(input.Substring(15, 3)), DateTimeKind.Utc);
            return dateTimeToMilliseconds(dtDateTime);
        }

        public static long dateTimeToMilliseconds(DateTime dt)
        {
            return (Int64)(dt.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))).TotalMilliseconds;
        }
    }
}
