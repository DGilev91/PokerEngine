using PokerEngine.Enums;

namespace PokerEngine.Models;

public abstract record GameEvent;

public sealed record PlayerAdded(int SeatIndex, long Stack) : GameEvent;

public sealed record ButtonSet(int SeatIndex) : GameEvent;

public sealed record Posted(int SeatIndex, PostType Type, long Amount, bool IsAllIn) : GameEvent;

public sealed record HoleDealt(int SeatIndex, IReadOnlyList<Card> Cards) : GameEvent;

public sealed record Acted(int SeatIndex, ActionType Type, long Amount, bool IsAllIn) : GameEvent;

public sealed record UncalledBetReturned(int SeatIndex, long Amount) : GameEvent;

public sealed record BetsCollected(IReadOnlyList<long> Amounts) : GameEvent;

public sealed record BoardDealt(int BoardIndex, IReadOnlyList<Card> Cards) : GameEvent;

public sealed record HoleShown(int SeatIndex, IReadOnlyList<Card> Cards) : GameEvent;

public sealed record Mucked(int SeatIndex) : GameEvent;

public sealed record HandEvaluation(int SeatIndex, int BoardIndex, HandRank Hand) : GameEvent;

public sealed record PotAwarded(int PotIndex, int BoardIndex, int SeatIndex, long Amount) : GameEvent;