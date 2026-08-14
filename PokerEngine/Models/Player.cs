namespace PokerEngine.Models;

public sealed class Player
{
    private readonly List<Card> _holeCards = [];

    public long StartingStack { get; }

    public long Stack { get; internal set; }

    public long TotalBet { get; internal set; }

    public long RoundBet { get; internal set; }

    public long Payoff { get; internal set; }

    public bool IsFolded { get; internal set; }

    public bool IsActive => !IsFolded;

    public bool IsAllIn => Stack == 0 && !IsFolded;

    public IReadOnlyList<Card> HoleCards => _holeCards;

    internal Player(long startingStack)
    {
        if (startingStack < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(startingStack));
        }

        StartingStack = startingStack;
        Stack = startingStack;
    }

    internal void DealHole(IReadOnlyList<Card> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        _holeCards.AddRange(cards);
    }
}