using PokerEngine.Enums;

namespace PokerEngine.Models;

public static class RankOrder
{
    public static IReadOnlyList<CardRank> Standard { get; } =
    [
        CardRank.Deuce,
        CardRank.Trey,
        CardRank.Four,
        CardRank.Five,
        CardRank.Six,
        CardRank.Seven,
        CardRank.Eight,
        CardRank.Nine,
        CardRank.Ten,
        CardRank.Jack,
        CardRank.Queen,
        CardRank.King,
        CardRank.Ace
    ];
}