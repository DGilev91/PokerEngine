namespace PokerEngine.Hand.Context;


public sealed class UncalledBet
{

    public int SeatId { get; }

    public long Amount { get; }


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