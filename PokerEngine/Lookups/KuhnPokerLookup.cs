using PokerEngine.Utilities;

namespace PokerEngine.Lookups;

public sealed class KuhnPokerLookup : Lookup
{
    protected override IReadOnlyList<Rank> RankOrder => Utilities.RankOrder.KuhnPoker;

    protected override void AddEntries()
    {
        AddMultisets(new Dictionary<int, int> { [1] = 1 }, [true], Label.HighCard);
    }
}
