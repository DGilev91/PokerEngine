namespace PokerEngine.Models;

public sealed class GameSettings
{
    public int BoardCount { get; init; } = 1;

    public int MaxRunoutCount { get; init; } = 1;
}
