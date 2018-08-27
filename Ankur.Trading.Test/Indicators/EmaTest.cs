using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankur.Trading;
using Ankur.Trading.Core;
using Ankur.Trading.Core.Indicators;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ankur.Trading.Test.Indicators
{
    [TestClass]
    public class EmaTest
    {
        private List<Candlestick> BuildCandleSticks()
        {
            var list =  new List<Candlestick>
            {
                new Candlestick() {Close = 11},
                new Candlestick() {Close = 12},
                new Candlestick() {Close = 13},
                new Candlestick() {Close = 14},
                new Candlestick() {Close = 15},
                new Candlestick() {Close = 16},
                new Candlestick() {Close = 17},
                new Candlestick() {Close = 18},
                new Candlestick() {Close = 19},
                new Candlestick() {Close = 20},
                new Candlestick() {Close = 21}
            };
            list.Reverse();
            return list;
        }

        [TestMethod]
        public void Ema_Test_5()
        {
            Ema sma = new Ema(BuildCandleSticks(),5);
            Assert.AreEqual(19,sma.EmaValue);
        }

        [TestMethod]
        public void Ema_Test_3()
        {
            Ema sma = new Ema(BuildCandleSticks(), 3);
            Assert.AreEqual(20, sma.EmaValue);
        }

        [TestMethod]
        public void Ema_Test_7()
        {
            Ema sma = new Ema(BuildCandleSticks(), 7);
            Assert.AreEqual(18, sma.EmaValue);
        }

        [TestMethod]
        public void Ema_Test_9()
        {
            Ema sma = new Ema(BuildCandleSticks(), 9);
            Assert.AreEqual(17, sma.EmaValue);
        }


    }
}
