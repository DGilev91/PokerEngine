using PokerKit.Utilities;

namespace PokerKit.Tests.Utilities;

public sealed class RankTests
{
    [Fact]
    public void Members()
    {
        var symbols = string.Concat(Enum.GetValues<Rank>().Select(rank => rank.ToSymbol()));

        Assert.Equal("A23456789TJQK?", symbols);
    }
}

public sealed class RankOrderTests
{
    [Fact]
    public void Members()
    {
        Assert.Equal("A2345678", ToSymbols(RankOrder.EightOrBetterLow));
        Assert.Equal("23456789TJQKA", ToSymbols(RankOrder.Standard));
        Assert.Equal("6789TJQKA", ToSymbols(RankOrder.ShortDeckHoldem));
        Assert.Equal("A23456789TJQK", ToSymbols(RankOrder.Regular));
        Assert.Equal("TJQKA", ToSymbols(RankOrder.RoyalPoker));
    }

    private static string ToSymbols(IEnumerable<Rank> ranks)
    {
        return string.Concat(ranks.Select(rank => rank.ToSymbol()));
    }
}

public sealed class SuitTests
{
    [Fact]
    public void Members()
    {
        var symbols = string.Concat(Enum.GetValues<Suit>().Select(suit => suit.ToSymbol()));

        Assert.Equal("cdhs?", symbols);
    }
}

public sealed class OriginalDeckTests
{
    [Fact]
    public void Members()
    {
        Assert.Equal(52, Deck.Standard.Count);
        AssertCountEqual(Deck.Standard, Card.Parse(
            "2c3c4c5c6c7c8c9cTcJcQcKcAc",
            "2d3d4d5d6d7d8d9dTdJdQdKdAd",
            "2h3h4h5h6h7h8h9hThJhQhKhAh",
            "2s3s4s5s6s7s8s9sTsJsQsKsAs"));

        Assert.Equal(36, Deck.ShortDeckHoldem.Count);
        AssertCountEqual(Deck.ShortDeckHoldem, Card.Parse(
            "6c7c8c9cTcJcQcKcAc",
            "6d7d8d9dTdJdQdKdAd",
            "6h7h8h9hThJhQhKhAh",
            "6s7s8s9sTsJsQsKsAs"));

        Assert.Equal(52, Deck.Regular.Count);
        AssertCountEqual(Deck.Standard, Card.Parse(
            "Ac2c3c4c5c6c7c8c9cTcJcQcKc",
            "Ad2d3d4d5d6d7d8d9dTdJdQdKd",
            "Ah2h3h4h5h6h7h8h9hThJhQhKh",
            "As2s3s4s5s6s7s8s9sTsJsQsKs"));

        Assert.Equal(3, Deck.KuhnPoker.Count);
        AssertCountEqual(Deck.KuhnPoker, Card.Parse("JsQsKs"));

        Assert.Equal(20, Deck.RoyalPoker.Count);
        AssertCountEqual(Deck.RoyalPoker, Card.Parse(
            "TcJcQcKcAc",
            "TdJdQdKdAd",
            "ThJhQhKhAh",
            "TsJsQsKsAs"));
    }

    private static void AssertCountEqual(IEnumerable<Card> expected, IEnumerable<Card> actual)
    {
        var expectedCounts = expected.GroupBy(card => card).ToDictionary(group => group.Key, group => group.Count());
        var actualCounts = actual.GroupBy(card => card).ToDictionary(group => group.Key, group => group.Count());

        Assert.Equal(expectedCounts.Count, actualCounts.Count);

        foreach (var pair in expectedCounts)
        {
            Assert.True(actualCounts.TryGetValue(pair.Key, out var count));
            Assert.Equal(pair.Value, count);
        }
    }
}
