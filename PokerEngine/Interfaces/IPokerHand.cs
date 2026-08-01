namespace PokerEngine.Interfaces;

public interface IPokerHand
{
    IPokerState State { get; }

    IDeck Deck { get; }

    void Initialize(IReadOnlyList<long> stacks);

    void PostAnte(int seat, long amount);

    void PostSmallBlind(int seat, long amount);

    void PostBigBlind(int seat, long amount);

    void PostStraddle(int seat, long amount);

    IReadOnlyList<string> DealHole(int seat);

    IReadOnlyList<string> DealBoard(int board = 0);

    void Fold(int seat);

    void Check(int seat);

    void Call(int seat);

    void Bet(int seat, long amount);

    void RaiseTo(int seat, long amount);
}