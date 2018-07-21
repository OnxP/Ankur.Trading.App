namespace Ankur.Trading.Core.LinkedList
{
    public interface ILinkedListItem
    {
        ILinkedListItem Next { get;  }
        ILinkedListItem Previous { get;  }
        bool IsHead { get; }
        bool IsLast { get; }
    }
}