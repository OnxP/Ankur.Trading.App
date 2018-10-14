using System;

namespace Binance.API.Csharp.Client.Models.Market
{
    public class Candlestick
    {
        public long OpenTime { get; set; }
        public DateTime OpenDateTime => DateTimeOffset.FromUnixTimeMilliseconds(OpenTime).DateTime.ToLocalTime();
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public long CloseTime { get; set; }
        public DateTime CloseDateTime => DateTimeOffset.FromUnixTimeMilliseconds(CloseTime).DateTime.ToLocalTime();
        public decimal QuoteAssetVolume { get; set; }
        public int NumberOfTrades { get; set; }
        public decimal TakerBuyBaseAssetVolume { get; set; }
        public decimal TakerBuyQuoteAssetVolume { get; set; }
    }
}
