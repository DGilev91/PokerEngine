using PokerEngine.Models;

namespace PokerEngine.Interfaces;

public interface IPokerHand
{
    IReadOnlyList<Seat> Seats { get; }

    IReadOnlyList<Pot> Pots { get; }

    IReadOnlyList<IReadOnlyList<string>> Boards { get; }

    int RemainingDeckCards { get; }

    void Initialize(IReadOnlyList<long> stacks);

    void PostAnte(int seat, long amount);
    void PostSmallBlind(int seat, long amount);
    void PostBigBlind(int seat, long amount);
    void PostStraddle(int seat, long amount);

    IReadOnlyList<string> DealHole(int seat, IReadOnlyList<string>? cards = null);

    string BurnCard(string? card = null);

    IReadOnlyList<string> DealBoard(int board = 0, IReadOnlyList<string>? cards = null);

    void Fold(int seat);
    void Check(int seat);
    void Call(int seat);
    void Bet(int seat, long amount);
    void RaiseTo(int seat, long amount);
}