namespace PokerEngine.Models;

public sealed record GameSetup(
    IReadOnlyList<long> Stacks,
    IReadOnlyList<long> Antes,
    IReadOnlyList<long> Blinds,
    IReadOnlyList<long> Straddles,
    IReadOnlyList<long> DeadBlinds);