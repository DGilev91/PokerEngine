using PokerEngine.Utilities;

namespace PokerEngine.Tests.Utilities;

public sealed class DeckTests
{
    [Fact]
    public void DeckSizesMatchPokerKit()
    {
        Assert.Equal(52, Deck.Standard.Count);
        Assert.Equal(36, Deck.ShortDeckHoldem.Count);
        Assert.Equal(52, Deck.Regular.Count);
        Assert.Equal(3, Deck.KuhnPoker.Count);
        Assert.Equal(20, Deck.RoyalPoker.Count);
    }

    [Fact]
    public void StandardDeckOrderMatchesPokerKit()
    {
        Assert.Equal(["2c", "2d", "2h", "2s", "3c", "3d"], Deck.Standard.Take(6).Select(card => card.ToString()));
    }

    [Fact]
    public void RegularDeckStartsWithAce()
    {
        Assert.Equal(["Ac", "Ad", "Ah", "As", "2c", "2d"], Deck.Regular.Take(6).Select(card => card.ToString()));
    }
}
