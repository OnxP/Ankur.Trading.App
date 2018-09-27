using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.Trading_Algorthm;
using Ankur.Trading.Core.Trading_Strategy;
using Ankur.Trading.Core.Trading;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using System.Threading;
using Ankur.Trading.Core.Log;
using Ankur.Trading.Core.Request;

namespace Ankur.Trading.Core.BackTest
{
    public class BackTest : TradingTest
    {
        public BackTest(IRequest request):base(request)
        {
            technicalAnalysis = new TechnicalAnalysis(_binanceClient, _request);
            tradingStrategy = new TradingStrategy(_binanceClient, _request);
            LastPrices = new Dictionary<string, decimal>();
            CandleSticks = new Queue<Dictionary<string, Candlestick>>();
            IsLastCandleStick = false;
            foreach (var item in _request.TradingPairs)
            {
                LastPrices.Add(item, 0m);
            }
        }

        public override IDictionary<string, Candlestick> GetNextCandleSticks()
        {
            while (CandleSticks.Count == 0 && !CandleSticksLoaded)
            {
                //Wait 1 second.
                Thread.Sleep(1000);
            }
            var candlesticks = CandleSticks.Dequeue();
            if (CandleSticks.Count == 0 && CandleSticksLoaded) IsLastCandleStick = true;
            return candlesticks;
        }
    }
}
