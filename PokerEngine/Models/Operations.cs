using PokerEngine.Enums;

namespace PokerEngine.Models;

public abstract record GameOperation;

public sealed record Initializing(GameSetup Setup) : GameOperation;

public sealed record HoleDealing(int PlayerIndex, IReadOnlyList<Card> Cards) : GameOperation;

public sealed record Folding(int PlayerIndex) : GameOperation;

public sealed record Checking(int PlayerIndex) : GameOperation;

public sealed record Calling(int PlayerIndex, long Amount, bool IsAllIn) : GameOperation;

public sealed record Betting(int PlayerIndex, long Amount, bool IsAllIn) : GameOperation;

public sealed record Raising(int PlayerIndex, long Amount, long ToAmount, bool IsAllIn) : GameOperation;

public sealed record RunoutCountSelection(int SeatId, int? Count) : GameOperation;

public sealed record BoardDealing(IReadOnlyList<Card> Cards) : GameOperation;

public sealed record ShowingOrMucking(int PlayerIndex, IReadOnlyList<Card> Cards) : GameOperation;

public sealed record BetCollection(IReadOnlyList<long> Bets) : GameOperation;

public sealed record ChipsPushing(IReadOnlyList<long> Amounts) : GameOperation;

public sealed record ChipsPulling(int PlayerIndex, long Amount) : GameOperation;