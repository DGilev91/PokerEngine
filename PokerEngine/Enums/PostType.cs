namespace PokerEngine.Enums;

/// <summary>
/// Defines the types of forced contributions that may be posted before or during hand setup.
/// </summary>
public enum PostType
{
    /// <summary>
    /// A mandatory ante contribution that does not count toward the current betting round.
    /// </summary>
    Ante,

    /// <summary>
    /// The mandatory small blind live wager.
    /// </summary>
    SmallBlind,

    /// <summary>
    /// The mandatory big blind live wager.
    /// </summary>
    BigBlind,

    /// <summary>
    /// A dead blind contribution that does not count toward the current live wager.
    /// It is commonly used when a player returns to the table after missing blinds.
    /// </summary>
    DeadBlind,

    /// <summary>
    /// An additional live blind posted outside the normal blind positions.
    /// </summary>
    ExtraBlind,

    /// <summary>
    /// A voluntary or mandatory live wager posted before the cards are dealt.
    /// </summary>
    Straddle
}