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

        public decimal Bought => buyTrade.Price * buyTrade.Quantity;
        public decimal Sold => sellTrade.Price * sellTrade.Quantity * -1;
        public decimal Pnl => Bought - Sold;
        public decimal PnlPercent => Math.Round((1 - Sold / Bought) * 100,2);
    }
}
