namespace PokerEngine.Models;

public sealed class Board
{
    public IReadOnlyList<string> Cards => _cards;

    private readonly List<string> _cards = [];

    internal void Deal(List<string> cards)
    {
        _cards.AddRange(cards);
    }
}