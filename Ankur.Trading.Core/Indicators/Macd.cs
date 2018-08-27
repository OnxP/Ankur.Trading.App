using Binance.API.Csharp.Client.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankur.Trading.Core.Indicators
{
    public class Macd
    {
        private Ema Fast { get; set; }
        private Ema Slow { get; set; }
        public IEnumerable<decimal> SignalLine { get; set; }
        public IEnumerable<decimal> MacdLine { get; set; }
        public IEnumerable<decimal> Histogram { get; set; }

        private IEnumerable<Candlestick> _candleSticks;
        private int _fast;
        private int _slow;
        private int _signal;


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

            List<decimal> list = new List<decimal>();

            for(int i = 0; i < _slow; i++)
            {

            }
        }
    }
}
