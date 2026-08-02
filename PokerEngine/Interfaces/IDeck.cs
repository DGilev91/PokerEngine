namespace PokerEngine.Interfaces;

/// <summary>
/// Represents a deck of playing cards used by the poker engine.
/// </summary>
public interface IDeck
{
    /// <summary>
    /// Gets the cards that are currently available in the deck.
    /// </summary>
    IReadOnlyList<string> Cards { get; }

    /// <summary>
    /// Gets the number of cards currently remaining in the deck.
    /// </summary>
    int RemainingCount { get; }

    /// <summary>
    /// Randomizes the order of the remaining cards in the deck.
    /// </summary>
    void Shuffle();

    /// <summary>
    /// Removes and returns the next card from the deck.
    /// </summary>
    /// <returns>
    /// The dealt card.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the deck contains no remaining cards.
    /// </exception>
    string Deal();

    /// <summary>
    /// Removes and returns the specified number of cards from the deck.
    /// </summary>
    /// <param name="count">
    /// The number of cards to deal.
    /// </param>
    /// <returns>
    /// The dealt cards in dealing order.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="count"/> is negative.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the deck does not contain enough remaining cards.
    /// </exception>
    IReadOnlyList<string> Deal(int count);

    /// <summary>
    /// Removes the specified card from the deck.
    /// </summary>
    /// <param name="card">
    /// The card to remove.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="card"/> is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the specified card is not available in the deck.
    /// </exception>
    void Take(string card);

    /// <summary>
    /// Removes all specified cards from the deck.
    /// </summary>
    /// <param name="cards">
    /// The cards to remove.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="cards"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the collection contains empty or duplicate cards.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when at least one specified card is not available in the deck.
    /// </exception>
    void Take(IReadOnlyList<string> cards);
}