using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.Trading_Algorthm;
using Binance.API.Csharp.Client.Models.Enums;

namespace Ankur.Trading.Core.Request
{
    public interface IRequest
    {
        List<string> TradingPairs { get; set; }
        TimeInterval Interval { get; set; }
        TradingAlgorthm Algorthm { get; set; }
        decimal StartAmount { get; set; }
        //not for simplicity the back test will make market limit orders the entry price.
        OrderType OrderType { get; set; }
        IEnumerable<ITradingResult> TradingResults { get; set; }
        bool TestTrade { get; }
        DateTime From { get; set; }
        DateTime To { get; set; }
        bool IsTest { get; set; }
        decimal FinalAmount { get; set; }
        decimal TradingAmount { get; set; }

        void Add(ITradingResult result);
    }
}
