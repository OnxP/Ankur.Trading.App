using System;
using System.Collections.Generic;
using System.Linq;
using Ankur.Trading.Core.Request;
using Ankur.Trading.Core.Trading_Algorthm;
using Binance.API.Csharp.Client.Models.Enums;

namespace Ankur.Trading.Core.LiveTest
{
    public class LiveTestRequest : IRequest
    {
        public List<string> TradingPairs { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public TimeInterval Interval { get; set; }
        public TradingAlgorthm Algorthm { get; set; }
        public decimal StartAmount { get; set; }
        public decimal TradingAmount { get; set; }
        //not for simplicity the back test will make market limit orders the entry price.
        public OrderType OrderType { get; set; }
        public IEnumerable<ITradingResult> TradingResults { get; set; }

        public bool TestTrade => true;

        public decimal FinalAmount { get; set; }
        public bool IsTest => true;

        bool IRequest.IsTest { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Add(ITradingResult tradingResult)
        {
            var list = TradingResults.ToList();
            list.Add(tradingResult);
            TradingResults = list;
        }
    }
}
