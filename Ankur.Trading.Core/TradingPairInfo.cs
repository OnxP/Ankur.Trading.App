using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Ankur.Trading.Core.Indicators;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core
{
    public class TradingPairInfo
    {
        private string pair;
        private TimeInterval interval;
        public IEnumerable<Candlestick> _candleSticks;
        public decimal CurrentPrice => _candleSticks.Last().Close;

        public Dictionary<int,Sma> Sma = new Dictionary<int, Sma>();

        public Ema Ema7;
        public Ema Ema25;
        public Ema Ema99;

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
            Sma.Add(40,new Sma(_candleSticks,40));
            Ema7 = new Ema(_candleSticks, 7);
            Ema25 = new Ema(_candleSticks, 25);
            Ema99 = new Ema(_candleSticks, 99);
        }

        public void Add(Candlestick futureCandleStick)
        {
            var candleSticks = _candleSticks.ToList<Candlestick>();
            candleSticks.Add(futureCandleStick);
            foreach (KeyValuePair<int, Sma> keyValuePair in Sma)
            {
                keyValuePair.Value.Add(futureCandleStick);
            }
            _candleSticks = candleSticks;
        }
    }
}
