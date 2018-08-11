using System;
using Ankur.Trading.Core.Trades;

namespace Ankur.Trading.Core.Trading_Algorthm
{
    public class TradingResult
    {
        private Trade buyTrade;
        private Trade sellTrade;

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
    }
}
