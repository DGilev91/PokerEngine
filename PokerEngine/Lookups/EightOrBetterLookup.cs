using PokerEngine.Utilities;

namespace PokerEngine.Lookups;

public sealed class EightOrBetterLookup : Lookup
{
    protected override IReadOnlyList<Rank> RankOrder => Utilities.RankOrder.EightOrBetterLow;

    protected override void AddEntries()
    {
        AddMultisets(new Dictionary<int, int> { [1] = 5 }, [false, true], Label.HighCard);
    }
}
