using PokerEngine.Enums;

namespace PokerEngine.Models;

public abstract record PokerHandEvent;

public sealed record NewHandEvent : PokerHandEvent;

public sealed record EndHandEvent : PokerHandEvent;

public sealed record SeatsEvent(
    IReadOnlyList<long> stacks
) : PokerHandEvent;

public sealed record HandStartedEvent : PokerHandEvent;

public sealed record HoleCardsEvent(
    int seatId,
    IReadOnlyList<string> cards
) : PokerHandEvent;

public sealed record RoundEvent(
    RoundType round,
    IReadOnlyList<string> cards
) : PokerHandEvent;

public sealed record PlayerPostedEvent(
    int seatId,
    PostType postType,
    long amount
) : PokerHandEvent;

public sealed record PlayerTurnEvent(
    int seatId
) : PokerHandEvent;

public sealed record PlayerActionEvent(
    int seatId,
    ActionType actionType,
    long amount,
    long amountTo,
    bool isAllIn
) : PokerHandEvent;

public sealed record UncalledBetReturnedEvent(
    int seatId,
    long amount
) : PokerHandEvent;

public sealed record ShowCardsEvent(
    int seatId,
    IReadOnlyList<string> cards,
    HandCategory category,
    IReadOnlyList<string> bestCards
) : PokerHandEvent;

public sealed record MuckCardsEvent(
    int seatId
) : PokerHandEvent;

public sealed record PotAwardedEvent(
    int potIndex,
    int boardIndex,
    int seatId,
    long amount
) : PokerHandEvent;