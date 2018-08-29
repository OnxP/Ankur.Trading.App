using System;
using System.Collections.Generic;
using Ankur.Trading.Core.BackTest;
using Ankur.Trading.Core.Request;
using Binance.API.Csharp.Client.Models.Market;
using System.Linq;

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
            result.LastPrice = tradingPairInfo.CurrentPrice;
            result.CloseDateTime = tradingPairInfo.CloseDateTime;
            switch (_request.Algorthm)
            {
                case TradingAlgorthm.SimpleSMA:
                    result.Action = SimpleSMA(tradingPairInfo);
                    break;
                case TradingAlgorthm.LongShochRsi:
                    result.Action = LongShochRsi(tradingPairInfo);
                    break;
                case TradingAlgorthm.Macd:
                    result.Action = Macd(tradingPairInfo);
                    break;
                default:
                    result.Action = TradeAction.Sell;
                    break;
            }
           
            //result.Action= sma5 > sma10 ? TradeAction.Buy : TradeAction.Sell;
            return result;
        }

        private TradeAction Macd(TradingPairInfo tradingPairInfo)
        {
            var mcad = tradingPairInfo.macd;
            decimal rsi = tradingPairInfo.rsi.Value;
            var srsi = tradingPairInfo.stochRsi;
            var greadientSma5 = tradingPairInfo.Ema[5].Gradient;

            if (rsi <= 30 && srsi.KValue <= 25 && mcad.MacdLine.First() < 0) return TradeAction.Buy;

            if (mcad.Value > 0) return TradeAction.Sell;

            return TradeAction.Wait;
        }

        private TradeAction LongShochRsi(TradingPairInfo tradingPairInfo)
        {
            var k = tradingPairInfo.stochRsi.KValue;
            var d = tradingPairInfo.stochRsi.DValue;
            if (k > d)
            {
                return TradeAction.Buy;
            }
            else
            {
                return TradeAction.Sell;
            }
        }

        private TradeAction SimpleSMA(TradingPairInfo tradingPairInfo)
        {
            var sma5 = tradingPairInfo.Ema[5].Value;
            var sma10 = tradingPairInfo.Ema[15].Value;
            var sma40 = tradingPairInfo.Ema[40].Value;

            if (sma5 > sma10)
            {
                return TradeAction.Buy;
            }
            else
            {
                return TradeAction.Sell;
            }
        }
    }
}
