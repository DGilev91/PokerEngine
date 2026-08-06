using PokerEngine.Enums;

namespace PokerEngine.Models;

public abstract record GameEvent;

public sealed record GameInitializedEvent(IReadOnlyList<long> Stacks, int ButtonSeatId, Ante Ante, IReadOnlyList<BlindOrStraddle> BlindsOrStraddles) : GameEvent;

public sealed record AntePostedEvent(int SeatId, long Amount, bool IsAllIn) : GameEvent;

public sealed record BlindOrStraddlePostedEvent(int SeatId, BlindType Type, long Amount, bool IsAllIn) : GameEvent;

public sealed record HoleCardsDealtEvent(int SeatId, IReadOnlyList<string> Cards) : GameEvent;

public sealed record FoldedEvent(int SeatId) : GameEvent;

public sealed record CheckedOrCalledEvent(int SeatId, long Amount, bool IsAllIn) : GameEvent;

public sealed record BetOrRaisedToEvent(int SeatId, long Amount, bool IsAllIn) : GameEvent;

public sealed record RunoutCountSelectedEvent(int SeatId, int? Count) : GameEvent;

public sealed record CardBurnedEvent(string Card) : GameEvent;

public sealed record BoardCardsDealtEvent(RoundType Round, int BoardIndex, IReadOnlyList<string> Cards) : GameEvent;

public sealed record CardsShownOrMuckedEvent(int SeatId, IReadOnlyList<string> Cards) : GameEvent;

public sealed record UncalledBetReturnedEvent(int SeatId, long Amount) : GameEvent;

public sealed record BetsCollectedEvent(IReadOnlyList<long> Bets) : GameEvent;

public sealed record ChipsPushedEvent(int PotIndex, int BoardIndex, int SeatId, long Amount) : GameEvent;

public sealed record ChipsPulledEvent(int SeatId, long Amount) : GameEvent;