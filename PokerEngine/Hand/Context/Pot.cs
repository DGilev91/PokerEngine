namespace PokerEngine.Hand.Context;

/// <summary>
/// Represents a main pot or side pot created during a poker hand.
/// </summary>
public sealed class Pot
{
    private readonly HashSet<int> _contributorSeatIds = [];
    private readonly HashSet<int> _eligibleSeatIds = [];

    /// <summary>
    /// Gets the zero-based pot index.
    /// </summary>
    /// <remarks>
    /// Index <c>0</c> represents the main pot.
    /// Greater indexes represent side pots.
    /// </remarks>
    public int Index { get; }

    /// <summary>
    /// Gets the total number of chips contained in the pot.
    /// </summary>
    public long Amount { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this is the main pot.
    /// </summary>
    public bool IsMain => Index == 0;

    /// <summary>
    /// Gets a value indicating whether this is a side pot.
    /// </summary>
    public bool IsSide => Index > 0;

    /// <summary>
    /// Gets the identifiers of all seats that contributed to this pot.
    /// </summary>
    public IReadOnlySet<int> ContributorSeatIds =>
        _contributorSeatIds;

    /// <summary>
    /// Gets the identifiers of seats eligible to win this pot.
    /// </summary>
    public IReadOnlySet<int> EligibleSeatIds =>
        _eligibleSeatIds;

    /// <summary>
    /// Initializes a new instance of the <see cref="Pot"/> class.
    /// </summary>
    /// <param name="index">
    /// The zero-based pot index.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="index"/> is negative.
    /// </exception>
    internal Pot(int index)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index),
                index,
                "Pot index cannot be negative.");
        }

        Index = index;
    }

    /// <summary>
    /// Adds a seat contribution to this pot.
    /// </summary>
    internal void AddContribution(int seatId, long amount)
    {
        if (seatId < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(seatId),
                seatId,
                "Seat identifier cannot be negative.");
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                amount,
                "Contribution amount must be greater than zero.");
        }

        Amount += amount;

        _contributorSeatIds.Add(seatId);
        _eligibleSeatIds.Add(seatId);
    }

    /// <summary>
    /// Removes a seat from the set of players eligible to win this pot.
    /// </summary>
    internal void RemoveEligibility(int seatId)
    {
        _eligibleSeatIds.Remove(seatId);
    }
}