using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.TradingAlgorthm;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core.BackTest
{
    public class BackTest
    {
        public string TradingPair { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public TimeInterval Interval { get; set; }
        public TradingAlgorthm.TradingAlgorthm Algorthm { get; set; }
        public decimal StartAmount { get; set; }
        //not for simplicity the back test will make market limit orders the entry price.
        public OrderType OrderType { get; set; }
        public IEnumerable<TradingResult> TradingResults { get; set; }

        private BinanceClient _binanceClient = new BinanceClient(new ApiClient(ConfigurationManager.AppSettings["ApiKey"],
            ConfigurationManager.AppSettings["ApiSecret"]), false);

        public void StartTrading()
        {
            //First Load data 100 candles starting from the FROM date.
            var technicalAnalysis = new TechnicalAnalysis(_binanceClient);
            technicalAnalysis.AddTradingPair(TradingPair, Interval, From);
            //get the candles for the from and to dates
            var futureCandleSticks = _binanceClient.GetCandleSticks(TradingPair, Interval, From,To).Result.Reverse();
            //loop through each candle and apply the algorthm to determine buy and sell oppertunities
            foreach (Candlestick futureCandleStick in futureCandleSticks)
            {
                
            }
            //store the result for any trades that are made


        }
    }
}
