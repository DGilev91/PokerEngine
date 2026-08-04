using System.Collections;

namespace PokerEngine.Utilities;

public readonly record struct Card(Rank Rank, Suit Suit)
{
    public static Card Unknown { get; } = new(Rank.Unknown, Suit.Unknown);

    public bool IsUnknown => Rank == Rank.Unknown || Suit == Suit.Unknown;

    public static IEnumerable<Rank> GetRanks(IEnumerable<Card> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        return cards.Select(card => card.Rank);
    }

    public static IEnumerable<Suit> GetSuits(IEnumerable<Card> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        return cards.Select(card => card.Suit);
    }

    public static bool ArePaired(IEnumerable<Card> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        var ranks = cards.Select(card => card.Rank).ToArray();

        return ranks.Distinct().Count() != ranks.Length;
    }

    public static bool AreSuited(IEnumerable<Card> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        return cards.Select(card => card.Suit).Distinct().Take(2).Count() <= 1;
    }

    public static bool AreRainbow(IEnumerable<Card> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        var suits = cards.Select(card => card.Suit).ToArray();

        return suits.Distinct().Count() == suits.Length;
    }

    public static IReadOnlyList<Card> Clean(Card card)
    {
        return [card];
    }

    public static IReadOnlyList<Card> Clean(string cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        return Parse(cards).ToArray();
    }

    public static IReadOnlyList<Card> Clean(IEnumerable<Card> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        return cards.ToArray();
    }

    public static IEnumerable<Card> Parse(params string[] rawCards)
    {
        ArgumentNullException.ThrowIfNull(rawCards);

        foreach (var raw in rawCards)
        {
            ArgumentNullException.ThrowIfNull(raw);

            var contents = raw.Replace("10", "T", StringComparison.OrdinalIgnoreCase).Replace(",", string.Empty);

            foreach (var content in contents.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            {
                if (content.Length % 2 != 0)
                {
                    throw new FormatException($"The sum of the lengths of valid card representations must be a multiple of 2, unlike '{content}'.");
                }

                for (var index = 0; index < content.Length; index += 2)
                {
                    var rank = RankExtensions.ParseRank(content[index]);
                    var suit = SuitExtensions.ParseSuit(content[index + 1]);

                    yield return new Card(rank, suit);
                }
            }
        }
    }

    public string ToLongString()
    {
        return $"{Rank.ToString().ToUpperInvariant()} OF {Suit.ToString().ToUpperInvariant()}S ({this})";
    }

    public override string ToString()
    {
        return $"{Rank.ToSymbol()}{Suit.ToSymbol()}";
    }
}
