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
        private Broker.Broker broker { get; set; }
        private Position BtcPosition => CurrentPosition.First(c => c.Ticker == "btc");
        public decimal CurrentBtcAmount => BtcPosition.Quantity;

        public event LogHandler.LogHandlerDelegate Log;

        public TradingStrategy(BinanceClient binanceClient, IRequest request)
        {
            broker = new Broker.Broker(binanceClient,request);
            _request = request;
            CurrentPosition = new List<Position>();
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
                if (!pos.Open)
                {
                    if (result.Action == TradeAction.Buy) CreateTrade(result);
                    continue;
                }

                //check stop loss against the candle's low so moves through the stop inside the candle are caught.
                //a gap below the stop fills at the open, otherwise the stop price is hit.
                var low = result.Candlestick?.Low ?? result.LastPrice;
                var open = result.Candlestick?.Open ?? result.LastPrice;
                if (low <= pos.StopPrice)
                {
                    CloseTrade(result, Math.Min(open, pos.StopPrice));
                    continue;
                }

                //move stop loss to 10% above the buy price once the position is 15% in profit.
                if (result.LastPrice > pos.BoughtPrice * 1.15m && !pos.InProfit)
                {
                    pos.StopPrice = pos.BoughtPrice * 1.10m;
                    pos.InProfit = true;
                }

                //trail the stop up 1% at a time while price is more than 2% above it.
                if (pos.InProfit && result.LastPrice > pos.StopPrice * 1.02m) pos.StopPrice *= 1.01m;

                if (result.Action == TradeAction.Sell) CloseTrade(result, result.LastPrice);
            }
        }

        public IEnumerable<ITradingResult> TradingResults()
        {
            //buy and sell legs are stored next to each other in the list, an unmatched trailing leg is ignored.
            var list = new List<ITradingResult>();
            foreach (var position in CurrentPosition.Where(x=>x.Ticker!="btc"))
            {
                var trades = position.Trades;
                for (int i = 0; i + 1 < trades.Count; i += 2)
                {
                    list.Add(new TradingResult(trades[i], trades[i + 1]));
                }
            }
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
                var transactionPair = broker.MakeTransaction(TradeAction.Buy, results.Ticker, _request.TradingAmount, results.LastPrice);
                var pos = CurrentPosition.First(x => x.Ticker == ticker);
                pos.Add(transactionPair.First());
                pos.StopPrice = results.LastPrice * 0.97m;
                pos.BoughtPrice = results.LastPrice;
                pos.InProfit = false;
                BtcPosition.Add(transactionPair.Last());
                Log?.Invoke(new TradingLog(transactionPair,results.CloseDateTime));
            }
        }

        //always a sell: the stop loss and the algorithm's sell signal both exit the whole position.
        private void CloseTrade(IAlgorthmResults results, decimal price)
        {
            var ticker = results.Ticker.Replace("btc", "");
            var position = CurrentPosition.First(x => x.Ticker == ticker);
            if (position.Open)
            {
                var transactionPair = broker.MakeTransaction(TradeAction.Sell, results.Ticker, position.Quantity, price);
                position.Add(transactionPair.First());
                BtcPosition.Add(transactionPair.Last());
                Log?.Invoke(new TradingLog(transactionPair, results.CloseDateTime));
            }
        }

        public void ClosePositions(IDictionary<string, decimal> lastPrices)
        {
            foreach (Position result in CurrentPosition.Where(x => x.Open && x.Ticker != "btc"))
            {
                IEnumerable<Trade> transactionPair;
                transactionPair = broker.MakeTransaction(TradeAction.Sell, result.Ticker, result.Quantity, result.Ticker.Contains("usd") ? lastPrices["btc" + result.Ticker] : lastPrices[result.Ticker + "btc"]);

                result.Add(transactionPair.First());
                BtcPosition.Add(transactionPair.Last());
            }
        }
    }
}
