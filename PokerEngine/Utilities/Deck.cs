namespace PokerKit.Utilities;

public static class Deck
{
    private static readonly Suit[] StandardSuits = [Suit.Club, Suit.Diamond, Suit.Heart, Suit.Spade];

    public static IReadOnlyList<Card> Standard { get; } = Build(RankOrder.Standard, StandardSuits);

    public static IReadOnlyList<Card> ShortDeckHoldem { get; } = Build(RankOrder.ShortDeckHoldem, StandardSuits);

    public static IReadOnlyList<Card> Regular { get; } = Build(RankOrder.Regular, StandardSuits);

    public static IReadOnlyList<Card> KuhnPoker { get; } = Build(RankOrder.KuhnPoker, [Suit.Spade]);

    public static IReadOnlyList<Card> RoyalPoker { get; } = Build(RankOrder.RoyalPoker, StandardSuits);

    private static IReadOnlyList<Card> Build(IEnumerable<Rank> ranks, IEnumerable<Suit> suits)
    {
        return ranks.SelectMany(rank => suits.Select(suit => new Card(rank, suit))).ToArray();
    }
}
