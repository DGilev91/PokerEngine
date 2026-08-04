using PokerEngine.Enums;

namespace PokerEngine.Rules;

public sealed class PokerRules
{
    public required HandMode Mode { get; init; }

    public required GameType GameType { get; init; }

    public required GameLimit GameLimit { get; init; }

    public required IReadOnlyList<HandStep> Steps { get; init; }

    public int InitialBoardCount { get; init; } = 1;

    public int MaxRunoutCount { get; init; } = 1;
}