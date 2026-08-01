namespace PokerEngine.Models;

public sealed class Pot
{
    private readonly HashSet<int> _contributorSeatIds = [];
    private readonly HashSet<int> _eligibleSeatIds = [];

    public int Index { get; }

    public long Amount { get; private set; }

    public bool IsMain => Index == 0;

    public bool IsSide => Index > 0;

    public IReadOnlySet<int> ContributorSeatIds =>
        _contributorSeatIds;

    public IReadOnlySet<int> EligibleSeatIds =>
        _eligibleSeatIds;

    public Pot(int index)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        Index = index;
    }

    internal void AddContribution(
        int seatId,
        long amount)
    {
        if (seatId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seatId));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        Amount += amount;

        _contributorSeatIds.Add(seatId);
        _eligibleSeatIds.Add(seatId);
    }

    internal void RemoveEligibility(int seatId)
    {
        _eligibleSeatIds.Remove(seatId);
    }
}