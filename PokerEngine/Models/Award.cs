namespace PokerEngine.Models;

public sealed record Award(int PotIndex, int BoardIndex, int PlayerIndex, long Amount);