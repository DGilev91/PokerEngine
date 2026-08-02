using PokerEngine.States;

namespace PokerEngine.Games;

/// <summary>
/// Represents a poker game definition that can create new hand states.
/// </summary>
public abstract class PokerGame : IPokerGame
{
    /// <inheritdoc />
    public abstract IPokerState CreateState();
}