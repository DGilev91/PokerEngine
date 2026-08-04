using PokerEngine.Enums;

namespace PokerEngine.Hand;

public abstract record HandEvent;

public sealed record HandCreatedEvent : HandEvent;

public sealed record SeatsInitializedEvent(IReadOnlyList<long> Stacks) : HandEvent;

public sealed record AntePostedEvent(int SeatId, long Amount, bool IsAllIn) : HandEvent;

public sealed record BlindPostedEvent(int SeatId, BlindType Type, long Amount, bool IsAllIn) : HandEvent;

public sealed record HandStartedEvent : HandEvent;

public sealed record HoleCardsDealtEvent(int SeatId, IReadOnlyList<string> Cards) : HandEvent;

public sealed record BoardCardsDealtEvent(RoundType Round, int BoardIndex, IReadOnlyList<string> Cards) : HandEvent;

public sealed record PlayerActedEvent(int SeatId, PlayerActionType Type, long Amount, bool IsAllIn) : HandEvent;

public sealed record CardsShownEvent(int SeatId, IReadOnlyList<string> Cards) : HandEvent;

public sealed record HandEvaluatedEvent(int SeatId, int BoardIndex, HandCategory Category, IReadOnlyList<string> BestCards) : HandEvent;

public sealed record UncalledBetReturnedEvent(int SeatId, long Amount) : HandEvent;

public sealed record PotWonEvent(int PotIndex, int BoardIndex, int SeatId, long Amount) : HandEvent;

public sealed record HandEndedEvent : HandEvent;