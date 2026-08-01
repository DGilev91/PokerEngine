using PokerEngine.Interfaces;

namespace PokerEngine.States;

public sealed class PokerState : IPokerState
{
    public void Initialize(IReadOnlyList<long> stacks)
    {
    }

    public void PostAnte(int seat, long amount)
    {
    }

    public void PostSmallBlind(int seat, long amount)
    {
    }

    public void PostBigBlind(int seat, long amount)
    {
    }

    public void PostStraddle(int seat, long amount)
    {
    }

    public void DealHole(int seat, IReadOnlyList<string> cards)
    {
    }

    public void DealBoard(IReadOnlyList<string> cards, int board = 0)
    {
    }

    public void Fold(int seat)
    {
    }

    public void Check(int seat)
    {
    }

    public void Call(int seat)
    {
    }

    public void Bet(int seat, long amount)
    {
    }

    public void RaiseTo(int seat, long amount)
    {
    }
}