using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankur.Trading.Core.TradingAlgorthm
{
    public class TradingResult
    {
        public decimal Bought { get; set; }
        public decimal Sold { get; set; }
        public decimal Pnl => Bought - Sold;
        public decimal PnlPercent => Math.Round((1 - Sold / Bought) * 100,2);
    }
}
