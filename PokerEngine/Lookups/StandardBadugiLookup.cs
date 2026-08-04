using PokerEngine.Utilities;

namespace PokerEngine.Lookups;

public sealed class StandardBadugiLookup : BadugiLookup
{
    protected override IReadOnlyList<Rank> RankOrder => Utilities.RankOrder.Standard;
}
