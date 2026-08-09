using PokerEngine.Enums;

namespace PokerEngine.Models;

public abstract record GameOperation;

public sealed record GameInitialization(IReadOnlyList<long> stacks, IReadOnlyList<long> deadBets, IReadOnlyList<long> forceBets) : GameOperation;

public sealed record DeadBetPosting(int PlayerIndex, long Amount) : GameOperation;

public sealed record ForceBetPosting(int PlayerIndex, long Amount) : GameOperation;

public sealed record HoleDealing(int PlayerIndex, IReadOnlyList<string> Cards) : GameOperation;

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