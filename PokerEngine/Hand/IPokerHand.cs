using PokerEngine.Enums;

namespace PokerEngine.Hand;

public sealed record Seats(IReadOnlyList<long> Stacks);

public sealed record Post(int SeatId, BlindType Type, long Amount);

public sealed record Posts(IReadOnlyList<Post> Items);

public sealed record Ante(bool Uniform, IReadOnlyList<long> Amounts)
{
    public static Ante None => new(true, [0]);

    public static Ante EveryPlayer(long amount)
    {
        return new(true, [amount]);
    }

    public static Ante ByPosition(params long[] amounts)
    {
        return new(false, amounts);
    }
}

public interface IPokerHand
{
    IReadOnlyList<HandEvent> Events { get; }

    HandRequest? CurrentRequest { get; }

    void Initialize(Seats seats, Posts posts, Ante ante);

    void DealHole(int seatId, IReadOnlyList<string> cards);

    void Start();

    void PlayerAction(int seatId, PlayerActionType actionType, long amount = 0);

    void DealBoard(IReadOnlyList<string> cards, int boardIndex = 0);

    void ShowCards(int seatId, IReadOnlyList<string> cards);

    void SelectRunoutCount(int count);

    string History();
}