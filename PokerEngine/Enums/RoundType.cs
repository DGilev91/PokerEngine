namespace PokerEngine.Enums;

/// <summary>
/// Defines the current betting round or terminal showdown stage of a poker hand.
/// </summary>
public enum RoundType
{
    /// <summary>
    /// The betting round before any board cards are dealt.
    /// </summary>
    Preflop,

    /// <summary>
    /// The betting round after the first three board cards are dealt.
    /// </summary>
    Flop,

    /// <summary>
    /// The betting round after the fourth board card is dealt.
    /// </summary>
    Turn,

    /// <summary>
    /// The betting round after the fifth board card is dealt.
    /// </summary>
    River,

    /// <summary>
    /// The final stage where remaining hands are evaluated and pots are awarded.
    /// </summary>
    Showdown
}