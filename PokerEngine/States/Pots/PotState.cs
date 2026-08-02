namespace PokerEngine.States.Pots;

public sealed class PotState
{
    private readonly List<Pot> _pots = [];

    public IReadOnlyList<Pot> Pots => _pots;

    public Pot? MainPot =>
        _pots.Count > 0
            ? _pots[0]
            : null;

    public IReadOnlyList<Pot> SidePots =>
        _pots.Count > 1
            ? _pots.Skip(1).ToArray()
            : [];

    public UncalledBet? UncalledBet { get; private set; }

    public long PotAmount =>
        _pots.Sum(pot => pot.Amount);

    public long UncalledAmount =>
        UncalledBet?.Amount ?? 0;

    public long TotalAmount =>
        PotAmount + UncalledAmount;

    public void Clear()
    {
        _pots.Clear();
        UncalledBet = null;
    }

    public void AddPot(Pot pot)
    {
        ArgumentNullException.ThrowIfNull(pot);

        _pots.Add(pot);
    }

    public void SetUncalledBet(
        int seatId,
        long amount)
    {
        UncalledBet = new UncalledBet(
            seatId,
            amount);
    }
}