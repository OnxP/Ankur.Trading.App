using Ankur.Trading.Core.Request;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ankur.Trading.MarketData
{
    //Connect to Binance and load Market Data, to be stored in the database.
    public class MarketData
    {
        IRequest _request;
        

        private BinanceClient _binanceClient = new BinanceClient(new ApiClient(ConfigurationManager.AppSettings["ApiKey"],
            ConfigurationManager.AppSettings["ApiSecret"]), false);

        public bool CandleSticksLoaded { get; private set; }
        public bool IsLastCandleStick { get; private set; }
        private Queue<Dictionary<string, Candlestick>> CandleSticks { get; set; }

        public MarketData()
        {
            //build database schema.
        }

        private async void load()
        {
            Task<bool> task = LoadCandleSticks();
            task.Start();
            //loop through each candle and apply the algorthm to determine buy and sell oppertunities

            while (ProcessCandleSticks(GetNextCandleSticks()))
            {

            }

            await task;

        }

        private bool ProcessCandleSticks(IDictionary<string, Candlestick> dictionary)
        {
            //store candle stick in the database.
            

            return !IsLastCandleStick;
        }

        private Task<bool> LoadCandleSticks()
        {
            return new Task<bool>(LoadAllCandleSticks);
        }

        private bool LoadAllCandleSticks()
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

        private IDictionary<string, Candlestick> GetNextCandleSticks()
        {
            while (CandleSticks.Count == 0 && !CandleSticksLoaded)
            {
                //Wait 1 second.
                Thread.Sleep(1000);
            }
            var candlesticks = CandleSticks.Dequeue();
            var closeDT = _request.To.AddMilliseconds(-1);
            var dt = candlesticks.First().Value.CloseDateTime;
            if (CandleSticks.Count == 0 && CandleSticksLoaded) IsLastCandleStick = true;
            return candlesticks;
        }

        private IEnumerable<DateTime> SplitDates(TimeInterval interval, DateTime from, DateTime to)
        {
            switch (interval)
            {
                case TimeInterval.Minutes_3:
                case TimeInterval.Minutes_5:
                case TimeInterval.Minutes_15:
                case TimeInterval.Minutes_30:
                    return SplitByDay(from, to);
                case TimeInterval.Hours_1:
                case TimeInterval.Hours_2:
                case TimeInterval.Hours_4:
                case TimeInterval.Hours_6:
                case TimeInterval.Hours_8:
                case TimeInterval.Hours_12:
                    return SplitByWeek(from, to);
                default:
                    return new List<DateTime> { to };
            }
        }

        private IEnumerable<DateTime> SplitByWeek(DateTime from, DateTime to)
        {
            var daysdiff = (to - from).TotalDays;
            for (int i = 0; i < daysdiff; i++)
            {
                yield return from.AddDays(i);
            }
        }

        private IEnumerable<DateTime> SplitByDay(DateTime from, DateTime to)
        {
            var daysdiff = (to - from).TotalDays;
            for (int i = 1; i <= daysdiff; i++)
            {
                yield return from.AddDays(i);
            }
        }
    }
}
