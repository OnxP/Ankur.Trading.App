using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.BackTest;
using Ankur.Trading.Core.Trading_Algorthm;
using Binance.API.Csharp.Client.Models.Enums;

namespace Ankur.Trading.BackTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            //PARAMS
            BackTestRequest request = new BackTestRequest
            {
                TradingPair = "eosbtc",
                From = new DateTime(2018, 01, 20),
                To = new DateTime(2018, 07, 20),
                Interval = TimeInterval.Days_1,
                Algorthm = TradingAlgorthm.SimpleSMA,
                StartAmount = 1m,
                OrderType = OrderType.LIMIT
            };

            //to start back testing, load the first 100 candles to build the relavent indicators
            //then loop though the remianing candles to simulate live trade streaming.
            //the system can then make decisions at which points to buy and sell (simple SMA strategy to start with).
            //once the processing has been completed the result are displayed, each trade with buy price and sell price and percentage profit.
            var backTest = new BackTest(request);
            //TODO make this process ASYNC.
            backTest.StartTrading();

            //wait for trading to complete.

            Console.WriteLine($"Starting Amount: {request.StartAmount}btc");
            foreach (TradingResult tradingResult in request.TradingResults)
            {
                Console.WriteLine($"Bought: {tradingResult.Bought} Sold: {tradingResult.Sold} PNL: {tradingResult.Pnl} - {tradingResult.PnlPercent}");
            }

            //total

            Console.WriteLine($"StartTime: {backTest.StartTime} FinishTime: {backTest.FinishTime}");
            Console.WriteLine($"BTC Finishing Amount: {request.FinalAmount}btc");
            Console.WriteLine($"Total PNL - {request.TradingResults.Sum(x=>x.Pnl)}");
            Console.WriteLine($"Total % profit - {CalculatePercent(request)}");

            Console.ReadKey();
        }

        public static decimal CalculatePercent(BackTestRequest request)
        {
            var sum = request.TradingResults.Sum(x => x.Pnl);

            //var diff = sum - request.StartAmount;
            var result = sum / request.StartAmount * 100;
            return Math.Round(result);

        }
    }
}
