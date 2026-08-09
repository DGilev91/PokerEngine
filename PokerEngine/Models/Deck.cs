using PokerEngine.Enums;

namespace PokerEngine.Models;

public static class Deck
{
    public static IReadOnlyList<Card> Standard { get; } = Create(RankOrder.Standard);

    private static IReadOnlyList<Card> Create(IReadOnlyList<CardRank> ranks)
    {
        CardSuit[] suits =
        [
            CardSuit.Club,
            CardSuit.Diamond,
            CardSuit.Heart,
            CardSuit.Spade
        ];

        return ranks
            .SelectMany(rank => suits.Select(suit => new Card(rank, suit)))
            .ToArray();
    }
}