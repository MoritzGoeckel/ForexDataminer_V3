using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V3_Trader_Project.Trader.Application
{
    class TooLittleValidDataException : Exception
    {
        public TooLittleValidDataException(string msg) : base(msg)
        {

        }
    }
}
