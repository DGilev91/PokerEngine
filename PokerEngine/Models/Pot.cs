namespace PokerEngine.Models;

public sealed record Pot(int Index, long Amount, IReadOnlyList<int> EligibleSeatIds);