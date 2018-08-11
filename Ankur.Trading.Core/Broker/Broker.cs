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

        internal Trade MakeTransaction(TradeAction action)
        {
            if (TestTrade)
            {
                var currentPrices = binanceClient.GetAllPrices().Result;
                var currentPrice = currentPrices.Where(x => x.Symbol == request.TradingPair).First();
                return new Trade(currentPrice);
            }
            else
            {   //goes to binance and places the transaction.
                var newOrder = binanceClient.PostNewOrder(request.TradingPair, request.TradeAmount, 0m, action == TradeAction.Buy ? Binance.API.Csharp.Client.Models.Enums.OrderSide.BUY : Binance.API.Csharp.Client.Models.Enums.OrderSide.SELL, Binance.API.Csharp.Client.Models.Enums.OrderType.MARKET).Result;
                var Order = binanceClient.GetOrder(request.TradingPair, newOrder.OrderId).Result;
                return new Trade(Order);
            }
        }
    }
}
