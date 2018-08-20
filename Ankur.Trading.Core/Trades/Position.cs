using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankur.Trading.Core.Trades
{
    public class Position
    {
        public bool Open => Trades.Count >= 1 || Quantity != 0;
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }
        public decimal Quantity => Trades.Sum(x => x.Quantity) + StartingAmount;
        public string Currency { get; set; }
        public decimal BoughtPrice { get; set; }
        public decimal SoldPrice { get; set; }
        public decimal StartingAmount { get; }

        internal IList<Trade> Trades = new List<Trade>();
        public string Ticker {get;set;}
        private decimal Amount;

        public Position(string ticker, decimal startAmount)
        {
            this.Ticker = ticker;
            this.StartingAmount = startAmount;
        }

        internal void Add(Trade trade)
        {
            Trades.Add(trade);
        }
       
    }
}
