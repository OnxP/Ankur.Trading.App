using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.Trades;

namespace Ankur.Trading.Core.Trading_Algorthm
{
    public class TradingLog
    {
        private Trade transaction;
        private Trade BtcTransaction;

        public TradingLog(IEnumerable<Trade> transactionPair)
        {
            this.transaction = transactionPair.First();
            this.BtcTransaction = transactionPair.Last();
        }

        public TradingLog(IEnumerable<Trade> transactionPair, DateTime closeTime)
        {
            this.transaction = transactionPair.First();
            this.BtcTransaction = transactionPair.Last();
            this.CloseTime = closeTime;
        }

        public DateTime CloseTime { get; set; }
        public string Pair => transaction.Symbol;
        public decimal Quantity => transaction.Quantity;
        public decimal BtcQuantity => BtcTransaction.Quantity;
        public decimal Price => Math.Round(Math.Abs(BtcQuantity) / Math.Abs(Quantity),6);
    }
}
