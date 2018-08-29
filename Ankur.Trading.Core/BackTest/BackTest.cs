using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.Trading_Algorthm;
using Ankur.Trading.Core.Trading_Strategy;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using System.Threading;

namespace Ankur.Trading.Core.BackTest
{
    public class BackTest
    {
        private BackTestRequest _request;
        private TechnicalAnalysis technicalAnalysis;
        private TradingStrategy tradingStrategy;

        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }

        //used to close positions at the end of backtesting.
        private Dictionary<string,decimal> LastPrices { get; set; }
        private Queue<Dictionary<string,Candlestick>> CandleSticks { get; set; }
        private bool IsLastCandleStick { get; set; }
        private bool CandleSticksLoaded { get; set; }
        public event TradingStrategy.LogHandler Log
        {
            add
            {
                tradingStrategy.Log += value;
            }
            remove
            {
                tradingStrategy.Log += value;
            }
        }

        private BinanceClient _binanceClient = new BinanceClient(new ApiClient(ConfigurationManager.AppSettings["ApiKey"],
            ConfigurationManager.AppSettings["ApiSecret"]), false);

        public BackTest(BackTestRequest request)
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

        public async void StartTrading()
        {
            StartTime = _request.From;
            //First Load data 100 candles starting from the FROM date.

            //get the candles for the from and to dates
            Task<bool> task = LoadCandleSticks();
            task.Start();
            //loop through each candle and apply the algorthm to determine buy and sell oppertunities

            while (ProcessCandleSticks(GetNextCandleSticks())) {

            }

            await task;
            
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
            
            for (int i = 0; i < count ; i++)
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

        public bool ProcessCandleSticks(IDictionary<string, Candlestick> candlestick)
        {
            //Process 1 candlestick for each pair.
            technicalAnalysis.AddCandleSticks(candlestick);
            tradingStrategy.Process(technicalAnalysis.RunAlgorthm());
            foreach (var item in candlestick)
            {
                LastPrices[item.Key] = item.Value.Close;
            }
            FinishTime = candlestick.First().Value.CloseDateTime;

            return !IsLastCandleStick;
        }

        public void FinishTrading()
        {
            tradingStrategy.ClosePositions(LastPrices);
            //store the result for any trades that are made

            _request.TradingResults = tradingStrategy.TradingResults();
            _request.FinalAmount = tradingStrategy.CurrentBtcAmount;
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
                    break;
                case TimeInterval.Hours_1:
                case TimeInterval.Hours_2:
                case TimeInterval.Hours_4:
                case TimeInterval.Hours_6:
                case TimeInterval.Hours_8:
                case TimeInterval.Hours_12:
                    return SplitByWeek(from, to);
                    break;
                default:
                    return new List<DateTime> { to };
                    break;
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
