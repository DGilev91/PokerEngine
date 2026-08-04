using PokerEngine.Utilities;

namespace PokerEngine.Lookups;

public class BadugiLookup : Lookup
{
    protected override IReadOnlyList<Rank> RankOrder => Utilities.RankOrder.Regular;

    protected override void AddEntries()
    {
        for (var count = 4; count > 0; count--)
        {
            AddMultisets(new Dictionary<int, int> { [1] = count }, [count == 1], Label.HighCard);
        }
    }

    protected override (long Hash, bool Suited) GetKey(IEnumerable<Card> cards)
    {
        var values = cards.ToArray();

        if (!Card.AreRainbow(values))
        {
            throw new ArgumentException($"Badugi hands must be rainbow, but the cards '{string.Concat(values)}' are not.", nameof(cards));
        }

        return base.GetKey(values);
    }
}
