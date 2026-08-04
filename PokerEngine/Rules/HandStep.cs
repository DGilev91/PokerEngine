using PokerEngine.Enums;

namespace PokerEngine.Rules;

public abstract class HandStep
{
    public HandStepType Type { get; }

    protected HandStep(HandStepType type)
    {
        Type = type;
    }
}