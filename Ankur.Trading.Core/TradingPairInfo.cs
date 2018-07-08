using System.Collections.Generic;
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

        public Sma Sma7;
        public Sma Sma25;
        public Sma Sma99;

        public TradingPairInfo(string pair, TimeInterval interval, IEnumerable<Candlestick> candleSticks)
        {
            this.pair = pair;
            this.interval = interval;
            this._candleSticks = candleSticks;
            BuildIndicators();
        }

        private void BuildIndicators()
        {
            Sma7 = new Sma(_candleSticks,7);
            Sma25 = new Sma(_candleSticks,25);
            Sma99 = new Sma(_candleSticks,99);
        }
        
    }
}
