using PokerEngine.Enums;

namespace PokerEngine.Rules;

public sealed class StraddleRules : HandStep
{
    public StraddleRules() : base(HandStepType.PostStraddles)
    {
    }

    public required StraddleType StraddleType { get; init; }

    public required IReadOnlyList<long> Amounts { get; init; }

    public required bool IsMandatory { get; init; }
}
