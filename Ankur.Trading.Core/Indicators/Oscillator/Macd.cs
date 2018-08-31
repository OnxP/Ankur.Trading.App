using Binance.API.Csharp.Client.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankur.Trading.Core.Indicators
{
    public class Macd : IIndicator
    {
        private Ema Fast { get; set; }
        private Ema Slow { get; set; }
        public IEnumerable<decimal> SignalLine => MacdLineEma.ema;
        public IEnumerable<decimal> MacdLine { get; set; }
        public IEnumerable<decimal> Histogram { get; set; }

        private IEnumerable<Candlestick> _candleSticks;
        private Ema MacdLineEma;
        private int _fast;
        private int _slow;
        private int _signal;

        private decimal Multiplier => 2 / (_signal + 1);

        public decimal Value => MacdLine.First() - SignalLine.First();

        public Macd (IEnumerable<Candlestick> candleSticks, int fast, int slow, int signal)
        {
            this._candleSticks = candleSticks;
            this._fast = fast;
            this._slow = slow;
            this._signal = signal;

            CalculateMacd();
        }

        private void CalculateMacd()
        {
            Fast = new Ema(_candleSticks, _fast);
            Slow = new Ema(_candleSticks, _slow);

            var fastEma = Fast.ema.ToList();
            var slowEma = Slow.ema.ToList();

            List<decimal> list = new List<decimal>();

            for(int i = 0; i < _slow; i++)
            {
                list.Add(fastEma[i] - slowEma[i]);
            }
            MacdLine = list;
            MacdLineEma = new Ema(MacdLine, _signal);

            List<decimal> histogram = new List<decimal>();

            for ( int i=0; i< SignalLine.Count();i++ )
            {
                histogram.Add(list[i] - SignalLine.ElementAt(i));
            }
            Histogram = histogram;
        }

        public void AddCandleStick(Candlestick candleStick)
        {
            List<Candlestick> candlesticklist = new List<Candlestick>();
            candlesticklist.Add(candleStick);
            candlesticklist.AddRange(_candleSticks.Take(100));
            _candleSticks = candlesticklist;
            Fast.AddCandleStick(candleStick);
            Slow.AddCandleStick(candleStick);
            var diff = Fast.Value - Slow.Value;
            List<decimal> list = new List<decimal>();
            list.Add(diff);
            list.AddRange(MacdLine.Take(100));
            MacdLine = list;
            MacdLineEma.AddCandleStick(new Candlestick() { Close = diff });

            list = new List<decimal>();
            list.Add(diff - MacdLineEma.Value);
            list.AddRange(Histogram.Take(100));
            Histogram = list;
        }
    }
}
