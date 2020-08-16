using System.ComponentModel;

namespace Binance.API.Csharp.Client.Models.Enums
{
    /// <summary>
    /// Time interval for the candlestick.
    /// </summary>
    public enum TimeInterval
    {
        [Description("1m")]
        Minutes_1 = 1,
        [Description("3m")]
        Minutes_3 = 3,
        [Description("5m")]
        Minutes_5 = 5,
        [Description("15m")]
        Minutes_15 = 15,
        [Description("30m")]
        Minutes_30 = 30,
        [Description("1h")]
        Hours_1 = 60,
        [Description("2h")]
        Hours_2,
        [Description("4h")]
        Hours_4,
        [Description("6h")]
        Hours_6,
        [Description("8h")]
        Hours_8,
        [Description("12h")]
        Hours_12,
        [Description("1d")]
        Days_1,
        [Description("3d")]
        Days_3,
        [Description("1w")]
        Weeks_1,
        [Description("1M")]
        Months_1
    }
}
