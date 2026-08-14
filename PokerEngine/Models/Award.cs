namespace PokerEngine.Models;

public sealed record Award(int PotIndex, int BoardIndex, int SeatIndex, long Amount);