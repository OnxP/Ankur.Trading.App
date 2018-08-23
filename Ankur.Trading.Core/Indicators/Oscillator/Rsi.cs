using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core.Oscillator
{
    public class Rsi
    {
        private readonly IEnumerable<Candlestick> _candleSticks;
        public IEnumerable<decimal> rsi;
        private readonly int _length;

        public decimal Value => rsi.Last();
        public decimal Gradient { get; set; }

        public Rsi(IEnumerable<Candlestick> candleSticks, int length){
            _candleSticks = candleSticks;
            this._length = length;
            CalculateRsi();
        }

        private void CalculateRsi()
        {
            var rsiList = new List<decimal>();
            var gainQueue = new Queue<decimal>(_length + 1);
            var lossQueue = new Queue<decimal>(_length + 1);
            decimal previousClose = 0m;
            decimal previousAvgGain = 0m;
            decimal previousAvgLoss = 0m;
            foreach (var candlestick in _candleSticks)
            {
                if (previousClose == 0m)
                {
                    previousClose = candlestick.Close;
                    continue;
                }

                decimal change = candlestick.Close - previousClose;
                previousClose = candlestick.Close;
                var gain = 0m;
                var loss = 0m;

                if (change > 0) gain = change;
                else loss = change * -1;

                gainQueue.Enqueue(gain);
                lossQueue.Enqueue(loss);

                if (gainQueue.Count > _length) gainQueue.Dequeue();
                if (lossQueue.Count > _length) lossQueue.Dequeue();

                if (gainQueue.Count != _length) continue;

                

                if(previousAvgGain ==0) previousAvgGain = gainQueue.Average();
                else previousAvgGain = (previousAvgGain * (_length-1) + gain)/_length;
                if (previousAvgLoss == 0) previousAvgLoss = lossQueue.Average();
                else previousAvgLoss = (previousAvgLoss * (_length - 1) + loss) / _length;


                rsiList.Add(GetRsi(previousAvgGain, previousAvgLoss));

            }

            rsi = rsiList;
        }

        private decimal GetRsi(decimal avgGain,  decimal avgLoss  )
        {
            if (avgLoss == 0)
                return 100m;

            var rs = avgGain / avgLoss;

            return 100 - (100/ (1+rs));
        }
    }
}
