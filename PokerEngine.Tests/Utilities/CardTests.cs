using PokerKit.Utilities;

namespace PokerKit.Tests.Utilities;

public sealed class CardTests
{
    [Fact]
    public void ParseCards()
    {
        var cards = Card.Parse("2c, 8d, 5s, Kh").ToArray();

        Assert.Equal(["2c", "8d", "5s", "Kh"], cards.Select(card => card.ToString()));
    }

    [Fact]
    public void ParseTen()
    {
        var card = Assert.Single(Card.Parse("10s"));

        Assert.Equal("Ts", card.ToString());
    }

    [Fact]
    public void DetectPairedSuitedAndRainbow()
    {
        Assert.True(Card.ArePaired(Card.Parse("2sKh2h")));
        Assert.True(Card.AreSuited(Card.Parse("2hKh3h")));
        Assert.True(Card.AreRainbow(Card.Parse("2sKh3c")));
    }

    [Fact]
    public void UnknownCard()
    {
        Assert.True(Card.Unknown.IsUnknown);
        Assert.Equal("??", Card.Unknown.ToString());
    }
}
