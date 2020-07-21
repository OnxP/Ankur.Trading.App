using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Ankur.Trading.Core.Trades
{
    public class Trades
    {
        public Trade OpenTrade { get; set; }
        public Trade CloseTrade { get; set; }

    }
}
