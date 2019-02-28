using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core.Indicators
{
    public class Sma : IIndicator
    {
        private IEnumerable<decimal> _closePrices;
        public IEnumerable<decimal> sma;
        private int Length;

        public decimal Value => sma.First();
        public decimal Gradient => sma.First() - sma.ElementAt(1);

        public Sma(IEnumerable<Candlestick> candleSticks, int length, string ticker): this(candleSticks.Select(x => x.Close),length)
        {
           
        }

        public Sma(IEnumerable<decimal> candleSticks, int length)
        {
            _closePrices = candleSticks;
            this.Length = length;
            CalculateSma();
        }

        private void CalculateSma()
        {
            List<decimal> smaList = new List<decimal>();
            Queue<decimal> queue = new Queue<decimal>(Length+1);

            for ( int i = 0; i< _closePrices.Count() - Length + 1; i++)
            {
                smaList.Add(_closePrices.Skip(i).Take(Length).Sum()/Length);
            }

            sma = smaList;
        }

        public void AddCandleStick(Candlestick futureCandleStick)
        {
            var list = new List<decimal>();
            list.Add(futureCandleStick.Close);
            list.AddRange(_closePrices.Take(100));
            _closePrices = list;
            //_candleSticks = list.OrderByDescending(x=>x.CloseDateTime);
            CalculateCurrentSma();
        }

        private void CalculateCurrentSma()
        {
            var sum = _closePrices.Take(Length).Sum();
            var list = new List<decimal>();
            list.Add(sum/Length);
            list.AddRange(sma.Take(100));
            sma = list;
        }
    }
}
