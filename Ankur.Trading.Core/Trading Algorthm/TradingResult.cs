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
        private Trade trade;

        public TradingResult(IEnumerable<Trade> transactionPair) : this(transactionPair.First(), transactionPair.Last())
        {
            
        }

        public TradingResult(Trade trade)
        {
            this.trade = trade;
        }

        public TradingResult(Trade buyTrade, Trade sellTrade)
        {
            this.buyTrade = buyTrade;
            this.sellTrade = sellTrade;
        }

        public DateTime FirstTrade => buyTrade.Time;
        public DateTime LastTrade => sellTrade.Time;
        public decimal Bought => Math.Round(buyTrade.Quantity,5);
        public decimal BtcBought => Math.Round(buyTrade.BtcQuantity*-1,5);
        public decimal Sold => Math.Round(sellTrade.Quantity*-1,5);
        public decimal BtcSold => Math.Round(sellTrade.BtcQuantity,5);
        public decimal Pnl => Math.Round(BtcSold - BtcBought,5);
        public decimal PnlPercent => Math.Round(((BtcSold / BtcBought-1)) * 100,5);
        public string Ticker => buyTrade.Symbol;

        public override string ToString()
        {
            return $"Ticker: {Ticker} Bought: {Bought} @ {buyTrade.Price} = {BtcBought} Sold: {Sold} @ {sellTrade.Price} = {BtcSold} PNL: {Pnl} - {PnlPercent}%";
        }

    }
}
