using PokerEngine.Enums;
using PokerEngine.Models;

namespace PokerEngine.Interfaces;

public interface IPokerState
{
    IReadOnlyList<PokerHandEvent> Events { get; }

    IReadOnlyList<Seat> Seats { get; }

    PotState PotState { get; }

    IReadOnlyList<IReadOnlyList<string>> Boards { get; }

    HandState State { get; }

    RoundType Round { get; }

    void Initialize(IReadOnlyList<long> stacks);

    void PlayerPost(int seatId, PostType postType, long amount);

    void Start();

    void DealHole(int seatId, IReadOnlyList<string>? cards = null);

    void PlayerAction(int seatId, ActionType actionType, long amount = 0);

    void SetRunoutCount(int count);

    void DealBoard(int boardIndex = 0, IReadOnlyList<string>? cards = null);

    void ShowCards(int seatId, IReadOnlyList<string> cards);
}