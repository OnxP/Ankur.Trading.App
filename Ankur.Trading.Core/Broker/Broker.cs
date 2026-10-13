using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.BackTest;
using Ankur.Trading.Core.Trades;
using Ankur.Trading.Core.Trading_Algorthm;
using Binance.API.Csharp.Client;
using Ankur.Trading.Core.Request;
using Binance.API.Csharp.Client.Models.Market;
using Binance.API.Csharp.Client.Models.Enums;

namespace Ankur.Trading.Core.Broker
{
    public class Broker
    {
        private BinanceClient binanceClient;
        private IRequest request;

        private bool TestTrade => request.TestTrade;

        public Broker(BinanceClient binanceClient, IRequest request)
        {
            this.binanceClient = binanceClient;
            this.request = request;
        }

        //return the breakdown of the transaction into individual legs.
        //a buy's quantity is the BTC to spend, a sell's quantity is the coins to sell.
        internal IEnumerable<Trade> MakeTransaction(TradeAction action,string ticker,decimal quantity,decimal lastPrice = 0m)
        {
            var isBuy = action == TradeAction.Buy;
            var btcQuantity = isBuy ? -quantity : CalculateBtcQuantity(lastPrice,quantity);
            var localQuantity = isBuy ? CalculateQuantity(lastPrice, quantity) : -quantity;
            var list = new List<Trade>();
            if (TestTrade)
            {
                //can't get the latest price for back testing, need to use the close price from the last candle
                //paired transaction
                var trade = new Trade(ticker, localQuantity);
                var btcTrade = new Trade("btc", btcQuantity);
                trade.CounterTrade = btcTrade;
                list.Add(trade);
                //BTC transaction
                list.Add(btcTrade);
            }
            else
            {   //goes to binance and places a market order, which is always sized in coins.
                var orderQuantity = isBuy ? localQuantity : -localQuantity;
                var newOrder = binanceClient.PostNewOrder(ticker, orderQuantity, 0m, isBuy ? OrderSide.BUY : OrderSide.SELL, OrderType.MARKET).Result;
                var order = binanceClient.GetOrder(ticker, newOrder.OrderId).Result;

                //binance only reports the executed coins, so the BTC leg is estimated at the last price.
                var coins = isBuy ? order.ExecutedQty : -order.ExecutedQty;
                var trade = new Trade(ticker, coins);
                var btcTrade = new Trade("btc", Math.Round(-coins * lastPrice, 6));
                trade.CounterTrade = btcTrade;
                list.Add(trade);
                list.Add(btcTrade);
            }
            return list;
        }

        private decimal CalculateBtcQuantity(decimal currentPrice, decimal quantity)
        {
            return Math.Round(quantity * currentPrice, 6);
        }

        private decimal CalculateQuantity(decimal currentPrice, decimal quantity)
        {
            return Math.Round(quantity / currentPrice,6);
        }
    }
}
