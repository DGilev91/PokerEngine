using PokerEngine.Enums;

namespace PokerEngine.Models;

public sealed class Card
{
    public CardSuit Suit { get; }

    public CardRank Rank { get; }

    public Card(CardRank rank, CardSuit suit)
    {
        Rank = rank;
        Suit = suit;
    }
}