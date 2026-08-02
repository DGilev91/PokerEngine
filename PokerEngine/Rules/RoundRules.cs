using PokerEngine.Enums;

namespace PokerEngine.Rules;

/// <summary>
/// Defines the rules for a single betting round.
/// </summary>
public sealed class RoundRules
{
    /// <summary>
    /// Gets the betting round type.
    /// </summary>
    public required RoundType Type { get; init; }

    /// <summary>
    /// Gets the number of cards dealt to each board
    /// at the beginning of this round.
    /// </summary>
    public int BoardCardCount { get; init; }

    /// <summary>
    /// Gets the configured bet size for this round.
    /// </summary>
    /// <remarks>
    /// In fixed-limit games, this value defines the fixed bet
    /// and full raise size.
    ///
    /// In no-limit and pot-limit games, it commonly defines
    /// the minimum full bet size.
    /// </remarks>
    public required long BetSize { get; init; }

    /// <summary>
    /// Gets the maximum number of full raises allowed
    /// during this round.
    /// </summary>
    /// <remarks>
    /// A value of <see langword="null"/> means that there is no limit.
    /// This setting is commonly used only in fixed-limit games.
    /// </remarks>
    public int? MaxRaises { get; init; }
}