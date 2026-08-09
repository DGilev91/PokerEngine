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

    public override string ToString()
    {
        return $"{RankToChar(Rank)}{SuitToChar(Suit)}";
    }

    public static Card Parse(string value)
    {
        if (!TryParse(value, out Card? card))
        {
            throw new FormatException($"Invalid card: {value}");
        }

        return card;
    }

    public static bool TryParse(string? value, out Card? card)
    {
        card = null;

        if (string.IsNullOrWhiteSpace(value) || value.Length != 2)
        {
            return false;
        }

        if (!TryParseRank(value[0], out CardRank rank) || !TryParseSuit(value[1], out CardSuit suit))
        {
            return false;
        }

        card = new Card(rank, suit);

        return true;
    }

    private static char RankToChar(CardRank rank) => rank switch
    {
        CardRank.Unknown => 'X',
        CardRank.Ace => 'A',
        CardRank.Deuce => '2',
        CardRank.Trey => '3',
        CardRank.Four => '4',
        CardRank.Five => '5',
        CardRank.Six => '6',
        CardRank.Seven => '7',
        CardRank.Eight => '8',
        CardRank.Nine => '9',
        CardRank.Ten => 'T',
        CardRank.Jack => 'J',
        CardRank.Queen => 'Q',
        CardRank.King => 'K',
        _ => throw new ArgumentOutOfRangeException(nameof(rank))
    };

    private static char SuitToChar(CardSuit suit) => suit switch
    {
        CardSuit.Unknown => 'X',
        CardSuit.Club => 'c',
        CardSuit.Diamond => 'd',
        CardSuit.Heart => 'h',
        CardSuit.Spade => 's',
        _ => throw new ArgumentOutOfRangeException(nameof(suit))
    };

    private static bool TryParseRank(char value, out CardRank rank)
    {
        CardRank? parsed = value switch
        {
            'X' => CardRank.Unknown,
            'A' => CardRank.Ace,
            '2' => CardRank.Deuce,
            '3' => CardRank.Trey,
            '4' => CardRank.Four,
            '5' => CardRank.Five,
            '6' => CardRank.Six,
            '7' => CardRank.Seven,
            '8' => CardRank.Eight,
            '9' => CardRank.Nine,
            'T' => CardRank.Ten,
            'J' => CardRank.Jack,
            'Q' => CardRank.Queen,
            'K' => CardRank.King,
            _ => null
        };

        rank = parsed ?? default;

        return parsed.HasValue;
    }

    private static bool TryParseSuit(char value, out CardSuit suit)
    {
        CardSuit? parsed = value switch
        {
            'X' => CardSuit.Unknown,
            'c' => CardSuit.Club,
            'd' => CardSuit.Diamond,
            'h' => CardSuit.Heart,
            's' => CardSuit.Spade,
            _ => null
        };

        suit = parsed ?? default;

        return parsed.HasValue;
    }
}