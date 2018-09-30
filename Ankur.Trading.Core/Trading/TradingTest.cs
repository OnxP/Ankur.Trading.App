using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ankur.Trading.Core.Log;
using Ankur.Trading.Core.Request;
using Ankur.Trading.Core.Trading_Algorthm;
using Ankur.Trading.Core.Trading_Strategy;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core.Trading
{
    public abstract class TradingTest
    {
        public IRequest _request;
        internal ITechnicalAnalysis technicalAnalysis;
        internal ITradingStrategy tradingStrategy;

        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }

        //used to close positions at the end of backtesting.
        protected Dictionary<string,decimal> LastPrices { get; set; }
        protected Queue<Dictionary<string,Candlestick>> CandleSticks { get; set; }
        protected bool IsLastCandleStick { get; set; }
        protected bool CandleSticksLoaded { get; set; }

        public event LogHandler.LogHandlerDelegate Log
        {
            add => tradingStrategy.Log += value;
            remove => tradingStrategy.Log -= value;
        }

        protected BinanceClient _binanceClient = new BinanceClient(new ApiClient(ConfigurationManager.AppSettings["ApiKey"],
            ConfigurationManager.AppSettings["ApiSecret"]), false);

        protected TradingTest(IRequest request)
        {
            this._request = request;
            LastPrices = new Dictionary<string, decimal>();
            CandleSticks = new Queue<Dictionary<string, Candlestick>>();
            IsLastCandleStick = false;
            foreach (var item in _request.TradingPairs)
            {
                LastPrices.Add(item, 0m);
            }
        }

        public IDictionary<string, Candlestick> GetNextCandleSticks()
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

        public async void StartTrading()
        {
            StartTime = _request.From;
            //First Load data 100 candles starting from the FROM date.

            //get the candles for the from and to dates
            Task<bool> task = LoadCandleSticks();
            task.Start();
            //loop through each candle and apply the algorthm to determine buy and sell opportunities

            while (ProcessCandleSticks(GetNextCandleSticks())) {

            }

            await task;
            
        }

        private Task<bool> LoadCandleSticks()
        {
            return new Task<bool>(LoadAllCandleSticks);
        }

        public abstract bool LoadAllCandleSticks();
        
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

        protected IEnumerable<DateTime> SplitDates(TimeInterval interval, DateTime from, DateTime to)
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

        protected IEnumerable<DateTime> SplitByWeek(DateTime from, DateTime to)
        {
            var daysdiff = (to - from).TotalDays;
            for (int i = 0; i < daysdiff; i++)
            {
                yield return from.AddDays(i);
            }
        }

        protected IEnumerable<DateTime> SplitByDay(DateTime from, DateTime to)
        {
            var daysdiff = (to - from).TotalDays;
            for (int i = 1; i <= daysdiff; i++)
            {
                yield return from.AddDays(i);
            }
        }
    }
}
