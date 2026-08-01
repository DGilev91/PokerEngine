namespace PokerEngine.Models;

public sealed class UncalledBet
{
    public int SeatId { get; }

    public long Amount { get; }

    public UncalledBet(int seatId, long amount)
    {
        if (seatId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seatId));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Сумма uncalled bet должна быть больше нуля.");
        }

        SeatId = seatId;
        Amount = amount;
    }
}