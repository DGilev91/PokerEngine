using PokerEngine.Enums;
using PokerEngine.Models;

namespace PokerEngine.Interfaces;

public interface IPokerHand
{
    IReadOnlyList<PokerHandEvent> Events { get; }

    IReadOnlyList<Seat> Seats { get; }

    PotState PotState { get; }

    IReadOnlyList<IReadOnlyList<string>> Boards { get; }

    RoundType Round { get; }

    void Initialize(IReadOnlyList<long> stacks);

    void Start();

    void PlayerPost(int seatId, PostType postType, long amount);

    IReadOnlyList<string> DealHole(int seatId, IReadOnlyList<string>? cards = null);

    void PlayerAction(int seatId, ActionType actionType, long amount = 0);

    IReadOnlyList<string> DealBoard(int boardIndex = 0, IReadOnlyList<string>? cards = null);

    void ShowCards(int seatId, IReadOnlyList<string> cards);

    void MuckCards(int seatId);
}