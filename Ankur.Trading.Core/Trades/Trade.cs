using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.API.Csharp.Client.Models.Account;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core.Trades
{
    public class Trade
    {
        private Order result;
        private SymbolPrice currentPrice;

        public Trade(Order result)
        {
            this.result = result;
        }
        public Trade(SymbolPrice currentPrice)
        {
            this.currentPrice = currentPrice;
        }
    }
}
