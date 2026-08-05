using PokerEngine.Enums;
using PokerEngine.Models;

namespace PokerEngine.Games;

public interface IGameState
{
    void InitializeSeats(IReadOnlyList<long> stacks);

    void SetButton(int seatId);

    void PostAnte(Ante ante);

    void PostBlindsOrStraddles(IReadOnlyList<Post> posts);

    void Start();

    void DealHole(int seatId, IReadOnlyList<string> cards);

    void PlayerAction(int seatId, PlayerActionType actionType, long amount = 0);

    void SelectRunoutCount(int count);

    void BurnCard(string card);

    void DealBoard(IReadOnlyList<string> cards, int boardIndex = 0);

    void ShowOrMuckCards(int seatId, IReadOnlyList<string> cards);
}