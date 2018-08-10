using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankur.Trading.Core.Trades
{
    public class Position
    {
        public bool Open { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }
        public decimal Quantity { get; set; }
        public string Currency { get; set; }
        public decimal BoughtPrice { get; set;}
        public decimal SoldPrice { get; set;}

        internal void Add(Position position)
        {
            throw new NotImplementedException();
        }
    }
}
