using PokerEngine.Enums;

namespace PokerEngine.Models;

public sealed class HandRank
{
    public HandCategory Category { get; }

    public long Strength { get; }

    public IReadOnlyList<string> Cards { get; }

    public HandRank(
        HandCategory category,
        long strength,
        IReadOnlyList<string> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        if (cards.Count != 5)
        {
            throw new ArgumentException(
                "Покерная комбинация должна состоять из пяти карт.",
                nameof(cards));
        }

        Category = category;
        Strength = strength;
        Cards = cards.ToArray();
    }

    public int CompareTo(HandRank? other)
    {
        if (other is null)
        {
            return 1;
        }

        return Strength.CompareTo(other.Strength);
    }
}