using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.API.Csharp.Client.Models.Account;
using Binance.API.Csharp.Client.Models.Market;
using Ankur.Trading.Core.Trading_Algorthm;

namespace Ankur.Trading.Core.Trades
{
    public class Trade
    {

        public Trade(Order result,TradeAction action)
        {
            this.Symbol = result.Symbol;
            this.Time = DateTimeOffset.FromUnixTimeMilliseconds(result.Time).DateTime;
            this.Quantity = result.ExecutedQty;
        }

        public Trade(string pair, decimal quantity)
        {
            this.Symbol = pair;
            this.Time = DateTime.Now;
            this.Quantity = quantity;
        }

        public Trade(string pair,decimal currentPrice,decimal quantity)
        {
            this.Symbol = pair;
            this.Time = DateTime.Now;
            this.Quantity = quantity;
        }

        public TradeAction Action => Quantity > 0 ? TradeAction.Buy : TradeAction.Sell;
        public DateTime Time { get; private set; }
        public decimal Price => Math.Round(BtcQuantity / Quantity * -1,6);
        public decimal Quantity { get; private set; }
        public string Symbol { get; private set; }
        public Trade CounterTrade { get; set; }
        public decimal BtcQuantity => CounterTrade.Quantity;
    }
}
