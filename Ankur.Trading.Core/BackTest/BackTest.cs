using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.Trading_Strategy;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core.BackTest
{
    public class BackTest
    {
        private BackTestRequest _request;
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }

        private BinanceClient _binanceClient = new BinanceClient(new ApiClient(ConfigurationManager.AppSettings["ApiKey"],
            ConfigurationManager.AppSettings["ApiSecret"]), false);

        public BackTest(BackTestRequest request)
        {
            this._request = request;
        }

        public void StartTrading()
        {
            //First Load data 100 candles starting from the FROM date.
            var technicalAnalysis = new TechnicalAnalysis(_binanceClient, _request);
            var tradingStrategy = new TradingStrategy(_binanceClient,_request);
            technicalAnalysis.AddTradingPair(_request.TradingPair, _request.Interval, _request.From);
            var futureCandleSticks = new List<Candlestick>();
            var from = _request.From;
            //get the candles for the from and to dates
            foreach (DateTime dt in SplitDates(_request.Interval, _request.From, _request.To))
            {
                futureCandleSticks.AddRange(_binanceClient.GetCandleSticks(_request.TradingPair, _request.Interval, from, dt).Result);
                from = dt;
            }
            
            StartTime = futureCandleSticks.First().OpenDateTime;
            //loop through each candle and apply the algorthm to determine buy and sell oppertunities
            var lastPrice = 0m;
            foreach (Candlestick futureCandleStick in futureCandleSticks)
            {
                technicalAnalysis.AddCandleStick(futureCandleStick);
                var analysisResults = technicalAnalysis.RunAlgorthm();
                tradingStrategy.Process(analysisResults);
                lastPrice = futureCandleStick.Close;
                FinishTime = futureCandleStick.CloseDateTime;
            }
            tradingStrategy.ClosePosition(lastPrice);
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
