using System.ComponentModel;

namespace Ankur.Trading.Core.Trading_Algorthm
{
    public enum TradingAlgorthm
    {
        [Description("Algorthm for trading using the Simple SMA, 5 10 and 40")]
        SimpleSMA,
        LongShochRsi,
        Macd,
    }
}
