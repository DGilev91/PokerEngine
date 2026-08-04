using PokerEngine.Enums;

namespace PokerEngine.Rules;

public sealed class BurnCardRules : HandStep
{
    public required RoundType RoundType { get; init; }

    public int CardCount { get; init; } = 1;

    public BurnCardRules() : base(HandStepType.BurnCard)
    {
    }
}