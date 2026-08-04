using PokerEngine.Enums;

namespace PokerEngine.Hand;

public abstract record HandRequest;

public sealed record HoleCardsRequest(int SeatId, int Count) : HandRequest;

public sealed record BoardCardsRequest(RoundType Round, int BoardIndex, int Count, bool BurnCard) : HandRequest;

public sealed record PlayerActionRequest(int SeatId, IReadOnlyList<PlayerActionType> AllowedActions, long CallAmount, long MinBet, long MaxBet, long MinRaiseTo, long MaxRaiseTo) : HandRequest;

public sealed record RunoutSelectionRequest(IReadOnlyList<int> AllowedCounts) : HandRequest;