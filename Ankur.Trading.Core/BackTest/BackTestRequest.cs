using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.Request;
using Ankur.Trading.Core.Trading_Algorthm;
using Binance.API.Csharp.Client.Models.Enums;

namespace Ankur.Trading.Core.BackTest
{
    public class BackTestRequest : IRequest
    {
        public string TradingPair { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public TimeInterval Interval { get; set; }
        public TradingAlgorthm Algorthm { get; set; }
        public decimal StartAmount { get; set; }
        //not for simplicity the back test will make market limit orders the entry price.
        public OrderType OrderType { get; set; }
        public IEnumerable<TradingResult> TradingResults { get; set; }

        public bool TestTrade => true;

        public object FinalAmount { get; internal set; }

        public void Add(TradingResult tradingResult)
        {
            var list = TradingResults.ToList();
            list.Add(tradingResult);
            TradingResults = list;
        }
    }
}
