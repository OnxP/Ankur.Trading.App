using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.TradingAlgorthm;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core
{
    public class TechnicalAnalysis
    {
        private readonly BinanceClient _binanceClient;
        private readonly int _numberOfCandleSticks = 100;

        public Dictionary<string,TradingPairInfo> Pairs = new Dictionary<string, TradingPairInfo>();

        public TechnicalAnalysis(BinanceClient binanceClient, TradingAlgorthm.TradingAlgorthm algorthm)
        {
            this._binanceClient = binanceClient;
        }

        public IEnumerable<TradingResult> TradingResults { get; set; }

        public void AddTradingPair(string pair, TimeInterval interval)
        {
            //var endTime = GetEndDate(DateTime.Now.Date, interval, NumberOfCandleSticks);
            var candleSticks = _binanceClient.GetCandleSticks(pair, interval, null,null,_numberOfCandleSticks).Result.Reverse();
            Pairs.Add(pair,new TradingPairInfo(pair,interval,candleSticks.Reverse()));
        }

        public void AddTradingPair(string pair, TimeInterval interval,DateTime? endTime)
        {
            //var endTime = GetEndDate(DateTime.Now.Date, interval, NumberOfCandleSticks);
            var candleSticks = _binanceClient.GetCandleSticks(pair, interval, null, endTime, _numberOfCandleSticks).Result.Reverse();
            Pairs.Add(pair, new TradingPairInfo(pair, interval, candleSticks.Reverse()));
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
        
        public IEnumerable<string> GetTradingOpportunities(string buy)
        {
            foreach (KeyValuePair<string, TradingPairInfo> tradingPairInfo in Pairs)
            {
                var sma5 = tradingPairInfo.Value.Sma[5].SmaValue;
                var sma10 = tradingPairInfo.Value.Sma[10].SmaValue;
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

        public void AddCandleStick(Candlestick futureCandleStick)
        {
            throw new NotImplementedException();
        }
    }
}
