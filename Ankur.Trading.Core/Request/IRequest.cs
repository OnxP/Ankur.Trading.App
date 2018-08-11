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
        string TradingPair { get; set; }
        TimeInterval Interval { get; set; }
        TradingAlgorthm Algorthm { get; set; }
        decimal StartAmount { get; set; }
        //not for simplicity the back test will make market limit orders the entry price.
        OrderType OrderType { get; set; }
        IEnumerable<TradingResult> TradingResults { get; set; }
        bool TestTrade { get; }

        void Add(TradingResult result);
    }
}
