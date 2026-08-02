namespace PokerEngine.States.Seats;

public sealed class Seat
{
    private readonly List<string> _holeCards = [];

    /// <summary>
    /// Gets the zero-based seat identifier.
    /// </summary>
    public int SeatId { get; }

    /// <summary>
    /// Gets the player's stack at the beginning of the hand.
    /// </summary>
    public long InitialStack { get; }

    /// <summary>
    /// Gets or sets the player's current remaining stack.
    /// </summary>
    public long Stack { get; set; }

    /// <summary>
    /// Gets or sets the total number of chips committed
    /// by the player during the current hand.
    /// </summary>
    public long TotalBet { get; set; }

    /// <summary>
    /// Gets or sets the number of chips committed
    /// by the player during the current betting round.
    /// </summary>
    public long RoundBet { get; set; }

    /// <summary>
    /// Gets the player's hole cards.
    /// </summary>
    public IReadOnlyList<string> HoleCards => _holeCards;

    /// <summary>
    /// Gets or sets a value indicating whether the player has folded.
    /// </summary>
    public bool IsFolded { get; set; }

    /// <summary>
    /// Gets a value indicating whether the player is all-in.
    /// </summary>
    public bool IsAllIn =>
        Stack == 0 &&
        !IsFolded;

    public Seat(
        int seatId,
        long stack)
    {
        if (seatId < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(seatId),
                seatId,
                "Seat identifier cannot be negative.");
        }

        if (stack < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stack),
                stack,
                "Stack cannot be negative.");
        }

        SeatId = seatId;
        InitialStack = stack;
        Stack = stack;
    }

    public void SetHoleCards(
        IEnumerable<string> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        _holeCards.Clear();
        _holeCards.AddRange(cards);
    }

    public void ClearRoundBet()
    {
        RoundBet = 0;
    }
}