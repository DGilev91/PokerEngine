namespace PokerEngine.Cards;

public static class CardTable
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
                $"Некорректная карта: {card}.",
                nameof(card));
        }

        int rank = IndexOf(Ranks, card[0]);

        if (rank < 0)
        {
            throw new ArgumentException(
                $"Неизвестный ранг карты: {card}.",
                nameof(card));
        }

        int suit = IndexOf(Suits, card[1]);

        if (suit < 0)
        {
            throw new ArgumentException(
                $"Неизвестная масть карты: {card}.",
                nameof(card));
        }

        return suit * RankCount + rank;
    }

    public static string Decode(int card)
    {
        Validate(card);

        return Cards[card];
    }

    public static int GetRank(int card)
    {
        Validate(card);

        return card % RankCount;
    }

    public static int GetSuit(int card)
    {
        Validate(card);

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
        if (string.IsNullOrWhiteSpace(card) ||
            card.Length != 2)
        {
            return false;
        }

        return IndexOf(Ranks, card[0]) >= 0 &&
               IndexOf(Suits, card[1]) >= 0;
    }

    private static IReadOnlyList<string> CreateCards()
    {
        var cards = new List<string>(CardCount);

        foreach (char suit in Suits)
        {
            foreach (char rank in Ranks)
            {
                cards.Add(string.Concat(rank, suit));
            }
        }

        return cards.AsReadOnly();
    }

    private static int IndexOf(
        IReadOnlyList<char> values,
        char value)
    {
        for (int index = 0; index < values.Count; index++)
        {
            if (values[index] == value)
            {
                return index;
            }
        }

        return -1;
    }

    private static void Validate(int card)
    {
        if (!IsValid(card))
        {
            throw new ArgumentOutOfRangeException(
                nameof(card),
                $"Карта должна иметь значение от 0 до {CardCount - 1}.");
        }
    }
}