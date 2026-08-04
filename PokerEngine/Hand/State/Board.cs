namespace PokerEngine.Hand.Context;

public sealed class Board
{
    private readonly List<string> _cards = [];

    public IReadOnlyList<string> Cards => _cards;

    public int Index { get; }

    public Board(int index)
    {
        Index = index;
    }

    internal void AddCards(IReadOnlyList<string> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        _cards.AddRange(cards);
    }
}