using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core.Indicators
{
    public class Ema
    {
        private IEnumerable<Candlestick> _candleSticks;
        public IEnumerable<decimal> ema;
        private readonly int Length;
        private decimal Multiplier => 2 / (decimal)(Length + 1);

        public decimal EmaValue => ema.Last();
        public decimal Gradient { get; set; }

        public Ema(IEnumerable<Candlestick> candleSticks, int length)
        {
            _candleSticks = candleSticks;
            this.Length = length;
            CalculateEma();
        }

        private void CalculateEma()
        {
            List<decimal> emaList = new List<decimal>();
            Queue<decimal> queue = new Queue<decimal>(Length+1);
            decimal previousValue = 0m;
            foreach (Candlestick candlestick in _candleSticks)
            {
                if (previousValue == 0m)
                {
                    queue.Enqueue(candlestick.Close);
                    if (queue.Count < Length) continue;
                    if (queue.Count > Length) queue.Dequeue();
                    var sum = queue.ToList().Sum();
                    previousValue = sum / Length;
                    emaList.Add(previousValue);
                }
                else
                {
                    previousValue = (candlestick.Close - previousValue) * Multiplier + previousValue;
                    emaList.Add(previousValue);
                }
            }

            ema = emaList;
        }

        public void Add(Candlestick futureCandleStick)
        {
            var list = _candleSticks.ToList();
            list.Add(futureCandleStick);
            _candleSticks = list.OrderByDescending(x => x.CloseDateTime);
            CalculateCurrentEma(futureCandleStick);
        }

        private void CalculateCurrentEma(Candlestick futureCandleStick)
        {
            var list = ema.ToList();
            list.Add((futureCandleStick.Close - EmaValue) * Multiplier + EmaValue);
            ema = list;
        }
    }
}
