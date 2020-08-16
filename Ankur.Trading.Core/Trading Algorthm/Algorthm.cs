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

        public IAlgorthmResults Evaulate(string ticker, TradingPairInfo tradingPairInfo)
        {
            var result = new AlgorthmResults(ticker);
            result.LastPrice = tradingPairInfo.CurrentPrice;
            result.CloseDateTime = tradingPairInfo.CloseDateTime;
            result.Candlestick = tradingPairInfo._candleSticks.First();
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
                case TradingAlgorthm.Triple:
                    result.Action = Triple(tradingPairInfo);
                    break;
                case TradingAlgorthm.Triple_Multiple:
                    result.Action = Triple_Multiple(tradingPairInfo);
                    break;
                default:
                    result.Action = TradeAction.Sell;
                    break;
            }
           
            //result.Action= sma5 > sma10 ? TradeAction.Buy : TradeAction.Sell;
            return result;
        }

        private TradeAction Triple_Multiple(TradingPairInfo tradingPairInfo)
        {
            try
            {
                var mcad = tradingPairInfo._15Min.macd;
                decimal rsi = tradingPairInfo._15Min.rsi.Value;
                var srsi = tradingPairInfo._15Min.stochRsi;

                //Low rsi, Low SRSI and Low Macd
                if (rsi < 50 && srsi.KValue < 30 && mcad.Value > 0)
                {
                    return Triple_Med(tradingPairInfo,TradeAction.Buy);
                }

                return Triple_Med(tradingPairInfo, TradeAction.Sell);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return TradeAction.Wait;
            }
            //check the long time series for buy positions
            
        }

        private TradeAction Triple_Med(TradingPairInfo tradingPairInfo,TradeAction Long)
        {
            //check the long time series for buy positions
            var mcad = tradingPairInfo._5Min.macd;
            decimal rsi = tradingPairInfo._5Min.rsi.Value;
            var srsi = tradingPairInfo._5Min.stochRsi;

            //Low rsi, Low SRSI and Low Macd
            if (rsi < 50 && srsi.KValue < 30 && mcad.Value > 0 && Long == TradeAction.Buy)
            {
                return Triple_Low(tradingPairInfo, TradeAction.Buy,TradeAction.Buy);
            }

            if (srsi.Value<0 && TradeAction.Sell == Long)
            {
                return Triple_Low(tradingPairInfo, TradeAction.Sell, TradeAction.Sell);
            }

            return TradeAction.Wait;
        }

        private TradeAction Triple_Low(TradingPairInfo tradingPairInfo, TradeAction Long, TradeAction Medium)
        {
            var mcad = tradingPairInfo.macd;
            decimal rsi = tradingPairInfo.rsi.Value;
            var srsi = tradingPairInfo.stochRsi;

            //Low rsi, Low SRSI and Low Macd
            if (rsi < 50 && srsi.Value > 0 && mcad.Value > 0 && Long == TradeAction.Buy && Medium == TradeAction.Buy)
                return TradeAction.Buy;

            if (rsi > 50 && srsi.Value < 0 && Medium == TradeAction.Sell)
                return TradeAction.Sell;

            return TradeAction.Wait;

        }

        private TradeAction Macd(TradingPairInfo tradingPairInfo)
        {
            var mcad = tradingPairInfo.macd;
            decimal rsi = tradingPairInfo.rsi.Value;
            var srsi = tradingPairInfo.stochRsi;
            var Sma5 = tradingPairInfo.Ema[5].Value;
            var Sma20 = tradingPairInfo.Ema[20].Value;
            var Sma100 = tradingPairInfo.Ema[20].Value;
            var Gsma20 = tradingPairInfo.Gsma[20].Value;
            var price = tradingPairInfo.CurrentPrice;

            if (mcad.Value > 0 && rsi < 50 && srsi.Value > 0 && srsi.DValue < 30 && Sma5 > Sma20 && price > Sma20)
                return TradeAction.Buy;

            if (tradingPairInfo.CurrentPrice < Sma5)
                return TradeAction.Sell;

            if (mcad.Value <= 0 && srsi.Value < 0)
                return TradeAction.Sell;

            return TradeAction.Wait;
        }

        private TradeAction Triple(TradingPairInfo tradingPairInfo)
        {
            var mcad = tradingPairInfo.macd;
            decimal rsi = tradingPairInfo.rsi.Value;
            var srsi = tradingPairInfo.stochRsi;

            //Low rsi, Low SRSI and Low Macd
            if (rsi < 50 && srsi.KValue > 30 && mcad.Value>0)
                return TradeAction.Buy;

            if (rsi > 50 && srsi.Value < 0 && srsi.KValue > 70)
                return TradeAction.Sell;

            return TradeAction.Wait;
        }

        private TradeAction Macd(TradingPairInfo tradingPairInfo)
        {
            var mcad = tradingPairInfo.macd;
            var Sma100 = tradingPairInfo.Sma[100].Value;
            var Sma100gradient = tradingPairInfo.Sma[100].averageGradient;
            var currentPrice = tradingPairInfo.CurrentPrice;

            if (mcad.Value > 0 && Sma100 > currentPrice && Sma100gradient > 0)
                return TradeAction.Buy;

            //if (mcad.Value < 0 && Sma100 < currentPrice)
            //    return TradeAction.Sell;

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
            var gsma = tradingPairInfo.Gsma[20].Value;
            var sma10 = tradingPairInfo.Ema[15].Value;
            var sma40 = tradingPairInfo.Ema[80].Value;

            if (sma5 > sma10 && tradingPairInfo.CurrentPrice > sma40)
            {
                return TradeAction.Buy;
            }

            return TradeAction.Wait;
        }
    }
}
