using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.BackTest;
using Ankur.Trading.Core.Request;
using Ankur.Trading.Core.Trading_Algorthm;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core
{
    public class TechnicalAnalysis : ITechnicalAnalysis
    {
        private readonly BinanceClient _binanceClient;
        private IRequest _request;
        private readonly int _numberOfCandleSticks = 100;

        private Algorthm _algorthm;

        public Dictionary<string,TradingPairInfo> Pairs = new Dictionary<string, TradingPairInfo>();

        public TechnicalAnalysis(BinanceClient binanceClient, IRequest request)
        {
            this._binanceClient = binanceClient;
            this._request = request;
            foreach (var item in _request.TradingPairs)
            {
                AddTradingPair(item, _request.Interval, _request.From);
            }
            
        }

        public IEnumerable<TradingResult> TradingResults { get; set; }

        public void AddTradingPair(string pair, TimeInterval interval)
        {
            //var endTime = GetEndDate(DateTime.Now.Date, interval, NumberOfCandleSticks);
            var candleSticks = _binanceClient.GetCandleSticks(pair, interval, null,null,_numberOfCandleSticks).Result;
            Pairs.Add(pair,new TradingPairInfo(pair,interval,candleSticks));
        }

        public void AddTradingPair(string pair, TimeInterval interval,DateTime? endTime)
        {
            //var endTime = GetEndDate(DateTime.Now.Date, interval, NumberOfCandleSticks);
            var candleSticks = _binanceClient.GetCandleSticks(pair, interval, null, endTime, _numberOfCandleSticks).Result;
            Pairs.Add(pair, new TradingPairInfo(pair, interval, candleSticks));
            _algorthm = new Algorthm(_request,candleSticks.ToList());
        }

        private DateTime GetEndDate(DateTime now, TimeInterval interval, int numberOfCandleSticks)
        {
            switch (interval)
            {
                case TimeInterval.Minutes_1:
                    return now.AddDays(-5);
                    break;
                case TimeInterval.Minutes_3:
                    return now.AddDays(-5);
                    break;
                case TimeInterval.Minutes_5:
                    return now.AddDays(-5);
                    break;
                case TimeInterval.Minutes_15:
                    return now.AddDays(-5);
                    break;
                case TimeInterval.Minutes_30:
                    return now.AddDays(-5);
                    break;
                case TimeInterval.Hours_1:
                    return now.AddDays(-10);
                    break;
                case TimeInterval.Hours_2:
                    return now.AddDays(-10);
                    break;
                case TimeInterval.Hours_4:
                    return now.AddDays(-50);
                    break;
                case TimeInterval.Hours_6:
                    return now.AddDays(-50);
                    break;
                case TimeInterval.Hours_8:
                    return now.AddDays(-50);
                    break;
                case TimeInterval.Hours_12:
                    return now.AddDays(-50);
                    break;
                case TimeInterval.Days_1:
                    return now.AddDays(-100);
                    break;
                case TimeInterval.Days_3:
                    return now.AddDays(-300);
                    break;
                case TimeInterval.Weeks_1:
                    return now.AddDays(-700);
                    break;
                case TimeInterval.Months_1:
                    return now.AddMonths(-100);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(interval), interval, null);
            }
        }

        public void AddCandleSticks(IDictionary<string, Candlestick> candlestick)
        {
            foreach (var kvp in candlestick)
            {
                AddCandleStick(kvp.Key, kvp.Value);
            }
        }

        public IEnumerable<string> GetTradingOpportunities(string buy)
        {
            foreach (KeyValuePair<string, TradingPairInfo> tradingPairInfo in Pairs)
            {
                var sma5 = tradingPairInfo.Value.Sma[5].Value;
                var sma10 = tradingPairInfo.Value.Sma[10].Value;
                var trade = sma5 > sma10 ? "BUY" : "SELL";
                StringBuilder builder = new StringBuilder();
                builder.Append($"Current Price: {tradingPairInfo.Value.CurrentPrice}");
                builder.Append($" Sma 5-{sma5}");
                builder.Append($" Sma 10-{sma10}");
                builder.Append($" Place Trade: {trade}");
                yield return builder.ToString();
            }
        }

        public IEnumerable<Candlestick> GetData()
        {
            return Pairs.FirstOrDefault().Value._candleSticks;
        }

        public void AddCandleStick(string ticker,Candlestick futureCandleStick)
        {
            Pairs[ticker].Add(futureCandleStick);
        }

        public IEnumerable<IAlgorthmResults> RunAlgorthm()
        {
            var results = new List<IAlgorthmResults>();
            foreach (var kvp in Pairs)
            {
                results.Add(_algorthm.Evaulate(kvp.Key,kvp.Value));
            }
            return results;
            //first run the algorthm to determine if there is a trading opprtunity 
            //produce some actions based on the results of the analysis
            //execute those actions, or check stop limits etc. Trading Stratgy object
            //3 options
            // - make a Buy Trade
            // - Make a Sell Trade
            //Do Nothing
        }
    }
}
