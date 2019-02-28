using System;
using System.Collections.Generic;
using System.Linq;
using Ankur.Trading.Core.Indicators;
using Binance.API.Csharp.Client.Models.Market;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ankur.Trading.Test.Indicators
{
    [TestClass]
    public class MaComparisioncs
    {
        private static IEnumerable<Candlestick> BuildCandleSticks()
        {
            var list = new List<Candlestick>
            {
                new Candlestick{Close = 22.2734m},
                new Candlestick{Close = 22.1940m},
                new Candlestick{Close = 22.0847m},
                new Candlestick{Close = 22.1741m},
                new Candlestick{Close = 22.1840m},
                new Candlestick{Close = 22.1344m},
                new Candlestick{Close = 22.2337m},
                new Candlestick{Close = 22.4323m},
                new Candlestick{Close = 22.2436m},
                new Candlestick{Close = 22.2933m},
                new Candlestick{Close = 22.1542m},
                new Candlestick{Close = 22.3926m},
                new Candlestick{Close = 22.3816m},
                new Candlestick{Close = 22.6109m},
                new Candlestick{Close = 23.3558m},
                new Candlestick{Close = 24.0519m},
                new Candlestick{Close = 23.7530m},
                new Candlestick{Close = 23.8324m},
                new Candlestick{Close = 23.9516m},
                new Candlestick{Close = 23.6338m},
                new Candlestick{Close = 23.8225m},
                new Candlestick{Close = 23.8722m},
                new Candlestick{Close = 23.6537m},
                new Candlestick{Close = 23.1870m},
                new Candlestick{Close = 23.0976m},
                new Candlestick{Close = 23.3260m},
                new Candlestick{Close = 22.6805m},
                new Candlestick{Close = 23.0976m},
                new Candlestick{Close = 22.4025m},
                new Candlestick{Close = 22.1725m}
            };
            list.Reverse();
            return list;
        }
        
        private static IEnumerable<decimal> SmaResults()
        {
            var list = new List<decimal>
            {
                22.225m,
                22.213m,
                22.233m,
                22.262m,
                22.306m,
                22.423m,
                22.615m,
                22.767m,
                22.907m,
                23.078m,
                23.212m,
                23.379m,
                23.527m,
                23.654m,
                23.711m,
                23.686m,
                23.613m,
                23.506m,
                23.432m,
                23.277m,
                23.131m
            };
            list.Reverse();
            return list;
        }

        private static IEnumerable<decimal> EmaResults()
        {
            var list = new List<decimal>
            {
                22.225m,
                22.212m,
                22.245m,
                22.270m,
                22.332m,
                22.518m,
                22.797m,
                22.971m,
                23.127m,
                23.277m,
                23.342m,
                23.429m,
                23.510m,
                23.536m,
                23.473m,
                23.404m,
                23.390m,
                23.261m,
                23.231m,
                23.081m,
                22.916m
            };
            list.Reverse();
            return list;
        }

        private string pair = "";
        [TestMethod]
        public void Ema_Test_10()
        {
            var ema = new Ema(BuildCandleSticks(), 10, pair);
            Assert.AreEqual(22.916m, Math.Round(ema.Value, 3));
            var results = ema.ema.Select(value => Math.Round(value, 3)).ToList();
            Compare(EmaResults().ToList(), results);
        }

        [TestMethod]
        public void Sma_Test_10()
        {
            var sma = new Sma(BuildCandleSticks(), 10, pair);
            Assert.AreEqual(23.131m, Math.Round(sma.Value, 3));
            var results = sma.sma.Select(value => Math.Round(value, 3)).ToList();
            Compare(SmaResults().ToList(), results);
        }

        private static void Compare(IReadOnlyList<decimal> smaResults, IReadOnlyList<decimal> results)
        {
            Assert.AreEqual(smaResults.Count, results.Count);
            for (var i = 0; i < smaResults.Count(); i++) Assert.AreEqual(smaResults[i], results[i]);
        }
    }
}