namespace PokerEngine.Models;

public sealed class Board
{
    public int Index { get; }

    public IReadOnlyList<string> Cards => _cards;

    private readonly List<string> _cards = [];

    internal Board(int index)
    {
        Index = index;
    }

    internal void Deal(List<string> cards)
    {
        _cards.AddRange(cards);
    }
}