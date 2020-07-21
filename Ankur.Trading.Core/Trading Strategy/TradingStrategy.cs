using Ankur.Trading.Core.BackTest;
using Ankur.Trading.Core.Trades;
using Ankur.Trading.Core.Trading_Algorthm;
using Binance.API.Csharp.Client;
using System.Collections.Generic;
using System;
using System.Linq;
using Ankur.Trading.Core.Log;
using Ankur.Trading.Core.Request;

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
    public class TradingStrategy : ITradingStrategy
    {
        private IRequest _request;
        private List<Position> CurrentPosition { get; set; }
        public List<Trades.Trades> Trades { get; set; }
        private Broker.Broker broker { get; set; }
        private IList<Position> positionHistory { get; set; }
        private Position BtcPosition => CurrentPosition.First(c => c.Ticker == "btc");
        public decimal CurrentBtcAmount => BtcPosition.Quantity;

        public event LogHandler.LogHandlerDelegate Log;

        public TradingStrategy(BinanceClient binanceClient, IRequest request)
        {
            broker = new Broker.Broker(binanceClient,request);
            _request = request;
            CurrentPosition = new List<Position>();
            Trades = new List<Trades.Trades>();
            var btcPosition = new Position("btc",0);
            btcPosition.Add(new Trade("btc", _request.StartAmount));
            CurrentPosition.Add(btcPosition);
            foreach (var ticker in _request.TradingPairs)
            {
                var ccy = ticker.Replace("btc", "");
                var pos = CurrentPosition.Where(x => x.Ticker == ccy);
                if (pos.Count() > 0) continue;
                CurrentPosition.Add(new Position(ccy,0));
            }
            
        }

        public void Process(IEnumerable<IAlgorthmResults> analysisResults)
        {
            foreach (IAlgorthmResults result in analysisResults)
            {
                var pos = CurrentPosition.First(x => x.Ticker == result.Ticker.Replace("btc", ""));
                if (!pos.Open && result.Action == TradeAction.Buy)
                {
                    CreateTrade(result);
                    continue;
                }
                if (!pos.Open) continue;

                //check stop loss.
                if (result.LastPrice <= pos.StopPrice) CloseTrade(result);
                //move stop loss to be 10 % of profit.
                var profictPrice = pos.BoughtPrice * 1.15m;
                var inProfit = pos.InProfit;
                if (result.LastPrice > profictPrice && !inProfit) pos.StopPrice = pos.BoughtPrice * 1.10m;

                if (result.LastPrice > pos.StopPrice * 1.05m && inProfit) pos.StopPrice = pos.StopPrice * 1.04m;
        
                if (result.Action == TradeAction.Sell) CloseTrade(result);

            }
        }

        public IEnumerable<ITradingResult> TradingResults()
        {
            //assume buy and sell pairs are store next to each other in the list.
            var list = new List<ITradingResult>();
            foreach (var position in CurrentPosition.Where(x=>x.Ticker!="btc"))
            {
                var trades = position.Trades.ToList();
                for (int i = 0; i < trades.Count(); i++)
                {
                    try
                    {
                        list.Add(new TradingResult(trades[i], trades[++i]));
                    }
                    catch{}
                } }
                
            //for(int i =1; i<CurrentPosition.Trades.Count;i+=2)
            //{
            //    var buyTrade = CurrentPosition.Trades[i - 1];
            //    var sellTrade = CurrentPosition.Trades[i];

            //    list.Add(new TradingResult(buyTrade,sellTrade));
            //}
            return list;
        }

        public void CreateTrade(IAlgorthmResults results)
        {
            var ticker = results.Ticker.Replace("btc","");
            var currentPosistion = CurrentPosition.Where(x => x.Ticker == ticker);
            if (currentPosistion.Count() == 0)
            {
                CurrentPosition.Add(new Position(ticker,0));
            }
            if (CurrentPosition.First(x => x.Ticker == ticker).Open) return;
            //check BTC amount.
            if (BtcPosition.Open && BtcPosition.Quantity >= _request.TradingAmount)
            {
                var transactionPair = broker.MakeTransaction(results.Action, results.Ticker, _request.TradingAmount, results.LastPrice);
                var pos = CurrentPosition.First(x => x.Ticker == ticker);
                pos.Add(transactionPair.First());
                pos.StopPrice = results.LastPrice * 0.95m;
                pos.BoughtPrice = results.LastPrice;
                BtcPosition.Add(transactionPair.Last());
                Log?.Invoke(new TradingLog(transactionPair,results.CloseDateTime));
            }
        }

        public void CloseTrade(IAlgorthmResults results)
        {
            var ticker = results.Ticker.Replace("btc", "");
            var position = CurrentPosition.First(x => x.Ticker == ticker);
            if (position.Open)
            {
                var transactionPair = broker.MakeTransaction(results.Action, results.Ticker, position.Quantity, results.LastPrice);
                position.Add(transactionPair.First());
                BtcPosition.Add(transactionPair.Last());
                Log?.Invoke(new TradingLog(transactionPair, results.CloseDateTime));
            }
        }

        public void ClosePositions(IDictionary<string, decimal> lastPrices)
        {
            foreach (Position result in CurrentPosition.Where(x => x.Open && x.Ticker != "btc"))
            {
                var transactionPair = broker.MakeTransaction(TradeAction.Sell, result.Ticker, result.Quantity, lastPrices[result.Ticker+"btc"]);
                result.Add(transactionPair.First());
                BtcPosition.Add(transactionPair.Last());
            }
        }

        public void WaitAction(IAlgorthmResults results)
        {

        }
    }
}
