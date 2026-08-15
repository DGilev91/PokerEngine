using PokerEngine.Enums;

namespace PokerEngine.Models;

public sealed record GameSetup(
    GameType GameType,
    GameLimit Limit,
    int Button,
    IReadOnlyList<long> Stacks,
    IReadOnlyList<long> Ante,
    IReadOnlyList<long> BlindsOrStraddles,
    IReadOnlyList<long> DeadBlinds);