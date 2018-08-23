using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Ankur.Trading.Core.Indicators;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using Ankur.Trading.Core.Indicators.Oscillator;

namespace Ankur.Trading.Core
{
    public class TradingPairInfo
    {
        private string pair;
        private TimeInterval interval;
        public IEnumerable<Candlestick> _candleSticks;
        public decimal CurrentPrice => _candleSticks.Last().Close;

        public Dictionary<int,Sma> Sma = new Dictionary<int, Sma>();
        public Dictionary<int,Ema> Ema = new Dictionary<int, Ema>();

        public ShochRsi stochRsi;

        public TradingPairInfo(string pair, TimeInterval interval, IEnumerable<Candlestick> candleSticks)
        {
            this.pair = pair;
            this.interval = interval;
            this._candleSticks = candleSticks;
            BuildIndicators();
        }

        private void BuildIndicators()
        {
            Sma.Add(5,new Sma(_candleSticks,5));
            Sma.Add(10,new Sma(_candleSticks,10));
            Sma.Add(15,new Sma(_candleSticks,15));
            Sma.Add(20,new Sma(_candleSticks,20));
            Sma.Add(40,new Sma(_candleSticks,40));
            Ema.Add(5,new Ema(_candleSticks,5));
            Ema.Add(10,new Ema(_candleSticks,10));
            Ema.Add(15,new Ema(_candleSticks,15));
            Ema.Add(20,new Ema(_candleSticks,20));
            Ema.Add(40,new Ema(_candleSticks,40));

            stochRsi = new ShochRsi(_candleSticks,14,14,8,8);
        }

        public void Add(Candlestick futureCandleStick)
        {
            var candleSticks = _candleSticks.ToList<Candlestick>();
            candleSticks.Add(futureCandleStick);
            foreach (KeyValuePair<int, Sma> keyValuePair in Sma)
            {
                keyValuePair.Value.Add(futureCandleStick);
            }
            foreach (KeyValuePair<int, Ema> keyValuePair in Ema)
            {
                keyValuePair.Value.Add(futureCandleStick);
            }
            stochRsi.Add(futureCandleStick);
            _candleSticks = candleSticks;
        }
    }
}
