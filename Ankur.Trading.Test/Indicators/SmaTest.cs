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
    public class SmaTest
    {
        private List<Candlestick> BuildCandleSticks()
        {
            var list =  new List<Candlestick>
            {
                new Candlestick() {Close = 25},
                new Candlestick() {Close = 85},
                new Candlestick() {Close = 65},
                new Candlestick() {Close = 45},
                new Candlestick() {Close = 95},
                new Candlestick() {Close = 75},
                new Candlestick() {Close = 15},
                new Candlestick() {Close = 35}
            };
            list.Reverse();
            return list;
        }

        [TestMethod]
        public void Sma_Test_3day()
        {
            Sma sma = new Sma(BuildCandleSticks(),3);
            Assert.AreEqual(58.3333,sma.SmaValue);
        }


    }
}
