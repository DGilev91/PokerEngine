using PokerEngine.Enums;

namespace PokerEngine.Models;

public sealed record GameSetup(
    GameType Type,
    GameLimit Limit,
    IReadOnlyList<long> Stacks,
    IReadOnlyList<long> Antes,
    IReadOnlyList<long> Blinds,
    IReadOnlyList<long> DeadBlinds,
    IReadOnlyList<long> Straddles);