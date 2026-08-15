using PokerEngine.Enums;

namespace PokerEngine.Models;

public sealed record HandRank(HandCategory Category, IReadOnlyList<Card> Cards);