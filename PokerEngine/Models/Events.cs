using PokerEngine.Enums;

namespace PokerEngine.Models;

public abstract record GameEvent;

public sealed record PlayerAdded(int PlayerIndex, long Stack) : GameEvent;

public sealed record ButtonSet(int PlayerIndex) : GameEvent;

public sealed record Posted(int PlayerIndex, PostType Type, long Amount, bool IsAllIn) : GameEvent;

public sealed record GameStarted : GameEvent;

public sealed record HoleDealt(int PlayerIndex, IReadOnlyList<Card> Cards) : GameEvent;

public sealed record Acted(int PlayerIndex, ActionType Type, long Amount, bool IsAllIn) : GameEvent;

public sealed record UncalledBetReturned(int PlayerIndex, long Amount) : GameEvent;

public sealed record BetsCollected(IReadOnlyList<long> Amounts) : GameEvent;

public sealed record BoardDealt(int BoardIndex, IReadOnlyList<Card> Cards) : GameEvent;

public sealed record HoleShown(int PlayerIndex, IReadOnlyList<Card> Cards) : GameEvent;

public sealed record Mucked(int PlayerIndex) : GameEvent;

public sealed record PotAwarded(int PotIndex, int BoardIndex, int PlayerIndex, long Amount) : GameEvent;