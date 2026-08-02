namespace PokerEngine.Enums;

/// <summary>
/// Defines the supported poker game variants.
/// </summary>
public enum GameType
{
    /// <summary>
    /// Texas Hold'em with two hole cards per player.
    /// </summary>
    TexasHoldem,

    /// <summary>
    /// Four-card Omaha with exactly two hole cards used to build the final hand.
    /// </summary>
    Omaha4c,

    /// <summary>
    /// Five-card Omaha with exactly two hole cards used to build the final hand.
    /// </summary>
    Omaha5c,

    /// <summary>
    /// Six-card Omaha with exactly two hole cards used to build the final hand.
    /// </summary>
    Omaha6c
}