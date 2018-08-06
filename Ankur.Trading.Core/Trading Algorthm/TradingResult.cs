using System;

namespace Ankur.Trading.Core.Trading_Algorthm
{
    public class TradingResult
    {
        public decimal Bought { get; set; }
        public decimal Sold { get; set; }
        public decimal Pnl => Bought - Sold;
        public decimal PnlPercent => Math.Round((1 - Sold / Bought) * 100,2);
    }
}
