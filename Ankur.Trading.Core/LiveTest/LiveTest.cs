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
            technicalAnalysis = new TechnicalAnalysis(BinanceClient, _request);
            tradingStrategy = new TradingStrategy(BinanceClient, _request);
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
            // get current server time.
            var time = BinanceClient.GetServerTime().Result.ServerTime;
            var currentServerTime = DateTimeOffset.FromUnixTimeMilliseconds(time).ToLocalTime();
            var diff = DateTime.Now - currentServerTime;
            var lastCandle = DateTime.Now + diff;
            //depending on the interval get candlesticks, 1sec after current candle stick has completed
            while (true)
            {
                var nextCandleStickCloseTime = GetNextCandleSticksCloseTime(lastCandle, _request.Interval);

                while (DateTime.Now.Ticks <= nextCandleStickCloseTime.Ticks)
                {
                    //wait until the next candlestick has closed.
                    Thread.Sleep(1000);
                }

                lastCandle = LoadNextCandleSticks();
            }
        }

        private DateTime LoadNextCandleSticks()
        {
            var candlesticks = new Dictionary<string, List<Candlestick>>();
            foreach (var ticker in _request.TradingPairs)
            {
                candlesticks.Add(ticker, BinanceClient.GetCandleSticks(ticker, _request.Interval,null,null, 1).Result.ToList());
            }
            int count = candlesticks.First().Value.Count();
            var lastCandle = candlesticks.First().Value[0].CloseDateTime;
            for (int i = 0; i < count; i++)
            {
                var CandleSticksByTime = new Dictionary<string, Candlestick>();
                foreach (var kvp in candlesticks)
                {
                    CandleSticksByTime.Add(kvp.Key, kvp.Value[i]);
                }
                CandleSticks.Enqueue(CandleSticksByTime);
            }

            return lastCandle;
        }

        private DateTime GetNextCandleSticksCloseTime(DateTime lastCandle, TimeInterval requestInterval)
        {
            switch (requestInterval)
            {
                case TimeInterval.Minutes_1:
                    return lastCandle.AddMinutes(1);
                case TimeInterval.Minutes_3:
                    return lastCandle.AddMinutes(3);
                case TimeInterval.Minutes_5:
                    return lastCandle.AddMinutes(5);
                case TimeInterval.Minutes_15:
                    return lastCandle.AddMinutes(15);
                case TimeInterval.Minutes_30:
                    return lastCandle.AddMinutes(30);
                case TimeInterval.Hours_1:
                    return lastCandle.AddHours(1);
                case TimeInterval.Hours_2:
                    return lastCandle.AddHours(2);
                case TimeInterval.Hours_4:
                    return lastCandle.AddHours(4);
                case TimeInterval.Hours_6:
                    return lastCandle.AddHours(6);
                case TimeInterval.Hours_8:
                    return lastCandle.AddHours(8);
                case TimeInterval.Hours_12:
                    return lastCandle.AddHours(12);
                case TimeInterval.Days_1:
                    return lastCandle.AddDays(1);
                case TimeInterval.Days_3:
                    return lastCandle.AddDays(3);
                case TimeInterval.Weeks_1:
                    return lastCandle.AddDays(7);
                case TimeInterval.Months_1:
                    return lastCandle.AddMonths(1);
                default:
                    return new DateTime();

            }
        }

        private DateTime LoadCandleSticksPerTicker(DateTime from, DateTime dt)
        {
            var candlesticks = new Dictionary<string, List<Candlestick>>();
            foreach (var ticker in _request.TradingPairs)
            {
                candlesticks.Add(ticker, BinanceClient.GetCandleSticks(ticker, _request.Interval, from, dt,10).Result.ToList());
            }
            int count = candlesticks.First().Value.Count();
            var lastCandle = candlesticks.First().Value[0].CloseDateTime;
            for (int i = 0; i < count; i++)
            {
                var CandleSticksByTime = new Dictionary<string, Candlestick>();
                foreach (var kvp in candlesticks)
                {
                    CandleSticksByTime.Add(kvp.Key, kvp.Value[i]);
                }
                CandleSticks.Enqueue(CandleSticksByTime);
            }

            return lastCandle;
        }
    }
}