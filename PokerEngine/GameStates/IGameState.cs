using PokerEngine.Enums;
using PokerEngine.Models;

namespace PokerEngine.GameStates;

public interface IGameState
{
    IReadOnlyList<GameEvent> Events { get; }

    IReadOnlyList<Player> Players { get; }

    IReadOnlyList<Pot> Pots { get; }

    IReadOnlyList<IReadOnlyList<Card>> Boards { get; }

    IReadOnlyList<Award> Awards { get; }

    int? ButtonIndex { get; }

    int? RoundIndex { get; }

    int? ActorIndex { get; }

    IReadOnlyList<ActionType> AllowedActions { get; }

    long? CallAmount { get; }

    long? MinBetAmount { get; }

    long? MaxBetAmount { get; }

    long? MinRaiseToAmount { get; }

    long? MaxRaiseToAmount { get; }


    bool CanAddPlayer { get; }

    bool CanSetButton { get; }

    bool CanPost { get; }

    bool CanStart { get; }

    bool CanDealHole { get; }

    bool CanReturnUncalledBet { get; }

    bool CanCollectBets { get; }

    bool CanDealBoard { get; }

    bool CanShowHole { get; }

    bool CanMuck { get; }

    bool CanAwardPots { get; }


    void AddPlayer(long stack);

    void SetButton(int playerIndex);

    void Post(int playerIndex, PostType type, long amount);

    void Start();

    void DealHole(int playerIndex, IReadOnlyList<string> cards);

    void Act(int playerIndex, ActionType type, long amount = 0);

    void ReturnUncalledBet();

    void CollectBets();

    void DealBoard(IReadOnlyList<string> cards, int boardIndex = 0);

    void ShowHole(int playerIndex, IReadOnlyList<string> cards);

    void Muck(int playerIndex);

    void AwardPots();
}