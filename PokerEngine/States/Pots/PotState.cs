namespace PokerEngine.States.Pots;

/// <summary>
/// Represents the current collection of pots and any uncalled bet
/// associated with a poker hand.
/// </summary>
public sealed class PotState
{
    private readonly List<Pot> _pots = [];

    /// <summary>
    /// Gets all main and side pots in index order.
    /// </summary>
    public IReadOnlyList<Pot> Pots => _pots;

    /// <summary>
    /// Gets the main pot, or <see langword="null"/> when no pot exists.
    /// </summary>
    public Pot? MainPot =>
        _pots.Count > 0
            ? _pots[0]
            : null;

    /// <summary>
    /// Gets all side pots.
    /// </summary>
    public IReadOnlyList<Pot> SidePots =>
        _pots.Count > 1
            ? _pots.Skip(1).ToArray()
            : [];

    /// <summary>
    /// Gets the current uncalled bet, or <see langword="null"/>
    /// when no uncalled amount exists.
    /// </summary>
    public UncalledBet? UncalledBet { get; private set; }

    /// <summary>
    /// Gets the total amount contained in all pots.
    /// </summary>
    public long PotAmount =>
        _pots.Sum(pot => pot.Amount);

    /// <summary>
    /// Gets the current uncalled amount.
    /// </summary>
    public long UncalledAmount =>
        UncalledBet?.Amount ?? 0;

    /// <summary>
    /// Gets the combined amount contained in the pots
    /// and the current uncalled bet.
    /// </summary>
    public long TotalAmount =>
        PotAmount + UncalledAmount;

    /// <summary>
    /// Clears all pot and uncalled-bet information.
    /// </summary>
    internal void Clear()
    {
        _pots.Clear();
        UncalledBet = null;
    }

    /// <summary>
    /// Adds a pot to the current pot state.
    /// </summary>
    /// <param name="pot">
    /// The pot to add.
    /// </param>
    internal void AddPot(Pot pot)
    {
        ArgumentNullException.ThrowIfNull(pot);

        _pots.Add(pot);
    }

    /// <summary>
    /// Sets the current uncalled bet.
    /// </summary>
    /// <param name="seatId">
    /// The identifier of the seat that owns the uncalled amount.
    /// </param>
    /// <param name="amount">
    /// The uncalled chip amount.
    /// </param>
    internal void SetUncalledBet(
        int seatId,
        long amount)
    {
        UncalledBet = new UncalledBet(
            seatId,
            amount);
    }
}