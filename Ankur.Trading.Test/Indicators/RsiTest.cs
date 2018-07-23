using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.Oscillator;
using Binance.API.Csharp.Client.Models.Market;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ankur.Trading.Test.Indicators
{
    [TestClass]
    public class RsiTest
    {
        private static IEnumerable<Candlestick> BuildCandleSticks()
        {
            var list = new List<Candlestick>
            {
                new Candlestick{Close = 44.3389m},
                new Candlestick{Close = 44.0902m},
                new Candlestick{Close = 44.1497m},
                new Candlestick{Close = 43.6124m},
                new Candlestick{Close = 44.3278m},
                new Candlestick{Close = 44.8264m},
                new Candlestick{Close = 45.0955m},
                new Candlestick{Close = 45.4245m},
                new Candlestick{Close = 45.8433m},
                new Candlestick{Close = 46.0826m},
                new Candlestick{Close = 45.8931m},
                new Candlestick{Close = 46.0328m},
                new Candlestick{Close = 45.6140m},
                new Candlestick{Close = 46.2820m},
                new Candlestick{Close = 46.2820m},
                new Candlestick{Close = 46.0028m},
                new Candlestick{Close = 46.0328m},
                new Candlestick{Close = 46.4116m},
                new Candlestick{Close = 46.2222m},
                new Candlestick{Close = 45.6439m},
                new Candlestick{Close = 46.2122m},
                new Candlestick{Close = 46.2521m},
                new Candlestick{Close = 45.7137m},
                new Candlestick{Close = 46.4515m},
                new Candlestick{Close = 45.7835m},
                new Candlestick{Close = 45.3548m},
                new Candlestick{Close = 44.0288m},
                new Candlestick{Close = 44.1783m},
                new Candlestick{Close = 44.2181m},
                new Candlestick{Close = 44.5672m},
                new Candlestick{Close = 43.4205m},
                new Candlestick{Close = 42.6628m},
                new Candlestick{Close = 43.1314m}
            };
            return list;
        }

        private static IEnumerable<decimal> RsiResults()
        {
            var list = new List<decimal>
            {
                70.53m,
                66.32m,
                66.55m,
                69.41m,
                66.36m,
                57.97m,
                62.93m,
                63.26m,
                56.06m,
                62.38m,
                54.71m,
                50.42m,
                39.99m,
                41.46m,
                41.87m,
                45.46m,
                37.30m,
                33.08m,
                37.77m
            };
            return list;
        }

        private static void Compare(IReadOnlyList<decimal> rsiResults, IReadOnlyList<decimal> results)
        {
            Assert.AreEqual(rsiResults.Count, results.Count);
            for (var i = 0; i < rsiResults.Count(); i++) Assert.AreEqual(rsiResults[i], results[i]);
        }

        [TestMethod]
        public void Rsi_Test_14()
        {
            var rsi = new Rsi(BuildCandleSticks(), 14);
            Assert.AreEqual(37.77m, Math.Round(rsi.Value, 2));
            var results = rsi.rsi.Select(value => Math.Round(value, 2)).ToList();
            Compare(RsiResults().ToList(), results);
        }
    }
}




















































