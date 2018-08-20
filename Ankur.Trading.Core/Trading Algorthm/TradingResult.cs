using System;
using System.Collections.Generic;
using Ankur.Trading.Core.Trades;
using System.Linq;

namespace Ankur.Trading.Core.Trading_Algorthm
{
    public class TradingResult
    {
        private Trade buyTrade;
        private Trade sellTrade;
        private IEnumerable<Trade> transactionPair;

        public TradingResult(IEnumerable<Trade> transactionPair) : this(transactionPair.First(), transactionPair.Last())
        {
            
        }

        public TradingResult(Trade buyTrade, Trade sellTrade)
        {
            this.buyTrade = buyTrade;
            this.sellTrade = sellTrade;
        }

        public DateTime FirstTrade => buyTrade.Time;
        public DateTime LastTrade => sellTrade.Time;
        public decimal Bought => buyTrade.Price * buyTrade.Quantity;
        public decimal Sold => sellTrade.Price * sellTrade.Quantity * -1;
        public decimal Pnl => Sold - Bought;
        public decimal PnlPercent => Math.Round(((Sold / Bought) -1) * 100,2);

        public object Pair { get; set; }
    }
}
