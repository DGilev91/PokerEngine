using PokerEngine.Enums;

namespace PokerEngine.Evaluation;

/// <summary>
/// Represents the evaluated rank of a five-card poker hand.
/// </summary>
public sealed class HandRank : IComparable<HandRank>
{
    /// <summary>
    /// Gets the category of the evaluated hand.
    /// </summary>
    public HandCategory Category { get; }

    /// <summary>
    /// Gets the numeric strength used to compare this hand with other hands.
    /// </summary>
    /// <remarks>
    /// A greater value represents a stronger hand.
    /// </remarks>
    public long Strength { get; }

    /// <summary>
    /// Gets the five cards forming the best evaluated poker hand.
    /// </summary>
    public IReadOnlyList<string> Cards { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HandRank"/> class.
    /// </summary>
    /// <param name="category">
    /// The category of the evaluated hand.
    /// </param>
    /// <param name="strength">
    /// The numeric strength of the evaluated hand.
    /// </param>
    /// <param name="cards">
    /// The five cards forming the best hand.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="cards"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="cards"/> does not contain exactly five cards.
    /// </exception>
    public HandRank(
        HandCategory category,
        long strength,
        IReadOnlyList<string> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        if (cards.Count != 5)
        {
            throw new ArgumentException(
                "A poker hand must contain exactly five cards.",
                nameof(cards));
        }

        Category = category;
        Strength = strength;
        Cards = [.. cards];
    }

    /// <summary>
    /// Compares this hand rank with another hand rank.
    /// </summary>
    /// <param name="other">
    /// The hand rank to compare with this instance.
    /// </param>
    /// <returns>
    /// A positive value when this hand is stronger,
    /// zero when both hands have equal strength,
    /// or a negative value when this hand is weaker.
    /// </returns>
    public int CompareTo(HandRank? other)
    {
        if (other is null)
        {
            return 1;
        }

        return Strength.CompareTo(other.Strength);
    }
}