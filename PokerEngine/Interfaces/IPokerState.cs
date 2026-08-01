namespace PokerEngine.Interfaces;

public interface IPokerState
{

    void Initialize(IReadOnlyList<long> stacks);

    void PostAnte(int seat, long amount);

    void PostSmallBlind(int seat, long amount);

    void PostBigBlind(int seat, long amount);

    void PostStraddle(int seat, long amount);

    void DealHole(int seat, IReadOnlyList<string> cards);

    void DealBoard(IReadOnlyList<string> cards, int board = 0);

    void Fold(int seat);

    void Check(int seat);

    void Call(int seat);

    void Bet(int seat, long amount);

    void RaiseTo(int seat, long amount);
}