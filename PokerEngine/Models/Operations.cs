using PokerEngine.Enums;

namespace PokerEngine.Models;

public abstract record GameOperation;

public sealed record GameInitialization(IReadOnlyList<long> stacks, IReadOnlyList<long> antesOrDeads, IReadOnlyList<long> blindsOrStraddles) : GameOperation;

public sealed record AnteOrDeadPosting(int PlayerIndex, long Amount) : GameOperation;

public sealed record BlindOrStraddlePosting(int PlayerIndex, long Amount) : GameOperation;

public sealed record HoleDealing(int PlayerIndex, IReadOnlyList<string> Cards, IReadOnlyList<bool> Statuses) : GameOperation;

public sealed record Folding(int PlayerIndex) : GameOperation;

public sealed record CheckingOrCalling(int PlayerIndex, long Amount) : GameOperation;

public sealed record BettingOrRaisingTo(int PlayerIndex, long Amount) : GameOperation;

public sealed record RunoutCountSelection(int SeatId, int? Count) : GameOperation;

public sealed record CardBurning(string Card) : GameOperation;

public sealed record BoardDealing(IReadOnlyList<string> Cards) : GameOperation;

public sealed record HoleCardsShowingOrMucking(int PlayerIndex, bool Status) : GameOperation;

public sealed record HandKilling(int PlayerIndex) : GameOperation;

public sealed record BetCollection(IReadOnlyList<long> Bets) : GameOperation;

public sealed record ChipsPushing(IReadOnlyList<long> Amounts) : GameOperation;

public sealed record ChipsPulling(int PlayerIndex, long Amount) : GameOperation;