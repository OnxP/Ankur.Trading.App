using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.BackTest;
using Ankur.Trading.Core.LiveTest;
using Ankur.Trading.Core.Log;
using Ankur.Trading.Core.Request;
using Ankur.Trading.Core.Trading_Algorthm;
using Binance.API.Csharp.Client.Models.Enums;

namespace Ankur.Trading.LiveTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            //PARAMS
            LiveTestRequest request = new LiveTestRequest
            {
                TradingPairs = new List<string> { "ethbtc",
                "ltcbtc",
                "bnbbtc",
                "neobtc",
                "bccbtc",
                "gasbtc",
                "wtcbtc",
                "qtumbtc",
                "yoyobtc",
                "zrxbtc",
                "funbtc",
                "snmbtc",
                "iotabtc",
                "xvgbtc",
                "eosbtc",
                "sntbtc",
                "etcbtc",
                "engbtc",
                "zecbtc",
                "dashbtc",
                "icnbtc",
                "btgbtc",
                "trxbtc",
                "xrpbtc",
                "enjbtc",
                "storjbtc",
                "venbtc",
                "kmdbtc",
                "nulsbtc",
                "rdnbtc",
                "xmrbtc",
                "batbtc",
                "arnbtc",
                "gvtbtc",
                "cdtbtc",
                "gxsbtc",
                "poebtc",
                "fuelbtc",
                "manabtc",
                "dgdbtc",
                "adxbtc",
                "cmtbtc",
                "xlmbtc",
                "tnbbtc",
                "gtobtc",
                "icxbtc",
                "ostbtc",
                "elfbtc",
                "aionbtc",
                "edobtc",
                "navbtc",
                "lunbtc",
                "trigbtc",
                "vibebtc",
                "iostbtc",
                "chatbtc",
                "steembtc",
                "nanobtc",
                "viabtc",
                "blzbtc",
                "aebtc",
                "rpxbtc",
                "ncashbtc",
                "poabtc",
                "zilbtc",
                "ontbtc",
                "stormbtc",
                "xembtc",
                "wanbtc",
                "wprbtc",
                "qlcbtc",
                "sysbtc",
                "grsbtc",
                "cloakbtc",
                "gntbtc",
                "loombtc",
                "bcnbtc",
                "repbtc",
                "tusdbtc",
                "zenbtc",
                "skybtc",
                "cvcbtc",
                "thetabtc",
                "iotxbtc",
                "qkcbtc",
                "agibtc",
                "nxsbtc",
                "databtc",
                "scbtc",
                "npxsbtc",
                "keybtc",
                "nasbtc",
                "mftbtc",
                "dentbtc",
                "ardrbtc",
                "hotbtc",
                "vetbtc",
                "dockbtc",
                "polybtc",},


                From = new DateTime(2018, 08, 30),
                To = new DateTime(2018, 08, 30),
                Interval = TimeInterval.Minutes_15,
                Algorthm = TradingAlgorthm.Macd,
                StartAmount = 1m,
                TradingAmount = 0.1m,
                OrderType = OrderType.LIMIT
            };

            //to start back testing, load the first 100 candles to build the relavent indicators
            //then loop though the remianing candles to simulate live trade streaming.
            //the system can then make decisions at which points to buy and sell (simple SMA strategy to start with).
            //once the processing has been completed the result are displayed, each trade with buy price and sell price and percentage profit.
            var backTest = new LiveTest(request);
            backTest.Log += LogTrade;
            //TODO make this process ASYNC.
            Console.WriteLine($"Starting Amount: {request.StartAmount}btc");
            backTest.StartTrading();
            backTest.FinishTrading();
            Console.WriteLine($"StartTime: {backTest.StartTime} FinishTime: {backTest.FinishTime}");

            DisplayTrades(request.TradingResults);

            Console.WriteLine($"BTC Finishing Amount: {request.FinalAmount}btc");
            Console.WriteLine($"Total PNL - {request.TradingResults.Sum(x => x.Pnl)}");
            Console.WriteLine($"Total % profit - {CalculatePercent(request)}");

            Console.ReadKey();
        }

        private static void DisplayTrades(IEnumerable<ITradingResult> tradingResults)
        {
            foreach (var result in tradingResults)
            {
                Console.WriteLine(result.ToString());
            }
        }

        public static decimal CalculatePercent(IRequest request)
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
