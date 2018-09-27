using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.Log;
using Ankur.Trading.Core.Trading_Algorthm;

namespace Ankur.Trading.Core.Trading_Strategy
{
    interface ITradingStrategy
    {
        decimal CurrentBtcAmount { get; }

        void Process(IEnumerable<IAlgorthmResults> results);
        void ClosePositions(IDictionary<string, decimal> lastPrices);
        IEnumerable<ITradingResult> TradingResults();
        event LogHandler.LogHandlerDelegate Log;
    }

    
}
