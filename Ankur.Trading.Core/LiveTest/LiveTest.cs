using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ankur.Trading.Core.Log;
using Ankur.Trading.Core.Request;
using Ankur.Trading.Core.Trading;
using Ankur.Trading.Core.Trading_Strategy;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core.LiveTest
{
    public class LiveTest : TradingTest
    {
        public LiveTest(IRequest request) : base(request)
        {
            this._request = request;
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

        public override bool LoadAllCandleSticks()
        {
            throw new NotImplementedException();
        }
    }
}