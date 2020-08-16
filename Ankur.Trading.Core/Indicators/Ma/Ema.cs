using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core.Indicators
{
    public class Ema : IIndicator
    {
        private IEnumerable<decimal> _closePrices;
        public IEnumerable<decimal> ema;
        private readonly int Length;
        private decimal Multiplier => 2 / (decimal)(Length + 1);

        public decimal Value => ema?.FirstOrDefault() ?? 0;
        public decimal Gradient => ema?.First() ?? 0 - ema?.ElementAt(1) ?? 0;

        public Ema(IEnumerable<Candlestick> candleSticks, int length, string ticker) : this(candleSticks.Select(x => x.Close),length)
        {
        }

        public Ema(IEnumerable<decimal> candleSticks, int length)
        {
            _closePrices = candleSticks;
            this.Length = length;
            if (_closePrices.Count() < length) return;
            CalculateEma();
        }

        private void CalculateEma()
        {
            List<decimal> emaList = new List<decimal>();
            List<decimal> closePrices = _closePrices.ToList();
            closePrices.Reverse();

            for (int i = Length -1 ; i < closePrices.Count(); i++)
            {
                if (i == Length-1) emaList.Add(closePrices.Take(Length).Sum()/Length);
                else emaList.Add((closePrices[i] - emaList.Last()) * Multiplier + emaList.Last());
            }
            emaList.Reverse();
            ema = emaList;
        }

        public void AddCandleStick(Candlestick futureCandleStick)
        {
            var list = new List<decimal>();
            list.Add(futureCandleStick.Close);
            list.AddRange(_closePrices.Take(100));
            _closePrices = list;
            if (_closePrices.Count() < Length) return;
            if (_closePrices.Count() == Length)
            {
                CalculateEma();
                return;
            }
            CalculateCurrentEma(futureCandleStick);
        }

        private void CalculateCurrentEma(Candlestick futureCandleStick)
        {
            var list = new List<decimal>();
            list.Add((futureCandleStick.Close - Value) * Multiplier + Value);
            list.AddRange(ema.Take(100));
            ema = list;
        }
    }
}
