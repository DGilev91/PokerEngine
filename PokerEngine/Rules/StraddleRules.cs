using PokerEngine.Enums;

namespace PokerEngine.Rules;

/// <summary>
/// Defines the straddle rules for a poker game.
/// </summary>
public sealed class StraddleRules
{
    /// <summary>
    /// Gets the rule used to determine which position may post
    /// the first straddle.
    /// </summary>
    public required StraddleType Type { get; init; }

    /// <summary>
    /// Gets the permitted amounts for consecutive straddles.
    /// </summary>
    /// <remarks>
    /// The first item is the initial straddle amount.
    /// The second item is the first restraddle amount,
    /// followed by any additional restraddles.
    ///
    /// An empty collection means that straddles are disabled.
    /// </remarks>
    public required IReadOnlyList<long> Amounts { get; init; }

    /// <summary>
    /// Gets a value indicating whether the first straddle is mandatory.
    /// </summary>
    public bool IsMandatory { get; init; }
}
