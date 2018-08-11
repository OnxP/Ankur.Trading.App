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

        internal Trade MakeTransaction(TradeAction action,decimal quantity,decimal lastPrice = 0m)
        {
            if (TestTrade)
            {
                //can't get the latest price for back testing, need to use the close price from the last candle
                return new Trade(request.TradingPair,lastPrice, action == TradeAction.Buy ? CalculateQuantity(lastPrice, quantity) : -quantity);
            }
            else
            {   //goes to binance and places the transaction.
                var newOrder = binanceClient.PostNewOrder(request.TradingPair, quantity, 0m, action == TradeAction.Buy ? Binance.API.Csharp.Client.Models.Enums.OrderSide.BUY : Binance.API.Csharp.Client.Models.Enums.OrderSide.SELL, Binance.API.Csharp.Client.Models.Enums.OrderType.MARKET).Result;
                var Order = binanceClient.GetOrder(request.TradingPair, newOrder.OrderId).Result;
                return new Trade(Order);
            }
        }

        private decimal CalculateQuantity(decimal currentPrice, decimal quantity)
        {
            return Math.Round(quantity / currentPrice,2);
        }
    }
}
