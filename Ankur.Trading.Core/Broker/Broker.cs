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
        //quantity is always in BTC.
        internal IEnumerable<Trade> MakeTransaction(TradeAction action,string ticker,decimal quantity,decimal lastPrice = 0m)
        {
            var btcQuantity = action == TradeAction.Buy ? -quantity : CalculateBtcQuantity(lastPrice,quantity);
            var localQuantity = action == TradeAction.Buy ? CalculateQuantity(lastPrice, quantity) : -quantity;
            var list = new List<Trade>();
            if (TestTrade)
            {
                //can't get the latest price for back testing, need to use the close price from the last candle
                //list.Add(new Trade(ticker, lastPrice, action == TradeAction.Buy ? CalculateQuantity(lastPrice, quantity) : -quantity,action));
                //paired transaction
                list.Add(new Trade(ticker, localQuantity));
                //BTC transaction
                list.Add(new Trade("btc", btcQuantity));
            }
            else
            {   //goes to binance and places the transaction.
                var newOrder = binanceClient.PostNewOrder(ticker, quantity, 0m, action == TradeAction.Buy ? OrderSide.BUY : OrderSide.SELL, OrderType.MARKET).Result;
                var Order = binanceClient.GetOrder(ticker, newOrder.OrderId).Result;
                list.Add( new Trade(Order,action));
            }
            return list;
        }

        private decimal CalculateBtcQuantity(decimal currentPrice, decimal quantity)
        {
            return Math.Round(quantity * currentPrice, 2);
        }

        private decimal CalculateQuantity(decimal currentPrice, decimal quantity)
        {
            return Math.Round(quantity / currentPrice,2);
        }
    }
}
