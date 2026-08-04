namespace PokerKit.Utilities;

public enum Rank
{
    Ace,
    Deuce,
    Trey,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Unknown
}

public static class RankExtensions
{
    public static char ToSymbol(this Rank rank)
    {
        return rank switch
        {
            Rank.Ace => 'A',
            Rank.Deuce => '2',
            Rank.Trey => '3',
            Rank.Four => '4',
            Rank.Five => '5',
            Rank.Six => '6',
            Rank.Seven => '7',
            Rank.Eight => '8',
            Rank.Nine => '9',
            Rank.Ten => 'T',
            Rank.Jack => 'J',
            Rank.Queen => 'Q',
            Rank.King => 'K',
            Rank.Unknown => '?',
            _ => throw new ArgumentOutOfRangeException(nameof(rank))
        };
    }

    public static Rank ParseRank(char value)
    {
        return char.ToUpperInvariant(value) switch
        {
            'A' => Rank.Ace,
            '2' => Rank.Deuce,
            '3' => Rank.Trey,
            '4' => Rank.Four,
            '5' => Rank.Five,
            '6' => Rank.Six,
            '7' => Rank.Seven,
            '8' => Rank.Eight,
            '9' => Rank.Nine,
            'T' => Rank.Ten,
            'J' => Rank.Jack,
            'Q' => Rank.Queen,
            'K' => Rank.King,
            '?' => Rank.Unknown,
            _ => throw new FormatException($"'{value}' is not a valid rank.")
        };
    }
}
