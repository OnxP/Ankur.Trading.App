using Binance.API.Csharp.Client.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankur.Trading.Core.Indicators.Ma
{
    public class Gsma: IIndicator
    {
        private Ema _ema;
        private IEnumerable<decimal> _diff;
        public IEnumerable<decimal> gsma;
        private readonly int averageLength;

        public decimal Value => gsma?.FirstOrDefault() ?? 0;

        public Gsma(IEnumerable<Candlestick> candleSticks, int length, int averageLength, string ticker) : this(candleSticks.Select(x => x.Close),length, averageLength)
        {
        }

        public Gsma(IEnumerable<decimal> candleSticks, int length,int averageLength)
        {
            _ema = new Ema(candleSticks, length);
            this.averageLength = averageLength;
            if (candleSticks.Count() < averageLength + length) return;
            CalculateGsma();
        }

        private void CalculateGsma()
        {
            List<decimal> smaList = new List<decimal>();
            List<decimal> diff = new List<decimal>();
            Queue<decimal> queue = new Queue<decimal>(averageLength + 1);

            for (int i = 0; i < _ema.ema.Count()-1; i++)
            {
                diff.Add(_ema.ema.Skip(i).First() - _ema.ema.Skip(i +1).First());
            }

            _diff = diff;

            for (int i = 0; i < _diff.Count() - averageLength + 1; i++)
            {
                smaList.Add(_diff.Skip(i).Take(averageLength).Sum() / averageLength);
            }

            gsma = smaList;
        }

        public void AddCandleStick(Candlestick futureCandleStick)
        {
            _ema.AddCandleStick(futureCandleStick);
            if (_ema.ema == null || _ema.ema.Count() < averageLength) return;
            if (_ema.ema != null && _ema.ema.Count() == averageLength)
            {
                CalculateGsma();
                return;
            }


            CalculateCurrentSma();
        }

        private void CalculateCurrentSma()
        {
            var listdiff = new List<decimal>();
            listdiff.Add(_ema.Value - _ema.ema.Skip(1).First());
            listdiff.AddRange(gsma.Take(100));
            _diff = listdiff;
            var sum = _diff.Take(averageLength).Sum();
            var list = new List<decimal>();
            list.Add(sum / averageLength);
            list.AddRange(gsma.Take(100));
            gsma = list;
        }
    }
}
