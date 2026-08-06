using PokerEngine.Enums;

namespace PokerEngine.Models;

public abstract record GameOperation;

public sealed record GameInitialization(IReadOnlyList<long> Stacks, Ante Ante, IReadOnlyList<BlindOrStraddle> BlindsOrStraddles) : GameOperation;

public sealed record AntePosting(int SeatId, long Amount, bool IsAllIn) : GameOperation;

public sealed record BlindOrStraddlePosting(int SeatId, BlindType Type, long Amount, bool IsAllIn) : GameOperation;

public sealed record HoleDealing(int SeatId, IReadOnlyList<string> Cards) : GameOperation;

public sealed record Folding(int SeatId) : GameOperation;

public sealed record CheckingOrCalling(int SeatId, long Amount, bool IsAllIn) : GameOperation;

public sealed record BettingOrRaisingTo(int SeatId, long Amount, bool IsAllIn) : GameOperation;

public sealed record RunoutCountSelection(int SeatId, int? Count) : GameOperation;

public sealed record CardBurning(string Card) : GameOperation;

public sealed record BoardDealing(RoundType Round, int BoardIndex, IReadOnlyList<string> Cards) : GameOperation;

public sealed record HoleCardsShowingOrMucking(int SeatId, IReadOnlyList<string> Cards) : GameOperation;

public sealed record HandKilling(int SeatId) : GameOperation;

public sealed record BetCollection(IReadOnlyList<long> CollectedBets) : GameOperation;

public sealed record ChipsPushing(int PotIndex, int BoardIndex, int SeatId, long Amount) : GameOperation;

public sealed record ChipsPulling(int SeatId, long Amount) : GameOperation;