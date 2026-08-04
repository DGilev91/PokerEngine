using PokerEngine.Utilities;

namespace PokerEngine.Lookups;

public sealed class RegularLookup : Lookup
{
    protected override IReadOnlyList<Rank> RankOrder => Utilities.RankOrder.Regular;

    protected override void AddEntries()
    {
        AddMultisets(new Dictionary<int, int> { [1] = 5 }, [false, true], Label.HighCard);
        AddMultisets(new Dictionary<int, int> { [2] = 1, [1] = 3 }, [false], Label.OnePair);
        AddMultisets(new Dictionary<int, int> { [2] = 2, [1] = 1 }, [false], Label.TwoPair);
        AddMultisets(new Dictionary<int, int> { [3] = 1, [1] = 2 }, [false], Label.ThreeOfAKind);
        AddMultisets(new Dictionary<int, int> { [3] = 1, [2] = 1 }, [false], Label.FullHouse);
        AddMultisets(new Dictionary<int, int> { [4] = 1, [1] = 1 }, [false], Label.FourOfAKind);
    }
}
