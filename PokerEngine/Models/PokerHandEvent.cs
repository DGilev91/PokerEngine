using PokerEngine.Enums;

namespace PokerEngine.Models;

public abstract record PokerHandEvent;

public sealed record NewHandEvent : PokerHandEvent;

public sealed record SeatsEvent(IReadOnlyList<long> stacks) : PokerHandEvent;

public sealed record PlayerPostedEvent(int seatId, PostType postType, long amount, bool isAllIn) : PokerHandEvent;

public sealed record HandStartedEvent : PokerHandEvent;

public sealed record HoleCardsEvent(int seatId, IReadOnlyList<string> cards) : PokerHandEvent;

public sealed record RunoutCountEvent(int count) : PokerHandEvent;

public sealed record BoardEvent(RoundType round, int boardIndex, IReadOnlyList<string> cards) : PokerHandEvent;

public sealed record PlayerTurnEvent(int seatId, IReadOnlyList<ActionType> actions, long callAmount, long minBet, long maxBet, long minRaiseTo, long maxRaiseTo) : PokerHandEvent;

public sealed record PlayerActionEvent(int seatId, ActionType actionType, long amount, long amountTo, bool isAllIn) : PokerHandEvent;

public sealed record UncalledBetReturnedEvent(int seatId, long amount) : PokerHandEvent;

public sealed record ShowCardsEvent(int seatId, IReadOnlyList<string> cards) : PokerHandEvent;

public sealed record MuckCardsEvent(int seatId) : PokerHandEvent;

public sealed record HandEvaluatedEvent(int seatId, int boardIndex, HandCategory category, IReadOnlyList<string> bestCards) : PokerHandEvent;

public sealed record PotAwardedEvent(int potIndex, int boardIndex, int seatId, long amount) : PokerHandEvent;

public sealed record EndHandEvent : PokerHandEvent;