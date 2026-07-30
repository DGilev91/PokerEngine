using PokerEngine.Enums;

namespace PokerEngine.Models;

public sealed class HandConfig
{
    public required BettingLimit BettingLimit { get; init; }

    public required GameType GameType { get; init; }

    public required int DealerSeat { get; init; }

    public required IReadOnlyDictionary<int, long> Seats { get; init; }

    public Automation Automation { get; init; } = Automation.All;
}