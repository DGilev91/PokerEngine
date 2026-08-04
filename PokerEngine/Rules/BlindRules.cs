using PokerEngine.Enums;

namespace PokerEngine.Rules;
 public sealed class BlindRules : HandStep
{
    public BlindRules(HandStepType type) : base(HandStepType.PostBlinds)
    {
    }

    public required long SmallBlind { get; init; }

    public required long BigBlind { get; init; }
}
