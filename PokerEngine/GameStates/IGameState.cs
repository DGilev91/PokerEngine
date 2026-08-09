using PokerEngine.Models;

namespace PokerEngine.GameStates;

public interface IGameState
{
    IReadOnlyList<GameOperation> Operations { get; }

    IReadOnlyList<Player> Players { get; }

    IReadOnlyList<Pot> Pots { get; }

    IReadOnlyList<IReadOnlyList<Card>> Boards { get; }

    int? RoundIndex { get; }

    int? ActorIndex { get; }

    long? CallAmount { get; }

    long? MinBetOrRaiseToAmount { get; }

    long? MaxBetOrRaiseToAmount { get; }


    bool CanInitialize { get; }

    bool CanDealHole { get; }

    bool CanFold { get; }

    bool CanCheck { get; }

    bool CanCall { get; }

    bool CanBet { get; }

    bool CanRaiseTo { get; }

    bool CanSelectRunoutCount { get; }

    bool CanDealBoard { get; }

    bool CanShowOrMuckHoleCards { get; }

    bool CanCollectBets { get; }

    bool CanPushChips { get; }

    bool CanPullChips { get; }


    void Initialize(GameSetup setup);

    void DealHole(IReadOnlyList<string> cards);

    void Fold();

    void Check();

    void Call();

    void Bet(long amount);

    void RaiseTo(long amount);

    void SelectRunoutCount(int count);

    void DealBoard(IReadOnlyList<string> cards);

    void ShowOrMuckHole(IReadOnlyList<string> cards);

    void CollectBets();

    void PushChips();

    void PullChips();
}