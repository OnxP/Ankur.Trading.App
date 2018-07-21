using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core.Indicators
{
    public class Sma
    {
        private readonly IEnumerable<Candlestick> _candleSticks;
        public IEnumerable<decimal> sma;
        private int Length;

        public decimal SmaValue => sma.Last();
        public decimal Gradient { get; set; }

        public Sma(IEnumerable<Candlestick> candleSticks, int length)
        {
            _candleSticks = candleSticks;
            this.Length = length;
            CalculateSma();
        }

        private void CalculateSma()
        {
            List<decimal> smaList = new List<decimal>();
            Queue<decimal> queue = new Queue<decimal>(Length+1);
            foreach (Candlestick candlestick in _candleSticks)
            {
                queue.Enqueue(candlestick.Close);
                if (queue.Count < Length) continue;
                if (queue.Count > Length) queue.Dequeue();
                var sum = queue.ToList().Sum();
                smaList.Add(sum/Length);
            }

            sma = smaList;
        }
    }
}
