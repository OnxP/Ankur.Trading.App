using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankur.Trading.Core.Trading_Algorthm
{
    //this should class should contain the result of the algorthm and evidence to explain why the recommendation was made.
    public class AlgorthmResults : IAlgorthmResults
    {
        public string Ticker => ticker;
        private readonly string ticker;

        public AlgorthmResults(string ticker)
        {
            this.ticker = ticker;
        }

        public TradeAction Action { get; set; }
        public decimal LastPrice { get; set; }
        public DateTime CloseDateTime { get; set; }
    }

    public enum TradeAction { Buy, Sell, Wait}
}
