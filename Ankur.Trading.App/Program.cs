using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market.TradingRules;
using Binance.API.Csharp.Client.Models.WebSocket;
using System.Windows;
using Ankur.Trading.Core.TradingAlgorthm;

namespace Ankur.Trading.App
{
    class Program
    {
        static void Main(string[] args)
        {
            //parameters
            int MaxNoTrades = 0; // Max number of concurrent trades
            int MaxTradeNo = 1; //Max number of trades to execute before exiting
            int PollTime = 1; // How often to poll for price updates.
            decimal TradeAmount = 0.001m; //trading amount in BTC.
            TradingAlgorthm TradingAlgorthm = TradingAlgorthm.SimpleSMA;

            BinanceClient binanceClient = new BinanceClient(new ApiClient(ConfigurationManager.AppSettings["ApiKey"],
                ConfigurationManager.AppSettings["ApiSecret"]), false);

          

            //binanceClient.ListenKlineEndpoint("ethbtc", TimeInterval.Minutes_1, KlineHandler);

            //get trading pairs, filter for BTC. Order by Volume if possible.
            var technicalAnalysis = new TechnicalAnalysis(binanceClient, TradingAlgorthm.SimpleSMA);
            technicalAnalysis.AddTradingPair("eosbtc", TimeInterval.Minutes_1);
            var results = technicalAnalysis.GetTradingOpportunities("Buy");
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
            Console.ReadKey();
        }
        private static void KlineHandler(KlineMessage messageData)
        {
            var klineData = messageData;
        }
    }
    //back testing...monitor a single pair and look for trading opertunities.
    //get 200 candles load 100 into the system then do a minute by minute replay.
    
}
