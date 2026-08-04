using PokerEngine.Enums;

namespace PokerEngine.Rules;

public sealed class PokerRules
{
    public required HandMode Mode { get; init; }

    public required GameLimit GameLimit { get; init; }
    public required IReadOnlyList<PostRules> Posts { get; init; }
    public required DealHoleRules DealHole { get; init; }
    public required IReadOnlyList<StreetRules> Streets { get; init; }
    public int InitialBoardCount { get; init; } = 1;
    public int MaxRunoutCount { get; init; } = 1;
}