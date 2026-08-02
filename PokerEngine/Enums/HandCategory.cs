namespace PokerEngine.Enums;

/// <summary>
/// Defines the ranking category of a poker hand.
/// </summary>
public enum HandCategory
{
    /// <summary>
    /// A hand with no matching ranks, straight, or flush.
    /// The highest cards determine its strength.
    /// </summary>
    HighCard = 0,

    /// <summary>
    /// A hand containing two cards of the same rank.
    /// </summary>
    OnePair = 1,

    /// <summary>
    /// A hand containing two different pairs.
    /// </summary>
    TwoPair = 2,

    /// <summary>
    /// A hand containing three cards of the same rank.
    /// </summary>
    ThreeCard = 3,

    /// <summary>
    /// A hand containing five cards in consecutive rank order.
    /// </summary>
    Straight = 4,

    /// <summary>
    /// A hand containing five cards of the same suit.
    /// </summary>
    Flush = 5,

    /// <summary>
    /// A hand containing three cards of one rank and two cards of another rank.
    /// </summary>
    FullHouse = 6,

    /// <summary>
    /// A hand containing four cards of the same rank.
    /// </summary>
    FourCard = 7,

    /// <summary>
    /// A hand containing five consecutive cards of the same suit.
    /// </summary>
    StraightFlush = 8,

    /// <summary>
    /// An ace-high straight flush consisting of Ten, Jack, Queen, King, and Ace.
    /// </summary>
    RoyalFlush = 9
}