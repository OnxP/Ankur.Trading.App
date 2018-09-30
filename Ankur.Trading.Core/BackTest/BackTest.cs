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

        public override bool LoadAllCandleSticks()
        {
            var from = _request.From;
            foreach (DateTime dt in SplitDates(_request.Interval, _request.From, _request.To))
            {
                LoadCandleSticksPerTicker(from, dt);
                from = dt;
            }
            CandleSticksLoaded = true;
            return IsLastCandleStick;
        }

        private void LoadCandleSticksPerTicker(DateTime from, DateTime dt)
        {
            var candlesticks = new Dictionary<string, List<Candlestick>>();
            foreach (var ticker in _request.TradingPairs)
            {
                candlesticks.Add(ticker, _binanceClient.GetCandleSticks(ticker, _request.Interval, from, dt).Result.ToList());
            }
            int count = candlesticks.First().Value.Count();

            for (int i = 0; i < count; i++)
            {
                var CandleSticksByTime = new Dictionary<string, Candlestick>();
                foreach (var kvp in candlesticks)
                {
                    CandleSticksByTime.Add(kvp.Key, kvp.Value[i]);
                }
                CandleSticks.Enqueue(CandleSticksByTime);
            }
        }
    }
}
