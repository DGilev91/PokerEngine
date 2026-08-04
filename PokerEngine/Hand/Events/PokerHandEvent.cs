using PokerEngine.Enums;

namespace PokerEngine.Hand.Events;

public abstract record PokerHandEvent;

public sealed record NewHandEvent : PokerHandEvent;

public sealed record SeatsEvent(IReadOnlyList<long> Stacks) : PokerHandEvent;

public sealed record AntePostedEvent(int SeatId, long Amount, bool IsAllIn) : PokerHandEvent;

public sealed record BlindPostedEvent(int SeatId, BlindType Type, long Amount, bool IsAllIn) : PokerHandEvent;

public sealed record StraddlePostedEvent(int SeatId, long Amount, bool IsAllIn) : PokerHandEvent;

public sealed record HandStartedEvent : PokerHandEvent;

public sealed record HoleCardsEvent(int SeatId, IReadOnlyList<string> Cards) : PokerHandEvent;

public sealed record WaitingRunoutEvent(int Count) : PokerHandEvent;

public sealed record RunoutCountSelectedEvent(int Count) : PokerHandEvent;

public sealed record BoardEvent(RoundType Round, int BoardIndex, IReadOnlyList<string> Cards) : PokerHandEvent;

public sealed record PlayerTurnEvent(int SeatId, IReadOnlyList<ActionType> Actions, long CallAmount, long MinBet, long MaxBet, long MinRaiseTo, long MaxRaiseTo) : PokerHandEvent;

public sealed record FoldEvent(int SeatId) : PokerHandEvent;

public sealed record CheckEvent(int SeatId) : PokerHandEvent;

public sealed record CallEvent(int SeatId, long Amount, bool IsAllIn) : PokerHandEvent;

public sealed record BetEvent(int SeatId, long Amount, bool IsAllIn) : PokerHandEvent;

public sealed record RaiseToEvent(int SeatId, long RaiseAmount, long AmountTo, bool IsAllIn) : PokerHandEvent;

public sealed record UncalledBetReturnedEvent(int SeatId, long Amount) : PokerHandEvent;

public sealed record ShowCardsEvent(int SeatId, IReadOnlyList<string> Cards) : PokerHandEvent;

public sealed record HandEvaluatedEvent(int SeatId, int BoardIndex, HandCategory Category, IReadOnlyList<string> BestCards) : PokerHandEvent;

public sealed record PotAwardedEvent(int PotIndex, int BoardIndex, int SeatId, long Amount) : PokerHandEvent;

public sealed record EndHandEvent : PokerHandEvent;