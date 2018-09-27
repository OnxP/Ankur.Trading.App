using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankur.Trading.Core.Log
{
    public interface ITradingLog
    {
        string Pair { get; }
        decimal Quantity { get;  }
        decimal BtcQuantity { get;  }
        decimal Price { get;  }
        DateTime CloseTime { get;  }
    }
}
