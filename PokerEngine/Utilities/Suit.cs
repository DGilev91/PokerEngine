namespace PokerKit.Utilities;

public enum Suit
{
    Club,
    Diamond,
    Heart,
    Spade,
    Unknown
}

public static class SuitExtensions
{
    public static char ToSymbol(this Suit suit)
    {
        return suit switch
        {
            Suit.Club => 'c',
            Suit.Diamond => 'd',
            Suit.Heart => 'h',
            Suit.Spade => 's',
            Suit.Unknown => '?',
            _ => throw new ArgumentOutOfRangeException(nameof(suit))
        };
    }

    public static Suit ParseSuit(char value)
    {
        return char.ToLowerInvariant(value) switch
        {
            'c' => Suit.Club,
            'd' => Suit.Diamond,
            'h' => Suit.Heart,
            's' => Suit.Spade,
            '?' => Suit.Unknown,
            _ => throw new FormatException($"'{value}' is not a valid suit.")
        };
    }
}
