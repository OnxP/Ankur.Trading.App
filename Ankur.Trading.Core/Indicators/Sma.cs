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
        private IEnumerable<Candlestick> _candleSticks;
        private IEnumerable<decimal> _sma;
        private int Length;

        public decimal SmaValue => _sma.First();
        public decimal Gradient { get; set; }

        public Sma(IEnumerable<Candlestick> candleSticks, int length)
        {
            _candleSticks = candleSticks;
            this.Length = length;
            CalculateSMA();
        }

        private void CalculateSMA()
        {
            decimal[] avgsDecimals = new decimal[_candleSticks.Count() - Length];
            int pos = 0;
            foreach (Candlestick candlestick in _candleSticks.OrderBy(x=>x.OpenTime))
            {
                if (pos + Length >= _candleSticks.Count()) break;
                for (int i = 0 + pos; i < avgsDecimals.Length && i - pos < Length; i++)
                {
                    avgsDecimals[i] += candlestick.Close;
                }
                pos++;
            }
            List<decimal> smaList = new List<decimal>();
            foreach (decimal avgsDecimal in avgsDecimals.Reverse())
            {
                smaList.Add(avgsDecimal/Length);
            }

            _sma = smaList;
        }
    }
}
