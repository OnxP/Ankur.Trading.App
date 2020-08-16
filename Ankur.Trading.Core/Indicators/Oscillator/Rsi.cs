using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Binance.API.Csharp.Client.Models.Market;
using Ankur.Trading.Core.Indicators;

namespace Ankur.Trading.Core.Oscillator
{
    public class Rsi:IIndicator
    {
        
        private IEnumerable<decimal> _closePrices;
        private List<decimal> _avgGain;
        private List<decimal> _avgLoss;
        public IEnumerable<decimal> rsi;
        private readonly int _length;

        public decimal Value => rsi?.First() ?? 0;
        public decimal Gradient { get; set; }

        public Rsi(IEnumerable<Candlestick> candleSticks, int length, string ticker) : this(candleSticks.Select(x => x.Close), length)
        { }

        public Rsi(IEnumerable<decimal> candleSticks, int length)
        {
            _closePrices = candleSticks;
            this._length = length;
            if (candleSticks.Count() < _length+1) return;
            CalculateRsi();
        }

        private void CalculateRsi()
        {
            var rsiList = new List<decimal>();
            List<decimal> closePrices = _closePrices.ToList();
            List<decimal> avgGain = new List<decimal>();
            List<decimal> avgLoss = new List<decimal>();
            closePrices.Reverse();

            //calculate the first RSI
            avgGain.Add(closePrices.Take(_length).Select((x, y) => closePrices[y+1] - x).Select(c => c > 0 ? c : 0).Average());
            avgLoss.Add(closePrices.Take(_length).Select((x, y) => closePrices[y+1] - x).Select(c => c < 0 ? -c : 0).Average());
            rsiList.Add(GetRsi(avgGain.First(), avgLoss.First()));

            for (int i = _length +1; i < closePrices.Count(); i++)
            {
                var diff = closePrices[i] - closePrices[i - 1];

                var t1 = (avgGain.Last() * (_length - 1) + (diff > 0 ? diff : 0)) / _length;
                var t2 = (avgLoss.Last() * (_length - 1) + (diff < 0 ? -diff : 0)) / _length;
                avgGain.Add(t1);
                avgLoss.Add(t2);
                rsiList.Add(GetRsi(avgGain.Last(),avgLoss.Last()));
            }

            rsiList.Reverse();
            avgGain.Reverse();
            avgLoss.Reverse();
            rsi = rsiList;
            _avgGain = avgGain;
            _avgLoss = avgLoss;
        }

        private decimal GetRsi(decimal avgGain,  decimal avgLoss  )
        {
            if (avgLoss == 0)
                return 100m;

            var rs = avgGain / avgLoss;

            return 100 - (100 / (1+rs));
        }

        public void AddCandleStick(Candlestick candleStick)
        {
            var diff = _closePrices.First() - candleStick.Close;

            var list = new List<decimal>();
            List<decimal> avgGain = new List<decimal>();
            List<decimal> avgLoss = new List<decimal>();
            List<decimal> rsiList = new List<decimal>();

            list.Add(candleStick.Close);
            list.AddRange(_closePrices.Take(100));
            _closePrices = list;

            if (_closePrices.Count() < _length+1) return;
            if (_closePrices.Count() == _length+1)
            {
                CalculateRsi();
                return;
            }


            var t1 = (_avgGain.First() * (_length - 1) + (diff > 0 ? diff : 0)) / _length;
            var t2 = (_avgLoss.First() * (_length - 1) + (diff < 0 ? -diff : 0)) / _length;
            avgGain.Add(t1);
            avgLoss.Add(t2);
            rsiList.Add(GetRsi(avgGain.First(), avgLoss.First()));

            avgGain.AddRange(_avgGain.Take(100));
            avgLoss.AddRange(_avgLoss.Take(100));
            
            rsiList.AddRange(rsi.Take(100));
            rsi = rsiList;
            _avgGain = avgGain;
            _avgLoss = avgLoss;
        }
    }
}
