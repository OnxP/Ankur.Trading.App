using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.API.Csharp.Client.Models.Market;
using Ankur.Trading.Core.Oscillator;

namespace Ankur.Trading.Core.Indicators.Oscillator
{
    public class ShochRsi
    {
        public IEnumerable<decimal> shochRsi;
        private IEnumerable<Candlestick> _candleSticks;
        private int _rsiLength;
        private int _shochRsiLength;
        private int _smoothK;
        private int _smoothD;
        public decimal KValue => K.SmaValue;
        public decimal DValue => D.SmaValue;
        private Rsi rsi;
        private Sma K;
        private Sma D;

        public ShochRsi(IEnumerable<Candlestick> candleSticks, int rsiLength, int shochRsiLength, int smoothK, int smoothD)
        {
            this._candleSticks = candleSticks;
            this._rsiLength = rsiLength;
            this._shochRsiLength = shochRsiLength;
            this._smoothK = smoothK;
            this._smoothD = smoothD;
            rsi = new Rsi(candleSticks, _rsiLength);
            
            CalculateShochRsi();
            K = new Sma(rsi.rsi.Select(x => x * 100), _smoothK);
            D = new Sma(K.sma, _smoothD);
        }

        private void CalculateShochRsi()
        {
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
            shochRsi = list;
        }

        
    }
}
