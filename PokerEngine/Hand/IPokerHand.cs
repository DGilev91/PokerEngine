using PokerEngine.Enums;
using PokerEngine.Hand.Events;

namespace PokerEngine.Hand;

public sealed record Seats(IReadOnlyList<long> Stacks);

public sealed record Post(int SeatId, PostType Type, long Amount);

public sealed record Posts(IReadOnlyList<Post> Items);

public sealed record Ante(bool Uniform, IReadOnlyList<long> Amounts)
{
    public static Ante None => new(true, [0]);

    public static Ante EveryPlayer(long amount)
    {
        return new Ante(true, [amount]);
    }

    public static Ante ByPosition(params long[] amounts)
    {
        return new Ante(false, amounts);
    }

}

public interface IPokerHand
{
    IReadOnlyList<PokerHandEvent> Events { get; }

    void Initialize(Seats seats, Posts posts, Ante ante);

    void DealHole(int seatId, IReadOnlyList<string> cards);

    void Start();

    void PlayerAction(int seatId, ActionType actionType, long amount = 0);

    void DealBoard(IReadOnlyList<string> cards, int boardIndex = 0);

    void ShowCards(int seatId,  IReadOnlyList<string> cards);

    string History();

    void SelectRunoutCount(int count);
}