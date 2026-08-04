namespace PokerKit.Utilities;

public static class RankOrder
{
    public static IReadOnlyList<Rank> Standard { get; } =
    [
        Rank.Deuce, Rank.Trey, Rank.Four, Rank.Five, Rank.Six, Rank.Seven,
        Rank.Eight, Rank.Nine, Rank.Ten, Rank.Jack, Rank.Queen, Rank.King, Rank.Ace
    ];

    public static IReadOnlyList<Rank> ShortDeckHoldem { get; } =
    [
        Rank.Six, Rank.Seven, Rank.Eight, Rank.Nine, Rank.Ten,
        Rank.Jack, Rank.Queen, Rank.King, Rank.Ace
    ];

    public static IReadOnlyList<Rank> Regular { get; } =
    [
        Rank.Ace, Rank.Deuce, Rank.Trey, Rank.Four, Rank.Five, Rank.Six,
        Rank.Seven, Rank.Eight, Rank.Nine, Rank.Ten, Rank.Jack, Rank.Queen, Rank.King
    ];

    public static IReadOnlyList<Rank> EightOrBetterLow { get; } =
    [
        Rank.Ace, Rank.Deuce, Rank.Trey, Rank.Four,
        Rank.Five, Rank.Six, Rank.Seven, Rank.Eight
    ];

    public static IReadOnlyList<Rank> KuhnPoker { get; } =
    [
        Rank.Jack, Rank.Queen, Rank.King
    ];

    public static IReadOnlyList<Rank> RoyalPoker { get; } =
    [
        Rank.Ten, Rank.Jack, Rank.Queen, Rank.King, Rank.Ace
    ];
}
