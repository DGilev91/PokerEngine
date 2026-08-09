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

    bool IsActive { get; }

    bool CanInitialize { get; }

    bool CanPostAnte { get; }

    bool CanPostDeadBlind { get; }

    bool CanPostBlind { get; }

    bool CanPostStraddle { get; }

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


    GameInitialization Initialize(IReadOnlyList<long> stacks);

    AntePosting PostAnte(int playerIndex, long amount);

    BlindPosting PostBlind(int playerIndex, long amount);

    StraddlePosting PostStraddle(int playerIndex, long amount);

    DeadBlindPosting PostDeadBlind(int playerIndex, long amount);

    HoleDealing DealHole(IReadOnlyList<string> cards);

    Folding Fold();

    Checking Check();

    Calling Call();

    Betting Bet(long amount);

    Raising RaiseTo(long amount);

    RunoutCountSelection SelectRunoutCount(int count);

    BoardDealing DealBoard(IReadOnlyList<string> cards);

    ShowingOrMucking ShowOrMuckHole(IReadOnlyList<string> cards);

    BetCollection CollectBets();

    ChipsPushing PushChips();

    ChipsPulling PullChips();
}