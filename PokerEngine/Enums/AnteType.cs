namespace PokerEngine.Enums;

/// <summary>
/// Defines how an ante is collected from players.
/// </summary>
public enum AnteType
{
    /// <summary>
    /// Every player at the table posts the ante.
    /// </summary>
    EveryPlayer,

    /// <summary>
    /// Only the player in the big blind position posts the ante.
    /// </summary>
    BigBlind,

    /// <summary>
    /// Only the player in the button position posts the ante.
    /// </summary>
    Button
}