using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Ankur.Trading.Core.Indicators;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using Ankur.Trading.Core.Indicators.Oscillator;
using Ankur.Trading.Core.Oscillator;
using System;

namespace Ankur.Trading.Core
{
    public class TradingPairInfo
    {
        private string pair;
        private TimeInterval interval;
        public IEnumerable<Candlestick> _candleSticks;
        public decimal CurrentPrice => _candleSticks.First().Close;

        public DateTime CloseDateTime => _candleSticks.First().CloseDateTime;

        public Dictionary<int,Sma> Sma = new Dictionary<int, Sma>();
        public Dictionary<int,Ema> Ema = new Dictionary<int, Ema>();

        public StochRsi stochRsi;
        public Rsi rsi;
        public Macd macd;

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

            stochRsi = new StochRsi(_candleSticks,14,14,8,8);
            rsi = new Rsi(_candleSticks,14);
            macd = new Macd(_candleSticks,12,26,9);
        }

        public void Add(Candlestick futureCandleStick)
        {
            List<Candlestick> list = new List<Candlestick>();
            list.Add(futureCandleStick);
            list.AddRange(_candleSticks);
            _candleSticks = list;
            foreach (KeyValuePair<int, Sma> keyValuePair in Sma)
            {
                keyValuePair.Value.AddCandleStick(futureCandleStick);
            }
            foreach (KeyValuePair<int, Ema> keyValuePair in Ema)
            {
                keyValuePair.Value.AddCandleStick(futureCandleStick);
            }
            stochRsi.AddCandleStick(futureCandleStick);
            rsi.AddCandleStick(futureCandleStick);
            macd.AddCandleStick(futureCandleStick);
        }
    }
}
