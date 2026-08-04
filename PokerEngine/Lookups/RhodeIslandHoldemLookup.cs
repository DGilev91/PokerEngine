using PokerEngine.Utilities;

namespace PokerEngine.Lookups;

public sealed class RhodeIslandHoldemLookup : Lookup
{
    protected override IReadOnlyList<Rank> RankOrder => Utilities.RankOrder.Standard;

    protected override void AddEntries()
    {
        AddMultisets(new Dictionary<int, int> { [1] = 3 }, [false], Label.HighCard);
        AddMultisets(new Dictionary<int, int> { [2] = 1, [1] = 1 }, [false], Label.OnePair);
        AddMultisets(new Dictionary<int, int> { [1] = 3 }, [true], Label.Flush);
        AddStraights(3, [false], Label.Straight);
        AddMultisets(new Dictionary<int, int> { [3] = 1 }, [false], Label.ThreeOfAKind);
        AddStraights(3, [true], Label.StraightFlush);
    }
}
