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
        public Trade(Order result)
        {
            this.Symbol = result.Symbol;
            this.Time = DateTimeOffset.FromUnixTimeMilliseconds(result.Time).DateTime;
            this.Price = result.Price;
            this.Quantity = result.ExecutedQty;
        }
        public Trade(string pair,decimal currentPrice,decimal quantity)
        {
            this.Symbol = pair;
            this.Time = DateTime.Now;
            this.Price = currentPrice;
            this.Quantity = quantity;
        }

        public DateTime Time { get; private set; }
        public decimal Price { get; private set; }
        public decimal Quantity { get; private set; }
        public string Symbol { get; private set; }
    }
}
