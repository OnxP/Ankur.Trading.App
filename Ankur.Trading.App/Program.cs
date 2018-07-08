using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;

namespace Ankur.Trading.App
{
    class Program
    {
        static void Main(string[] args)
        {
            ApiClient apiClient = new ApiClient(ConfigurationManager.AppSettings["ApiKey"],
                ConfigurationManager.AppSettings["ApiSecret"]);
            BinanceClient binanceClient = new BinanceClient(apiClient, false);
            var technicalAnalysis = new TechnicalAnalysis(binanceClient);
            technicalAnalysis.AddTradingPair("ethbtc", TimeInterval.Minutes_1);
            var results = technicalAnalysis.GetTradingOpportunities("Buy");
            Console.WriteLine(results.ToString());
        }
    }
    
}
