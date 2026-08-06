using PokerEngine.Models;

namespace PokerEngine.Games;

public interface IGameState
{
    IReadOnlyList<GameEvent> Events { get; }

    bool IsActive { get; }

    bool CanPostAnte { get; }

    bool CanCollectBets { get; }

    bool CanPostBlindOrStraddle { get; }

    bool CanStart { get; }

    bool CanDealHole { get; }

    bool CanFold { get; }

    bool CanCheckOrCall { get; }

    bool CanBetOrRaiseTo { get; }

    bool CanSelectRunoutCount { get; }

    bool CanBurnCard { get; }

    bool CanDealBoard { get; }

    bool CanShowOrMuckCards { get; }

    bool CanKillHand { get; }

    bool CanPushChips { get; }

    bool CanPullChips { get; }

    int? ActorSeatId { get; }

    long CheckingOrCallingAmount { get; }

    long? MinBetOrRaiseToAmount { get; }

    long? MaxBetOrRaiseToAmount { get; }

    void Initialize(IReadOnlyList<long> stacks, int buttonSeatId, Ante ante, IReadOnlyList<BlindOrStraddle> blindsOrStraddles);

    AntePostedEvent PostAnte();

    BetsCollectedEvent CollectBets();

    BlindOrStraddlePostedEvent PostBlindOrStraddle();

    GameStartedEvent Start();

    HoleCardsDealtEvent DealHole(IReadOnlyList<string> cards);

    FoldedEvent Fold();

    CheckedOrCalledEvent CheckOrCall();

    BetOrRaisedToEvent BetOrRaiseTo(long amount);

    RunoutCountSelectedEvent SelectRunoutCount(int? count);

    CardBurnedEvent BurnCard(string card);

    BoardCardsDealtEvent DealBoard(IReadOnlyList<string> cards, int boardIndex = 0);

    CardsShownOrMuckedEvent ShowOrMuckCards(IReadOnlyList<string>? cards = null);

    HandKilledEvent KillHand();

    IReadOnlyList<ChipsPushedEvent> PushChips();

    ChipsPulledEvent PullChips();
}