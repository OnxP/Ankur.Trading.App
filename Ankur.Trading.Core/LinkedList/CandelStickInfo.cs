using System.Collections.Generic;
using Binance.API.Csharp.Client.Models.Market;

namespace Ankur.Trading.Core.LinkedList
{
    public class CandleStickInfo : ILinkedListItem
    {
        public CandleStickInfo(CandleStickInfo next, CandleStickInfo previous, Candlestick candlestick)
        {
            if (next == null) IsLast = true;
            else Next = next;
            if (Previous == null) IsHead = true;
            else Previous = previous;
            Candlestick = candlestick;
        }

        public Candlestick Candlestick { get; set; }

        //public List<TechnicalIndicator> Indicators { get; set; }

        public ILinkedListItem Next { get; private set; }
        public ILinkedListItem Previous { get; private set; }
        public bool IsHead { get; }
        public bool IsLast { get; }

    }
}
 