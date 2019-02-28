using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.API.Csharp.Client.Models.Market;
using Ankur.Trading.Core.Oscillator;

namespace Ankur.Trading.Core.Indicators.Oscillator
{
    public class StochRsi : IIndicator
    {
        public IEnumerable<decimal> stochRsi;
        private IEnumerable<Candlestick> _candleSticks;
        private int _rsiLength;
        private int _shochRsiLength;
        private int _smoothK;
        private int _smoothD;
        public decimal KValue => K.Value;
        public decimal DValue => D.Value;
        public decimal Value => K.Value - D.Value;
        private Rsi rsi;
        private Sma K;
        private Sma D;

        public StochRsi(IEnumerable<Candlestick> candleSticks, int rsiLength, int shochRsiLength, int smoothK,
            int smoothD, string ticker)
        {
            this._candleSticks = candleSticks;
            this._rsiLength = rsiLength;
            this._shochRsiLength = shochRsiLength;
            this._smoothK = smoothK;
            this._smoothD = smoothD;

            rsi = new Rsi(candleSticks, _rsiLength, "");
            var candlesticks = _candleSticks.Skip(_rsiLength + _shochRsiLength - 1).ToList();
            CalculateShochRsi(candlesticks);
        }

        public StochRsi(IEnumerable<Candlestick> candleSticks, int rsiLength, int shochRsiLength, int smoothK,
            int smoothD, string ticker, IEnumerable<Candlestick> RSI)
        {
            this._candleSticks = candleSticks;
            this._rsiLength = rsiLength;
            this._shochRsiLength = shochRsiLength;
            this._smoothK = smoothK;
            this._smoothD = smoothD;

            rsi = new Rsi(candleSticks, _rsiLength, "");
            rsi.rsi = RSI.Select(x => x.Close);
            var candlesticks = _candleSticks.Skip(_shochRsiLength - 1).ToList();
            CalculateShochRsi(candlesticks);
        }

        private void CalculateKandD()
        {
            if (K == null)
            {
                K = new Sma(stochRsi.Select(x => x * 100), _smoothK);
                D = new Sma(K.sma, _smoothD);
            }
            else
            {
                K.AddCandleStick(new Candlestick { Close = stochRsi.First() * 100 });
                D.AddCandleStick(new Candlestick { Close = K.Value });
            }
        }

        private void CalculateShochRsi(IEnumerable<Candlestick> candleSticks)
        {
            
            var rsiList = rsi.rsi.ToList();
            var list = new List<decimal>();        
            
            int i = 0;
            foreach (var candlestick in candleSticks)
            {
                var rsiLenList = rsiList.Skip(i++).Take(_shochRsiLength).ToList();
                var maxRsi = rsiLenList.Max();
                var minRsi = rsiLenList.Min();
                decimal shochrsi;
                if (maxRsi - minRsi != 0)
                    shochrsi = (rsiLenList.Last() - minRsi) / (maxRsi - minRsi);
                else
                    shochrsi = 0;
                list.Add(shochrsi);
            }
            stochRsi = list;
            stochRsi.Reverse();
            CalculateKandD();
        }

        public void AddCandleStick(Candlestick futureCandleStick)
        {
            var list = new List<Candlestick>();
            list.Add(futureCandleStick);
            list.AddRange(_candleSticks.Take(100));
            _candleSticks = list;
            rsi.AddCandleStick(futureCandleStick);
            var sRsiList = new List<decimal>();
            var rsiLenList = rsi.rsi.Take(_shochRsiLength).ToList();
            var maxRsi = rsiLenList.Max();
            var minRsi = rsiLenList.Min();
            decimal shochrsi;
            if (maxRsi - minRsi != 0)
                shochrsi = (rsi.Value - minRsi) / (maxRsi - minRsi);
            else
                shochrsi = 0;

            sRsiList.Add(shochrsi);
            sRsiList.AddRange(stochRsi.Take(100));
            stochRsi = sRsiList;
            CalculateKandD();
        }
    }
}
