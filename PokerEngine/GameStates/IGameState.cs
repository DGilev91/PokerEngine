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

    bool CanPostDeadBet { get; }

    bool CanPostForceBet { get; }

    bool CanDealHole { get; }

    bool CanFold { get; }

    bool CanCheckOrCall { get; }

    bool CanBetOrRaiseTo { get; }

    bool CanSelectRunoutCount { get; }

    bool CanBurnCard { get; }

    bool CanDealBoard { get; }

    bool CanShowOrMuckHoleCards { get; }

    bool CanKillHand { get; }

    bool CanCollectBets { get; }

    bool CanPushChips { get; }

    bool CanPullChips { get; }


    GameInitialization Initialize(IReadOnlyList<long> stacks, IReadOnlyList<long> deadBets, IReadOnlyList<long> forceBets);

    DeadBetPosting PostDeadBet();

    ForceBetPosting PostForceBet();

    HoleDealing DealHole(IReadOnlyList<Card> cards);

    Folding Fold();

    CheckingOrCalling CheckOrCall();

    BettingOrRaisingTo BetOrRaiseTo(long amount);

    RunoutCountSelection SelectRunoutCount(int count);

    CardBurning BurnCard(Card card);

    BoardDealing DealBoard(IReadOnlyList<Card> cards);

    HoleCardsShowingOrMucking ShowOrMuckHoleCards(IReadOnlyList<Card> cards);

    HandKilling KillHand();

    BetCollection CollectBets();

    ChipsPushing PushChips();

    ChipsPulling PullChips();
}