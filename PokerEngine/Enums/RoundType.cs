namespace PokerEngine.Enums;

public enum RoundType
{
    /// <summary>
    /// No betting round is currently active.
    /// </summary>
    None,

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