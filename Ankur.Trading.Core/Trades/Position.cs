using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankur.Trading.Core.Trades
{
    public class Position
    {
        public bool Open => Trades.Count >= 1 && Quantity != 0;
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }
        public decimal Quantity { get; set; }
        public string Currency { get; set; }
        public decimal BoughtPrice { get; set;}
        public decimal SoldPrice { get; set;}

        internal IList<Trade> Trades;
        internal void Add(Trade trade)
        {
            Trades.Add(trade);
        }
    }
}
