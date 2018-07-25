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
        private IEnumerable<Candlestick> _candleSticks;
        public decimal CurrentPrice => _candleSticks.Last().Close;

        public Dictionary<int,Sma> Sma = new Dictionary<int, Sma>();

        public Sma Sma7;
        public Sma Sma25;
        public Sma Sma99;

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
            Sma.Add(20,new Sma(_candleSticks,20));
            Ema7 = new Ema(_candleSticks, 7);
            Ema25 = new Ema(_candleSticks, 25);
            Ema99 = new Ema(_candleSticks, 99);
        }
        
    }
}
