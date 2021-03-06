﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Ankur.Trading.Core.Indicators;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using Ankur.Trading.Core.Indicators.Oscillator;
using Ankur.Trading.Core.Oscillator;
using System;
using Ankur.Trading.Core.Indicators.Ma;
using kx;

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
        public Dictionary<int,Gsma> Gsma = new Dictionary<int, Gsma>();

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
            Sma.Add(5,new Sma(_candleSticks,5, pair));
            Sma.Add(10,new Sma(_candleSticks,10, pair));
            Sma.Add(15,new Sma(_candleSticks,15, pair));
            Sma.Add(20,new Sma(_candleSticks,20, pair));
            Sma.Add(40,new Sma(_candleSticks,40, pair));
            Sma.Add(100,new Sma(_candleSticks,100, pair));
            Ema.Add(5,new Ema(_candleSticks,5, pair));
            Ema.Add(10,new Ema(_candleSticks,10, pair));
            Ema.Add(15,new Ema(_candleSticks,15, pair));
            Ema.Add(20,new Ema(_candleSticks,20, pair));
            Ema.Add(40,new Ema(_candleSticks,40, pair));
            Ema.Add(80,new Ema(_candleSticks,80, pair));

            Gsma.Add(20,new Gsma(_candleSticks,20,10, pair));

            stochRsi = new StochRsi(_candleSticks,14,14,8,8, pair);
            rsi = new Rsi(_candleSticks,14, pair);
            macd = new Macd(_candleSticks,12,26,9, pair);
        }

        public void Add(Candlestick futureCandleStick)
        {
            //kdb c = new kdb("localhost", 5000);
            List<Candlestick> list = new List<Candlestick>();
            list.Add(futureCandleStick);
            list.AddRange(_candleSticks);
            _candleSticks = list;
            foreach (KeyValuePair<int, Sma> keyValuePair in Sma)
            {
                keyValuePair.Value.AddCandleStick(futureCandleStick);
                //c.k($"insert[`MA](`{pair};\"Z\"$ \"{futureCandleStick.OpenDateTime:yyyy-MM-ddTHH:mm:ss:fffff}\"; `SMA; {keyValuePair.Key.ToString()}; \"f\"${keyValuePair.Value.Value})");
            }
            foreach (KeyValuePair<int, Ema> keyValuePair in Ema)
            {
                keyValuePair.Value.AddCandleStick(futureCandleStick);
               // c.k($"insert[`MA](`{pair};\"Z\"$ \"{futureCandleStick.OpenDateTime:yyyy-MM-ddTHH:mm:ss:fffff}\"; `EMA;{keyValuePair.Key.ToString()}; \"f\"${keyValuePair.Value.Value})");
            }
            foreach (KeyValuePair<int, Gsma> keyValuePair in Gsma)
            {
                keyValuePair.Value.AddCandleStick(futureCandleStick);
                //c.k($"insert[`MA](`{pair};\"Z\"$ \"{futureCandleStick.OpenDateTime:yyyy-MM-ddTHH:mm:ss:fffff}\"; `GSMA; {keyValuePair.Key.ToString()}; \"f\"${keyValuePair.Value.Value*10000000000})");
            }
            stochRsi.AddCandleStick(futureCandleStick);
            //c.k($"insert[`Oscillator](`{pair};\"Z\"$ \"{futureCandleStick.OpenDateTime:yyyy-MM-ddTHH:mm:ss:fffff}\"; `SRSI; \"f\"${ stochRsi.KValue};\"f\"${stochRsi.DValue})");
            rsi.AddCandleStick(futureCandleStick);
            //c.k($"insert[`Oscillator](`{pair};\"Z\"$ \"{futureCandleStick.OpenDateTime:yyyy-MM-ddTHH:mm:ss:fffff}\"; `RSI; \"f\"${ rsi.Value};0.0)");
            macd.AddCandleStick(futureCandleStick);
            //c.k($"insert[`Oscillator](`{pair};\"Z\"$ \"{futureCandleStick.OpenDateTime:yyyy-MM-ddTHH:mm:ss:fffff}\"; `MACD; \"f\"${ macd.MacdLine.First()*100000000};\"f\"${macd.SignalLine.First()*100000000})");
        }
    }
}
