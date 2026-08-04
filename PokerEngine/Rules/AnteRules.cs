using PokerEngine.Enums;

namespace PokerEngine.Rules;

public sealed class AnteRules : HandStep
{

    public required AnteType Type { get; init; }

    public required long Amount { get; init; }

    public AnteRules() : base(HandStepType.PostAntes)
    {
    }
}