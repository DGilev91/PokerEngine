using PokerEngine.Enums;

namespace PokerEngine.Models;

public sealed record PlayerAction(int SeatIndex, IReadOnlyList<ActionType> AllowedActions,
    long CallAmount,
    long? MinBetAmount,
    long? MaxBetAmount,
    long? MinRaiseToAmount,
    long? MaxRaiseToAmount);