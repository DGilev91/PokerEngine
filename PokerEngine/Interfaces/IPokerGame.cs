namespace PokerEngine.Interfaces;

public interface IPokerGame
{
    int MinPlayers { get; }

    int MaxPlayers { get; }

    IHand CreateHand(IReadOnlyList<long> stacks);
}
