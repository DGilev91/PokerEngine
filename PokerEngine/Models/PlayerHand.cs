namespace PokerEngine.Models;

public sealed record PlayerHand(int SeatIndex, int BoardIndex, HandRank Hand);