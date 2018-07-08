using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;

namespace Ankur.Trading.Core
{
    public class TechnicalAnalysis
    {
        private BinanceClient binanceClient;
        private int NumberOfCandleSticks = 100;

        public Dictionary<string,TradingPairInfo> Pairs = new Dictionary<string, TradingPairInfo>();

        public TechnicalAnalysis(BinanceClient binanceClient)
        {
            this.binanceClient = binanceClient;
        } 

        public void AddTradingPair(string pair, TimeInterval interval)
        {
            //var endTime = GetEndDate(DateTime.Now.Date, interval, NumberOfCandleSticks);
            var candleSticks = binanceClient.GetCandleSticks(pair, interval, null,null,NumberOfCandleSticks).Result.Reverse();
            Pairs.Add(pair,new TradingPairInfo(pair,interval,candleSticks));
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

        

        public object GetTradingOpportunities(string buy)
        {
            throw new NotImplementedException();
        }
    }
}
