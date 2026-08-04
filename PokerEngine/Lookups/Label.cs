namespace PokerEngine.Lookups;

public enum Label
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush
}

public static class LabelExtensions
{
    public static string ToDisplayName(this Label label)
    {
        return label switch
        {
            Label.HighCard => "High card",
            Label.OnePair => "One pair",
            Label.TwoPair => "Two pair",
            Label.ThreeOfAKind => "Three of a kind",
            Label.Straight => "Straight",
            Label.Flush => "Flush",
            Label.FullHouse => "Full house",
            Label.FourOfAKind => "Four of a kind",
            Label.StraightFlush => "Straight flush",
            _ => throw new ArgumentOutOfRangeException(nameof(label))
        };
    }
}
