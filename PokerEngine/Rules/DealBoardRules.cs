using PokerEngine.Enums;

namespace PokerEngine.Rules;

public sealed class DealBoardRules : HandStep
{
    public required RoundType RoundType { get; init; }

    public required int CardCount { get; init; }

    public DealBoardRules() : base(HandStepType.DealBoard)
    {
    }
}