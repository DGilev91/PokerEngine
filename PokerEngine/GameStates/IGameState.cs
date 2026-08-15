using PokerEngine.Enums;
using PokerEngine.Models;

namespace PokerEngine.GameStates;

public interface IGameState
{
    IReadOnlyList<GameEvent> Events { get; }

    IReadOnlyList<Player> Players { get; }

    IReadOnlyList<Pot> Pots { get; }

    IReadOnlyList<IReadOnlyList<Card>> Boards { get; }

    IReadOnlyList<PlayerHand> Hands { get; }

    IReadOnlyList<Award> Awards { get; }

    int? ButtonSeatIndex { get; }

    int? RoundIndex { get; }

    int? ActorSeatIndex { get; }

    IReadOnlyList<ActionType> AllowedActions { get; }

    long? CallAmount { get; }

    long? MinBetAmount { get; }

    long? MaxBetAmount { get; }

    long? MinRaiseToAmount { get; }

    long? MaxRaiseToAmount { get; }

    void AddPlayer(long stack);

    void SetButton(int seatIndex);

    void Post(int seatIndex, PostType type, long amount);

    void Start();

    void DealHole(int seatIndex, IReadOnlyList<string> cards);

    void Act(int seatIndex, ActionType type, long amount = 0);

    void ReturnUncalledBet();

    void CollectBets();

    void DealBoard(IReadOnlyList<string> cards, int boardIndex = 0);

    void ShowHole(int seatIndex, IReadOnlyList<string> cards);

    void Muck(int seatIndex);

    void EvaluateHands();

    void AwardPots();
}