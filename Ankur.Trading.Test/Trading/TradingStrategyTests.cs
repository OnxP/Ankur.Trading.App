using System;
using System.Collections.Generic;
using System.Linq;
using Ankur.Trading.Core.Request;
using Ankur.Trading.Core.Trading_Algorthm;
using Ankur.Trading.Core.Trading_Strategy;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ankur.Trading.Test.Trading_Strategy
{
    [TestClass]
    public class TradingStrategyTests
    {
        //0.25 btc at a price of 0.001 buys 250 coins.
        private const string Pair = "sysbtc";

        private static TradingStrategy BuildStrategy()
        {
            return new TradingStrategy(new BinanceClient(new ApiClient("key", "secret"), false), new FakeRequest());
        }

        private static IAlgorthmResults Candle(TradeAction action, decimal open, decimal low, decimal close)
        {
            return new AlgorthmResults(Pair)
            {
                Action = action,
                LastPrice = close,
                CloseDateTime = DateTime.UtcNow,
                Candlestick = new Candlestick { Open = open, High = Math.Max(open, close), Low = low, Close = close }
            };
        }

        private static void Process(TradingStrategy strategy, IAlgorthmResults result)
        {
            strategy.Process(new[] { result });
        }

        [TestMethod]
        public void BuyThenBuySignalOnStopCandle_ClosesPositionInsteadOfBuying()
        {
            var strategy = BuildStrategy();
            Process(strategy, Candle(TradeAction.Buy, 0.001m, 0.001m, 0.001m));
            Assert.AreEqual(0.75m, strategy.CurrentBtcAmount);

            //stop is 0.00097, this candle pierces it while the algorthm still signals a buy.
            Process(strategy, Candle(TradeAction.Buy, 0.00096m, 0.00090m, 0.00095m));

            //250 coins sold at the 0.00096 open, which is above the stop price.
            Assert.AreEqual(0.99m, strategy.CurrentBtcAmount);
            Assert.AreEqual(1, strategy.TradingResults().Count());
        }

        [TestMethod]
        public void GapBelowStop_FillsAtOpen()
        {
            var strategy = BuildStrategy();
            Process(strategy, Candle(TradeAction.Buy, 0.001m, 0.001m, 0.001m));

            Process(strategy, Candle(TradeAction.Wait, 0.0009m, 0.0008m, 0.00085m));

            //gapped below the 0.00097 stop, so the exit fills at the 0.0009 open: 250 * 0.0009 = 0.225 btc.
            Assert.AreEqual(0.975m, strategy.CurrentBtcAmount);
        }

        [TestMethod]
        public void InProfitFlag_DoesNotCarryIntoNextTrade()
        {
            var strategy = BuildStrategy();

            //first trade reaches 15% profit, which arms the breakeven stop.
            Process(strategy, Candle(TradeAction.Buy, 1m, 1m, 1m));
            Process(strategy, Candle(TradeAction.Wait, 1.2m, 1.2m, 1.2m));
            //algorthm sells, closing the first trade.
            Process(strategy, Candle(TradeAction.Sell, 1.2m, 1.2m, 1.2m));

            //second trade is bought at 1.0 with a fresh 0.97 stop and must not inherit the trailing mode.
            Process(strategy, Candle(TradeAction.Buy, 1m, 1m, 1m));
            Process(strategy, Candle(TradeAction.Wait, 0.99m, 0.99m, 0.995m));
            //close is below the stale 0.9797 stop but the low is above the correct 0.97 stop.
            Process(strategy, Candle(TradeAction.Wait, 0.98m, 0.975m, 0.976m));

            //the stop stayed at 0.97, so the 0.975 low did not exit: only the first trade is closed.
            Assert.AreEqual(1, strategy.TradingResults().Count());
            //first trade nets 0.75 + 0.30 = 1.05 btc, then the second buy takes another 0.25 btc.
            Assert.AreEqual(0.80m, strategy.CurrentBtcAmount);
        }

        [TestMethod]
        public void UnmatchedLegs_AreIgnoredInTradingResults()
        {
            var strategy = BuildStrategy();
            Process(strategy, Candle(TradeAction.Buy, 0.001m, 0.001m, 0.001m));

            Assert.AreEqual(0, strategy.TradingResults().Count());
        }
    }

    //minimal request used to drive the strategy in back test mode, so no orders reach binance.
    internal class FakeRequest : IRequest
    {
        public FakeRequest()
        {
            TradingPairs = new List<string> { Pair };
            StartAmount = 1m;
            TradingAmount = 0.25m;
            TestTrade = true;
            IsTest = true;
            TradingResults = new List<ITradingResult>();
        }

        private const string Pair = "sysbtc";

        public List<string> TradingPairs { get; set; }
        public TimeInterval Interval { get; set; }
        public TradingAlgorthm Algorthm { get; set; }
        public decimal StartAmount { get; set; }
        public OrderType OrderType { get; set; }
        public IEnumerable<ITradingResult> TradingResults { get; set; }
        public bool TestTrade { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool IsTest { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal TradingAmount { get; set; }

        public void Add(ITradingResult result)
        {
            var list = new List<ITradingResult>(TradingResults) { result };
            TradingResults = list;
        }
    }
}
