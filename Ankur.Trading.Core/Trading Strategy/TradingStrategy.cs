using Ankur.Trading.Core.BackTest;
using Ankur.Trading.Core.Trades;
using Ankur.Trading.Core.Trading_Algorthm;
using Binance.API.Csharp.Client;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Ankur.Trading.Core.Trading_Strategy
{
    //This class should process the results and make a trade according to the rules.
    //It has to keep track of open positions
    //Needs to get the latest price to determine if the results are accurate
    //- in a large system with lots of trading pairs there could be a delay from the processing the algorthm
    //  and actually making the trade, in addition the price can move enough during this processing time to
    //  invalidate the results of the algorthm.
    //at the moment this class will work with only a single trading pair.
    //base class for other trading stragies.
    public class TradingStrategy
    {
        private BackTestRequest _request;
        private List<Position> CurrentPosition { get; set; }
        private Broker.Broker broker { get; set; }
        private IList<Position> positionHistory { get; set; }
        private Position BtcPosition => CurrentPosition.First(c => c.Ticker == "btc");
        public decimal CurrentBtcAmount => BtcPosition.Quantity;

        public delegate void LogHandler(TradingResult result);

        public event LogHandler Log;

        public TradingStrategy(BinanceClient binanceClient, BackTestRequest request)
        {
            broker = new Broker.Broker(binanceClient,request);
            _request = request;
            CurrentPosition = new List<Position>();
            var btcPosition = new Position("btc",0);
            btcPosition.Add(new Trade("btc", _request.StartAmount));
            CurrentPosition.Add(btcPosition);
            foreach (var ticker in _request.TradingPairs)
            {
                var ccy = ticker.Substring(0, 3);
                if (CurrentPosition.Select(x => x.Ticker == ccy).Count() > 0) continue;
                CurrentPosition.Add(new Position(ccy,0));
            }
            
        }

        public void Process(IEnumerable<AlgorthmResults> analysisResults)
        {
            //Loop though the open positions first to see if they need to be closed.
            foreach (AlgorthmResults result in analysisResults.Where(x => x.Action == TradeAction.Sell))
            {
                SellAction(result);
            }
            //Loop though the remaining Buy Positions. If more than one Buy position exists then additional information is required to decide which of the positions to buy. E.g direction of the longer ema.
            foreach (AlgorthmResults result in analysisResults.Where(x => x.Action == TradeAction.Buy))
            {
                BuyAction(result);
            }
        }

        internal IEnumerable<TradingResult> TradingResults()
        {
            //assume buy and sell pairs are store next to each other in the list.
            var list = new List<TradingResult>();
            //for(int i =1; i<CurrentPosition.Trades.Count;i+=2)
            //{
            //    var buyTrade = CurrentPosition.Trades[i - 1];
            //    var sellTrade = CurrentPosition.Trades[i];

            //    list.Add(new TradingResult(buyTrade,sellTrade));
            //}
            return list;
        }

        public void BuyAction(AlgorthmResults results)
        {
            var ticker = results.ticker.Substring(0, 3);
            var currentPosistion = CurrentPosition.Where(x => x.Ticker == ticker);
            if (currentPosistion.Count() == 0)
            {
                CurrentPosition.Add(new Position(ticker,0));
            }
            if (CurrentPosition.First(x => x.Ticker == ticker).Open) return;
            //check BTC amount.
            if (BtcPosition.Open && BtcPosition.Quantity >= _request.TradingAmount)
            {
                var transactionPair = broker.MakeTransaction(results.Action, results.ticker, _request.TradingAmount, results.LastPrice);
                CurrentPosition.First(x=>x.Ticker == ticker).Add(transactionPair.First());
                BtcPosition.Add(transactionPair.Last());
                Log?.Invoke(new TradingResult(transactionPair));
            }
        }

        public void SellAction(AlgorthmResults results)
        {
            var ticker = results.ticker.Substring(0, 3);
            var position = CurrentPosition.First(x => x.Ticker == ticker);
            if (position.Open)
            {
                var transactionPair = broker.MakeTransaction(results.Action, results.ticker, position.Quantity, results.LastPrice);
                position.Add(transactionPair.First());
                BtcPosition.Add(transactionPair.Last());
                Log?.Invoke(new TradingResult(transactionPair));
            }
        }

        internal void ClosePositions(IDictionary<string, decimal> lastPrices)
        {
            foreach (Position result in CurrentPosition.Where(x => x.Open))
            {
                var transactionPair = broker.MakeTransaction(TradeAction.Sell, result.Ticker, result.Quantity, lastPrices[result.Ticker]);
                result.Add(transactionPair.First());
                BtcPosition.Add(transactionPair.Last());
            }
        }

        public void WaitAction(AlgorthmResults results)
        {

        }
    }
}
