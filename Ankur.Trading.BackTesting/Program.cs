using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.BackTest;
using Ankur.Trading.Core.Log;
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
                // 
                TradingPairs = new List<string> { "trxbtc", "sysbtc", "eosbtc", "xrpbtc" },
                From = new DateTime(2020, 01, 21),
                To = new DateTime(2020, 07, 20),
                Interval = TimeInterval.Hours_1,
                Algorthm = TradingAlgorthm.Macd,
                StartAmount = 1m,
                TradingAmount = 0.5m,
                OrderType = OrderType.LIMIT
            };

            //to start back testing, load the first 100 candles to build the relavent indicators
            //then loop though the remianing candles to simulate live trade streaming.
            //the system can then make decisions at which points to buy and sell (simple SMA strategy to start with).
            //once the processing has been completed the result are displayed, each trade with buy price and sell price and percentage profit.
            var backTest = new BackTest(request);
            backTest.Log += LogTrade;
            //TODO make this process ASYNC.
            Console.WriteLine($"Starting Amount: {request.StartAmount}btc");
            backTest.StartTrading();
            backTest.FinishTrading();
            Console.WriteLine($"StartTime: {backTest.StartTime} FinishTime: {backTest.FinishTime}");

            DisplayTrades(request.TradingResults);

            Console.WriteLine($"BTC Finishing Amount: {request.FinalAmount}btc");
            Console.WriteLine($"Total PNL - {request.TradingResults.Sum(x=>x.Pnl)}");
            Console.WriteLine($"Total % profit - {CalculatePercent(request)}");
            Console.WriteLine($"Win/Loss - Total - {CalculateRatio(request)}");


            Console.ReadKey();
        }

        private static void DisplayTrades(IEnumerable<ITradingResult> tradingResults)
        {
            foreach (var result in tradingResults)
            {
                Console.WriteLine(result.ToString());
            }
        }
        public static string CalculateRatio(BackTestRequest request)
        {
            double total = request.TradingResults.Count();
            double win = request.TradingResults.Count(x => x.Pnl > 0);
            double loss = request.TradingResults.Count(x => x.Pnl < 0);

            //var diff = sum - request.StartAmount;
            return $"{win}/{loss} {Math.Round(win/total)*100,0}% - {total}";

        }

        public static decimal CalculatePercent(BackTestRequest request)
        {
            var sum = request.TradingResults.Sum(x => x.Pnl);

            //var diff = sum - request.StartAmount;
            var result = sum / request.StartAmount * 100;
            return Math.Round(result);

        }

        public static void LogTrade(ITradingLog tradingResult)
        {
            Console.WriteLine($"Pair: {tradingResult.Pair} \t Amount: {tradingResult.Quantity} \t BtcAmount: {tradingResult.BtcQuantity} \t Price:{tradingResult.Price} \t CloseDateTime: {tradingResult.CloseTime.ToString()}");
        }
    }
}
