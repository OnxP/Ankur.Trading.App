using System.Collections.Generic;
using Ankur.Trading.Core.BackTest;
using Ankur.Trading.Core.Request;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core.Trading_Algorthm
{
    public class Algorthm
    {
        private IRequest _request;
        private List<Candlestick> list;

        public Algorthm(IRequest request, List<Candlestick> list)
        {
            _request = request;
            this.list = list;
        }

        public AlgorthmResults Evaulate(TradingPairInfo tradingPairInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}
