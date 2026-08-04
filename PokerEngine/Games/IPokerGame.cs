using PokerEngine.Hand;

namespace PokerEngine.Games;

/// <summary>
/// Defines a poker game configuration capable of creating hand state instances.
/// </summary>
public interface IPokerGame
{
    /// <summary>
    /// Creates a new state instance for a single poker hand.
    /// </summary>
    /// <returns>
    /// A new <see cref="IPokerHand"/> configured with this game's rules.
    /// </returns>
    IPokerHand CreateHand();
}