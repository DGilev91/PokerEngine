using PokerEngine.Enums;

namespace PokerEngine.Models;

public abstract record GameEvent;

public sealed record GameInitializedEvent(IReadOnlyList<long> Stacks, int ButtonSeatId) : GameEvent;

public sealed record AntePostedEvent(int SeatId, long Amount, bool IsAllIn) : GameEvent;

public sealed record BlindOrStraddlePostedEvent(int SeatId, BlindType Type, long Amount, bool IsAllIn) : GameEvent;

public sealed record GameStartedEvent : GameEvent;

public sealed record HoleCardsDealtEvent(int SeatId, IReadOnlyList<string> Cards) : GameEvent;

public sealed record PlayerActionEvent(int SeatId, PlayerActionType ActionType, long Amount, bool IsAllIn) : GameEvent;

public sealed record RunoutCountSelectedEvent(int Count) : GameEvent;

public sealed record CardBurnedEvent(string Card) : GameEvent;

public sealed record BoardCardsDealtEvent(RoundType Round, IReadOnlyList<string> Cards, int BoardIndex) : GameEvent;

public sealed record CardsShownOrMuckedEvent(int SeatId, IReadOnlyList<string> Cards) : GameEvent;

public sealed record UncalledBetReturnedEvent(int SeatId, long Amount) : GameEvent;

public sealed record PotAwardedEvent(int PotIndex, int BoardIndex, int SeatId, long Amount) : GameEvent;

public sealed record GameEndedEvent : GameEvent;