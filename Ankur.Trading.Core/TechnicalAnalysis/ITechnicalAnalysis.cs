using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core.Trading_Algorthm
{
    interface ITechnicalAnalysis
    {
        void AddCandleSticks(IDictionary<string, Candlestick> candlestick);
        IEnumerable<IAlgorthmResults> RunAlgorthm();
    }
}
