using PokerEngine.Models;

namespace PokerEngine.Games;

public interface IGameState
{
    IReadOnlyList<GameOperation> Operations { get; }

    IReadOnlyList<Player> Players { get; }

    IReadOnlyList<Pot> Pots { get; }

    IReadOnlyList<Board> Boards { get; }

    int? RoundIndex { get; }

    int? ActorIndex { get; }

    long? CallAmount { get; }

    long? MinBetOrRaiseToAmount { get; }

    long? MaxBetOrRaiseToAmount { get; }

    bool IsActive { get; }

    bool CanInitialize { get; }

    bool CanPostAnte { get; }

    bool CanPostBlindOrStraddle { get; }

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


    GameInitialization Initialize(IReadOnlyList<long> stacks, IReadOnlyList<long> antes, IReadOnlyList<long> blindsOrStraddles);

    AntePosting PostAnte();

    BlindOrStraddlePosting PostBlindOrStraddle();

    HoleDealing DealHole(IReadOnlyList<string> cards);

    Folding Fold();

    CheckingOrCalling CheckOrCall();

    BettingOrRaisingTo BetOrRaiseTo(long amount);

    RunoutCountSelection SelectRunoutCount(int count);

    CardBurning BurnCard(string card);

    BoardDealing DealBoard(IReadOnlyList<string> cards, int boardIndex);

    HoleCardsShowingOrMucking ShowOrMuckHoleCards(IReadOnlyList<string> cards);

    HandKilling KillHand();

    BetCollection CollectBets();

    ChipsPushing PushChips();

    ChipsPulling PullChips();
}