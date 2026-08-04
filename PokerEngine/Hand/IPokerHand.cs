using PokerEngine.Enums;
using PokerEngine.Hand.Events;

namespace PokerEngine.Hand;

public interface IPokerHand
{
    IReadOnlyList<PokerHandEvent> Events { get; }

    void Initialize(IReadOnlyList<long> stacks);

    void PlayerPost(int seatId, PostType postType, long amount);

    void Start();

    void DealHole(int seatId, IReadOnlyList<string> cards);

    void PlayerAction(int seatId, ActionType actionType, long amount = 0);

    void SelectRunoutCount(int count);

    void DealBoard(IReadOnlyList<string> cards, int boardIndex = 0);

    void ShowCards(int seatId,  IReadOnlyList<string> cards);

    string History();
}