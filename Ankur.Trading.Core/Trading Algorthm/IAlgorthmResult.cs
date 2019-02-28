using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core.Trading_Algorthm
{
    public interface IAlgorthmResults
    {
        TradeAction Action { get; }
        string Ticker { get;  }
        decimal LastPrice { get; }
        DateTime CloseDateTime { get; }
        Candlestick Candlestick { get; }
    }
}
