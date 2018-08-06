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
            //get the candles for the from and to dates
            var futureCandleSticks = _binanceClient.GetCandleSticks(_request.TradingPair, _request.Interval, _request.From, _request.To).Result.Reverse();
            //loop through each candle and apply the algorthm to determine buy and sell oppertunities
            foreach (Candlestick futureCandleStick in futureCandleSticks)
            {
                technicalAnalysis.AddCandleStick(futureCandleStick);
                var analysisResults = technicalAnalysis.RunAlgorthm();
                tradingStrategy.Process(analysisResults);
            }
            //store the result for any trades that are made

            //_request.TradingResults = technicalAnalysis.TradingResults;
        }
    }
}
