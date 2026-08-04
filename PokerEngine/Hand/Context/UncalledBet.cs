namespace PokerEngine.Hand.Context;

/// <summary>
/// Represents an unmatched wager that must be returned to a player.
/// </summary>
public sealed class UncalledBet
{
    /// <summary>
    /// Gets the seat that receives the returned uncalled amount.
    /// </summary>
    public int SeatId { get; }

    /// <summary>
    /// Gets the unmatched amount returned to the player.
    /// </summary>
    public long Amount { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UncalledBet"/> class.
    /// </summary>
    /// <param name="seatId">
    /// The identifier of the seat receiving the returned amount.
    /// </param>
    /// <param name="amount">
    /// The unmatched chip amount.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="seatId"/> is negative
    /// or <paramref name="amount"/> is not greater than zero.
    /// </exception>
    internal UncalledBet(
        int seatId,
        long amount)
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
                "Uncalled bet amount must be greater than zero.");
        }

        SeatId = seatId;
        Amount = amount;
    }
}