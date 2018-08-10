using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.BackTest;
using Ankur.Trading.Core.Trades;
using Ankur.Trading.Core.Trading_Algorthm;
using Binance.API.Csharp.Client;

namespace Ankur.Trading.Core.Broker
{
    public class Broker
    {
        private BinanceClient binanceClient;
        private BackTestRequest request;

        public Broker(BinanceClient binanceClient, BackTestRequest request)
        {
            this.binanceClient = binanceClient;
            this.request = request;
        }

        internal Position MakeTransaction(TradeAction action)
        {
            throw new NotImplementedException();
        }
    }
}
