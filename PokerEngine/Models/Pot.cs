namespace PokerEngine.Models;

public sealed record Pot(long Amount, IReadOnlyList<int> PlayerIndices);