using PokerEngine.Enums;

namespace PokerEngine.Rules;

/// <summary>
/// Defines the ante rules for a poker game.
/// </summary>
public sealed class AnteRules
{
    /// <summary>
    /// Gets the position or group of players responsible for posting the ante.
    /// </summary>
    public required AnteType Type { get; init; }

    /// <summary>
    /// Gets the required ante amount.
    /// </summary>
    public required long Amount { get; init; }
}