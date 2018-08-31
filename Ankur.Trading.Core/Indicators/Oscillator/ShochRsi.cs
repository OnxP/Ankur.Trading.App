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

        public StochRsi(IEnumerable<Candlestick> candleSticks, int rsiLength, int shochRsiLength, int smoothK, int smoothD)
        {
            this._candleSticks = candleSticks;
            this._rsiLength = rsiLength;
            this._shochRsiLength = shochRsiLength;
            this._smoothK = smoothK;
            this._smoothD = smoothD;
            
            
            CalculateShochRsi(candleSticks);
            
            
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
            rsi = new Rsi(candleSticks, _rsiLength);
            var rsiList = rsi.rsi.ToList();
            var list = new List<decimal>();        
            var candlesticks = _candleSticks.Skip(_rsiLength + _shochRsiLength).ToList();
            int i = 0;
            foreach (var candlestick in candlesticks)
            {
                var rsiLenList = rsiList.Skip(i++).Take(_shochRsiLength).ToList();
                var maxRsi = rsiLenList.Max();
                var minRsi = rsiLenList.Min();
                var shochrsi = (rsiLenList.Last() - minRsi) / (maxRsi - minRsi);
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
            var maxRsi = rsi.rsi.Take(_rsiLength).Max();
            var minRsi = rsi.rsi.Take(_rsiLength).Min();
            var shochrsi = (rsi.Value- minRsi) / (maxRsi - minRsi);

            sRsiList.Add(shochrsi);
            sRsiList.AddRange(stochRsi.Take(100));
            stochRsi = sRsiList;
            CalculateKandD();
        }
    }
}
