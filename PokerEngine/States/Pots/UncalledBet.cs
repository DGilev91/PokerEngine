namespace PokerEngine.States.Pots;

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

    public UncalledBet(
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