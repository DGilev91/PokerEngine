using PokerEngine.Enums;

namespace PokerEngine.Models;

public sealed record GameSetup(
    GameType Type,
    GameLimit Limit,
    IReadOnlyList<long> Stacks,
    IReadOnlyList<long> Ante,
    IReadOnlyList<long> BlindsOrStraddles,
    IReadOnlyList<long> DeadBlinds);