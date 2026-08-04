using PokerEngine.Lookups;

namespace PokerEngine.Tests.Lookups;

public sealed class LookupTests
{
    [Fact]
    public void StandardLookupOrdersHands()
    {
        var lookup = new StandardLookup();
        var pair = lookup.GetEntry("As3sQhJsJc");
        var trips = lookup.GetEntry("2s4sKhKsKc");

        Assert.True(pair.CompareTo(trips) < 0);
        Assert.Equal(Label.OnePair, pair.Label);
        Assert.Equal(Label.ThreeOfAKind, trips.Label);
    }

    [Fact]
    public void StandardLookupRejectsImpossibleHand()
    {
        var lookup = new StandardLookup();

        Assert.Throws<ArgumentException>(() => lookup.GetEntry("AcAdAhAsAc"));
    }

    [Fact]
    public void ShortDeckFlushBeatsFullHouse()
    {
        var lookup = new ShortDeckHoldemLookup();
        var fullHouse = lookup.GetEntry("AhAcAs6h6d");
        var flush = lookup.GetEntry("AhKhQhJh9h");

        Assert.True(flush.CompareTo(fullHouse) > 0);
    }

    [Fact]
    public void EightOrBetterAcceptsOnlyQualifyingLow()
    {
        var lookup = new EightOrBetterLookup();

        Assert.True(lookup.HasEntry("As2d3h4c8s"));
        Assert.False(lookup.HasEntry("As2d3h4c9s"));
        Assert.False(lookup.HasEntry("As2d3h4c4s"));
    }

    [Fact]
    public void RegularLookupIgnoresFlushes()
    {
        var lookup = new RegularLookup();
        var suited = lookup.GetEntry("Ah6h7h8h9h");
        var mixed = lookup.GetEntry("As6d7h8c9s");

        Assert.Equal(suited.Index, mixed.Index);
    }

    [Fact]
    public void BadugiRequiresRainbowCards()
    {
        var lookup = new BadugiLookup();

        Assert.True(lookup.HasEntry("Ac2d3h4s"));
        Assert.Throws<ArgumentException>(() => lookup.GetEntry("Ac2c3h4s"));
    }

    [Fact]
    public void KuhnPokerSupportsOnlyJqk()
    {
        var lookup = new KuhnPokerLookup();

        Assert.True(lookup.HasEntry("J?"));
        Assert.True(lookup.HasEntry("Q?"));
        Assert.False(lookup.HasEntry("2?"));
    }

    [Fact]
    public void RhodeIslandUsesThreeCardHands()
    {
        var lookup = new RhodeIslandHoldemLookup();
        var straight = lookup.GetEntry("6s7h8c");
        var trips = lookup.GetEntry("TsTdTh");

        Assert.True(straight.CompareTo(trips) < 0);
        Assert.Equal(Label.Straight, straight.Label);
        Assert.Equal(Label.ThreeOfAKind, trips.Label);
    }
}
