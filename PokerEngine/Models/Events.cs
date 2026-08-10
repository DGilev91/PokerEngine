using PokerEngine.Enums;

namespace PokerEngine.Models;

public abstract record GameEvent;

public sealed record SeatInitialized(int PlayerIndex, long Stack) : GameEvent;

public sealed record AntePosted(int PlayerIndex, long Amount, bool IsAllIn) : GameEvent;

public sealed record BlindPosted(int PlayerIndex, long Amount, bool IsAllIn) : GameEvent;

public sealed record StraddlePosted(int PlayerIndex, long Amount, bool IsAllIn) : GameEvent;

public sealed record DeadBlindPosted(int PlayerIndex, long Amount, bool IsAllIn) : GameEvent;

public sealed record HoleDealt(int PlayerIndex, IReadOnlyList<Card> Cards) : GameEvent;

public sealed record Folded(int PlayerIndex) : GameEvent;

public sealed record Checked(int PlayerIndex) : GameEvent;

public sealed record Called(int PlayerIndex, long Amount, bool IsAllIn) : GameEvent;

public sealed record Bet(int PlayerIndex, long Amount, bool IsAllIn) : GameEvent;

public sealed record Raised(int PlayerIndex, long Amount, long ToAmount, bool IsAllIn) : GameEvent;

public sealed record RunoutCountSelected(int PlayerIndex, int? Count) : GameEvent;

public sealed record BoardDealt(IReadOnlyList<Card> Cards) : GameEvent;

public sealed record HoleShown(int PlayerIndex, IReadOnlyList<Card> Cards) : GameEvent;

public sealed record HoleMucked(int PlayerIndex) : GameEvent;