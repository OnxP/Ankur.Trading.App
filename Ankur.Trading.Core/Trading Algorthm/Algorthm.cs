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
            var result = new AlgorthmResults();
            var sma5 = tradingPairInfo.Sma[5].SmaValue;
            var sma10 = tradingPairInfo.Sma[10].SmaValue;
            var sma40 = tradingPairInfo.Sma[40].SmaValue;
            result.LastPrice = tradingPairInfo.CurrentPrice;

            if(sma5 > sma10)
            {
                result.Action = TradeAction.Buy;
            }
            else
            {
                result.Action = TradeAction.Sell;
            }
            //result.Action= sma5 > sma10 ? TradeAction.Buy : TradeAction.Sell;
            return result;
        }
    }
}
