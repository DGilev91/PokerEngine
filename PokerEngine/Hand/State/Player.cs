namespace PokerEngine.Hand.Context;

public sealed class Player
{
    private readonly List<string> _holeCards = [];

    public int SeatId { get; }

    public long InitialStack { get; }

    public long Stack { get; internal set; }

    public long TotalBet { get; internal set; }

    public long RoundBet { get; internal set; }

    public IReadOnlyList<string> HoleCards => _holeCards;

    public bool IsFolded { get; internal set; }

    public bool IsAllIn => Stack == 0 && !IsFolded;

    internal Player(int seatId, long stack)
    {
        if (seatId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seatId), seatId, "Seat identifier cannot be negative.");
        }

        if (stack < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stack), stack, "Stack cannot be negative.");
        }

        SeatId = seatId;
        InitialStack = stack;
        Stack = stack;
    }

    internal void SetHoleCards(IEnumerable<string> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        _holeCards.Clear();
        _holeCards.AddRange(cards);
    }

    internal void ClearRoundBet()
    {
        RoundBet = 0;
    }
}