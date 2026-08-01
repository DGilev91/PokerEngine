using PokerEngine.Cards;
using PokerEngine.Interfaces;

namespace PokerEngine.Tests.Cards;

public sealed class DeckTests
{
    [Fact]
    public void Constructor_ShouldCreateStandard52CardDeck()
    {
        // Act
        IDeck deck = new Deck();

        // Assert
        Assert.Equal(52, deck.Cards.Count);
        Assert.Equal(52, deck.RemainingCount);
    }

    [Fact]
    public void Constructor_ShouldCreateUniqueCards()
    {
        // Arrange
        IDeck deck = new Deck();

        // Act
        int uniqueCardCount = deck.Cards.Distinct().Count();

        // Assert
        Assert.Equal(52, uniqueCardCount);
    }

    [Fact]
    public void Deal_ShouldReturnAndRemoveOneCard()
    {
        // Arrange
        IDeck deck = new Deck();

        // Act
        string card = deck.Deal();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(card));
        Assert.Equal(51, deck.RemainingCount);
        Assert.DoesNotContain(card, deck.Cards);
    }

    [Fact]
    public void Deal_WithCount_ShouldReturnAndRemoveCards()
    {
        // Arrange
        IDeck deck = new Deck();

        // Act
        IReadOnlyList<string> cards = deck.Deal(5);

        // Assert
        Assert.Equal(5, cards.Count);
        Assert.Equal(47, deck.RemainingCount);

        foreach (string card in cards)
        {
            Assert.DoesNotContain(card, deck.Cards);
        }
    }

    [Fact]
    public void Deal_ShouldNotReturnDuplicateCards()
    {
        // Arrange
        IDeck deck = new Deck();

        // Act
        IReadOnlyList<string> cards = deck.Deal(52);

        // Assert
        Assert.Equal(52, cards.Count);
        Assert.Equal(52, cards.Distinct().Count());
        Assert.Equal(0, deck.RemainingCount);
    }

    [Fact]
    public void Deal_WhenDeckIsEmpty_ShouldThrow()
    {
        // Arrange
        IDeck deck = new Deck();
        deck.Deal(52);

        // Act
        Action action = () => deck.Deal();

        // Assert
        Assert.Throws<InvalidOperationException>(action);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Deal_WithNegativeCount_ShouldThrow(int count)
    {
        // Arrange
        IDeck deck = new Deck();

        // Act
        Action action = () => deck.Deal(count);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void Deal_MoreThanRemainingCards_ShouldThrow()
    {
        // Arrange
        IDeck deck = new Deck();
        deck.Deal(50);

        // Act
        Action action = () => deck.Deal(3);

        // Assert
        Assert.Throws<InvalidOperationException>(action);
    }
}