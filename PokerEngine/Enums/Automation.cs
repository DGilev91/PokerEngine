namespace PokerEngine.Enums;

/// <summary>
/// Defines which poker hand operations are performed automatically.
/// Multiple options can be combined.
/// </summary>
[Flags]
public enum Automation
{
    /// <summary>
    /// No actions are performed automatically.
    /// </summary>
    None = 0,

    /// <summary>
    /// Automatically shuffles the deck when the hand is initialized.
    /// </summary>
    ShuffleDeck = 1 << 0,

    /// <summary>
    /// Automatically posts all configured antes.
    /// </summary>
    PostAntes = 1 << 1,

    /// <summary>
    /// Automatically posts the small blind and big blind.
    /// </summary>
    PostBlinds = 1 << 2,

    /// <summary>
    /// Automatically posts configured mandatory straddles.
    /// </summary>
    PostStraddles = 1 << 3,

    /// <summary>
    /// Automatically deals hole cards to all players.
    /// </summary>
    DealHoleCards = 1 << 4,

    /// <summary>
    /// Automatically deals board cards for each street and runout.
    /// </summary>
    DealBoard = 1 << 5,

    /// <summary>
    /// Automatically burns a card before dealing board cards when required.
    /// </summary>
    BurnCards = 1 << 6,

    /// <summary>
    /// Enables all available automation options.
    /// </summary>
    All =
        ShuffleDeck |
        PostAntes |
        PostBlinds |
        PostStraddles |
        DealHoleCards |
        BurnCards |
        DealBoard
}