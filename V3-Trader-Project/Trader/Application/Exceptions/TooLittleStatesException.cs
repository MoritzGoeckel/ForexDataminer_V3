using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Application
{
    class TooLittleStatesException : Exception
    {
        public TooLittleStatesException(string msg) : base(msg)
        {

        }
    }
}
