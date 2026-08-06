using PokerEngine.Enums;
using PokerEngine.Models;

namespace PokerEngine.Games;

public interface IGameState
{
    IReadOnlyList<GameOperation> Operations { get; }

    IReadOnlyList<long> StartingStacks { get; }

    IReadOnlyList<long> Stacks { get; }

    IReadOnlyList<bool> Statuses { get; }

    IReadOnlyList<long> Contributions { get; }

    IReadOnlyList<long> RoundBets { get; }

    IReadOnlyList<Pot> Pots { get; }

    IReadOnlyList<Board> Boards { get; }

    RoundType? CurrentRound { get; }

    bool IsActive { get; }

    bool CanPostAnte { get; }

    bool CanPostBlindOrStraddle { get; }

    bool CanDealHole { get; }

    bool CanFold { get; }

    bool CanCheckOrCall { get; }

    bool CanBetOrRaiseTo { get; }

    bool CanSelectRunoutCount { get; }

    bool CanBurnCard { get; }

    bool CanDealBoard { get; }

    bool CanShowOrMuckCards { get; }

    bool CanCollectBets { get; }

    bool CanPushChips { get; }

    int? ActorSeatId { get; }

    long CheckingOrCallingAmount { get; }

    long? MinBetOrRaiseToAmount { get; }

    long? MaxBetOrRaiseToAmount { get; }

    GameInitialization Initialize(IReadOnlyList<long> stacks, Ante ante, IReadOnlyList<BlindOrStraddle> blindsOrStraddles);

    AntePosting PostAnte();

    BlindOrStraddlePosting PostBlindOrStraddle();

    HoleDealing DealHole(IReadOnlyList<string> cards);

    Folding Fold();

    CheckingOrCalling CheckOrCall();

    BettingOrRaisingTo BetOrRaiseTo(long amount);

    RunoutCountSelection SelectRunoutCount(int? count);

    CardBurning BurnCard(string card);

    BoardDealing DealBoard(IReadOnlyList<string> cards, int boardIndex = 0);

    HoleCardsShowingOrMucking ShowOrMuckCards(IReadOnlyList<string>? cards = null);

    BetCollection CollectBets();

    IReadOnlyList<ChipsPushing> PushChips();
}