using System.Collections.ObjectModel;

namespace PokerEngine.Cards;

internal static class CardTable
{
    public const int RankCount = 13;
    public const int SuitCount = 4;
    public const int CardCount = 52;

    public static IReadOnlyList<char> Ranks { get; } =
    [
        '2', '3', '4', '5', '6', '7',
        '8', '9', 'T', 'J', 'Q', 'K', 'A'
    ];

    public static IReadOnlyList<char> Suits { get; } =
    [
        'c', 'd', 'h', 's'
    ];

    public static IReadOnlyList<string> Cards { get; } =
        CreateCards();

    public static int Encode(string card)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(card);

        if (card.Length != 2)
        {
            throw new ArgumentException(
                $"Invalid card: {card}.",
                nameof(card));
        }

        int rank = GetRankIndex(card[0]);

        if (rank < 0)
        {
            throw new ArgumentException(
                $"Unknown card rank: {card[0]}.",
                nameof(card));
        }

        int suit = GetSuitIndex(card[1]);

        if (suit < 0)
        {
            throw new ArgumentException(
                $"Unknown card suit: {card[1]}.",
                nameof(card));
        }

        return Encode(rank, suit);
    }

    public static int Encode(
        int rank,
        int suit)
    {
        ValidateRank(rank);
        ValidateSuit(suit);

        return suit * RankCount + rank;
    }

    public static bool TryEncode(
        string? card,
        out int encodedCard)
    {
        encodedCard = default;

        if (card is not { Length: 2 })
        {
            return false;
        }

        int rank = GetRankIndex(card[0]);

        if (rank < 0)
        {
            return false;
        }

        int suit = GetSuitIndex(card[1]);

        if (suit < 0)
        {
            return false;
        }

        encodedCard = suit * RankCount + rank;

        return true;
    }

    public static string Decode(int card)
    {
        ValidateCard(card);

        return Cards[card];
    }

    public static int GetRank(int card)
    {
        ValidateCard(card);

        return card % RankCount;
    }

    public static int GetSuit(int card)
    {
        ValidateCard(card);

        return card / RankCount;
    }

    public static char GetRankChar(int card)
    {
        return Ranks[GetRank(card)];
    }

    public static char GetSuitChar(int card)
    {
        return Suits[GetSuit(card)];
    }

    public static bool IsValid(int card)
    {
        return (uint)card < CardCount;
    }

    public static bool IsValid(string? card)
    {
        return TryEncode(
            card,
            out _);
    }

    public static int GetRankIndex(char rank)
    {
        return rank switch
        {
            '2' => 0,
            '3' => 1,
            '4' => 2,
            '5' => 3,
            '6' => 4,
            '7' => 5,
            '8' => 6,
            '9' => 7,
            'T' => 8,
            'J' => 9,
            'Q' => 10,
            'K' => 11,
            'A' => 12,
            _ => -1
        };
    }

    public static int GetSuitIndex(char suit)
    {
        return suit switch
        {
            'c' => 0,
            'd' => 1,
            'h' => 2,
            's' => 3,
            _ => -1
        };
    }

    private static ReadOnlyCollection<string> CreateCards()
    {
        string[] cards = new string[CardCount];

        for (int suit = 0;
             suit < SuitCount;
             suit++)
        {
            for (int rank = 0;
                 rank < RankCount;
                 rank++)
            {
                int card =
                    suit * RankCount + rank;

                cards[card] = string.Concat(
                    Ranks[rank],
                    Suits[suit]);
            }
        }

        return Array.AsReadOnly(cards);
    }

    private static void ValidateCard(int card)
    {
        if (!IsValid(card))
        {
            throw new ArgumentOutOfRangeException(
                nameof(card),
                card,
                $"Card value must be between 0 and {CardCount - 1}.");
        }
    }

    private static void ValidateRank(int rank)
    {
        if ((uint)rank >= RankCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rank),
                rank,
                $"Rank value must be between 0 and {RankCount - 1}.");
        }
    }

    private static void ValidateSuit(int suit)
    {
        if ((uint)suit >= SuitCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(suit),
                suit,
                $"Suit value must be between 0 and {SuitCount - 1}.");
        }
    }
}