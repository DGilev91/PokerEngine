namespace PokerEngine.Models;

public sealed class Player
{
    private readonly List<string> _holeCards = [];

    public int SeatId { get; }

    public long StartingStack { get; }

    public long Stack { get; internal set; }

    public long RoundBet { get; internal set; }

    public long Payoff { get; internal set; }

    public bool IsFolded { get; internal set; }

    public bool IsActive => !IsFolded;

    public bool IsAllIn => Stack == 0 && !IsFolded;

    public IReadOnlyList<string> HoleCards => _holeCards;

    internal Player(int seatId, long startingStack)
    {
        if (seatId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seatId));
        }

        if (startingStack < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(startingStack));
        }

        SeatId = seatId;
        StartingStack = startingStack;
        Stack = startingStack;
    }

    internal void DealHole(IReadOnlyList<string> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        _holeCards.AddRange(cards);
    }
}