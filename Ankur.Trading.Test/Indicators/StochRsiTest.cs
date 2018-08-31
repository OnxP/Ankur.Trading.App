using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading.Core.Oscillator;
using Binance.API.Csharp.Client.Models.Market;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ankur.Trading.Core.Indicators.Oscillator;

namespace Ankur.Trading.Test.Indicators
{
    [TestClass]
    public class StochRsiTest
    {
        private static IEnumerable<Candlestick> BuildCandleSticks()
        {
            var list = new List<Candlestick>
            {
                new Candlestick{Close = 54.0907m},
                new Candlestick{Close = 59.8981m},
                new Candlestick{Close = 58.1992m},
                new Candlestick{Close = 59.7562m},
                new Candlestick{Close = 52.3508m},
                new Candlestick{Close = 52.8207m},
                new Candlestick{Close = 56.9367m},
                new Candlestick{Close = 57.4695m},
                new Candlestick{Close = 55.2607m},
                new Candlestick{Close = 57.5080m},
                new Candlestick{Close = 54.8013m},
                new Candlestick{Close = 51.4717m},
                new Candlestick{Close = 56.1598m},
                new Candlestick{Close = 58.3369m},
                new Candlestick{Close = 56.0218m},
                new Candlestick{Close = 60.2219m},
                new Candlestick{Close = 56.7477m},
                new Candlestick{Close = 57.3832m},
                new Candlestick{Close = 50.2306m},
                new Candlestick{Close = 57.0617m},
                new Candlestick{Close = 61.5069m},
                new Candlestick{Close = 63.6927m},
                new Candlestick{Close = 66.2177m},
                new Candlestick{Close = 69.1576m},
                new Candlestick{Close = 70.7253m},
                new Candlestick{Close = 67.7876m},
                new Candlestick{Close = 68.8154m},
                new Candlestick{Close = 62.3843m},
                new Candlestick{Close = 67.5881m},
                new Candlestick{Close = 67.5881m},
            };
            return list;
        }

        private static IEnumerable<decimal> ShochRsiResults()
        {
            var list = new List<decimal>
            {
                0.81m,
                0.54m,
                1.00m,
                0.60m,
                0.68m,
                0.00m,
                0.68m,
                1.00m,
                1.00m,
                1.00m,
                1.00m,
                1.00m,
                0.86m,
                0.91m,
                0.59m,
                0.85m,
                0.85m
            };
            return list;
        }

        private static void Compare(IReadOnlyList<decimal> shochRsiResults, IReadOnlyList<decimal> results)
        {
            Assert.AreEqual(shochRsiResults.Count, results.Count);
            for (var i = 0; i < shochRsiResults.Count(); i++) Assert.AreEqual(shochRsiResults[i], results[i]);
        }

        [TestMethod]
        public void ShochRsi_Test_14()
        {
            var shochRsi = new StochRsi(BuildCandleSticks(),14, 14,3,3);
            Assert.AreEqual(0.85m, Math.Round(shochRsi.KValue, 2));
            var results = shochRsi.stochRsi.Select(value => Math.Round(value, 2)).ToList();
            Compare(ShochRsiResults().ToList(), results);
        }
    }
}