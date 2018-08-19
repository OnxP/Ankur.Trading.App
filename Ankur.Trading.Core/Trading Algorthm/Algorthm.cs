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

        public AlgorthmResults Evaulate(string ticker, TradingPairInfo tradingPairInfo)
        {
            var result = new AlgorthmResults(ticker);
            var sma5 =  tradingPairInfo.Ema[5].EmaValue;
            var sma10 = tradingPairInfo.Ema[15].EmaValue;
            var sma40 = tradingPairInfo.Ema[40].EmaValue;
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
