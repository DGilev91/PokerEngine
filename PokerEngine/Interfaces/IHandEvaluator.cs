using PokerEngine.Models;

namespace PokerEngine.Interfaces;

/// <summary>
/// Evaluates a player's poker hand against a board.
/// </summary>
public interface IHandEvaluator
{
    /// <summary>
    /// Evaluates the best available hand that can be formed
    /// from the supplied hole cards and board cards.
    /// </summary>
    /// <param name="holeCards">
    /// The player's private hole cards.
    /// </param>
    /// <param name="boardCards">
    /// The shared community cards on the board.
    /// </param>
    /// <returns>
    /// A <see cref="HandRank"/> containing the hand category,
    /// comparison strength, and best five-card combination.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="holeCards"/> or
    /// <paramref name="boardCards"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the card counts are invalid, a card is invalid,
    /// or duplicate cards are present.
    /// </exception>
    HandRank Evaluate(
        IReadOnlyList<string> holeCards,
        IReadOnlyList<string> boardCards);
}