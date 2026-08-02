namespace PokerEngine.Enums;

public enum StraddleType
{
    /// <summary>
    /// A classic UTG straddle posted by the player immediately after the big blind.
    /// </summary>
    Utg,

    /// <summary>
    /// A Mississippi straddle that may be posted from any permitted position,
    /// often including the button.
    /// </summary>
    Mississippi,

    /// <summary>
    /// A straddle that may only be posted from the button position.
    /// </summary>
    Button,

    /// <summary>
    /// A straddle posted from a specific seat supplied to the engine.
    /// </summary>
    AnyPosition
}