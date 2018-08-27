using Binance.API.Csharp.Client.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankur.Trading.Core.Indicators
{
    interface IIndicator
    {
        decimal Value { get; }
        void AddCandleStick(Candlestick candleStick);
    }
}
