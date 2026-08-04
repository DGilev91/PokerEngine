using PokerEngine.Enums;

namespace PokerEngine.Rules;

public sealed class BettingRules : HandStep
{
    public required RoundType RoundType { get; init; }

    public long? BetSize { get; init; }

    public int? MaxRaises { get; init; }

    public BettingRules() : base(HandStepType.Betting)
    {
    }
}