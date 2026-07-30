namespace PokerEngine.Interfaces;

public interface IDeck
{
    IReadOnlyList<string> Cards { get; }

    int RemainingCount { get; }

    void Shuffle();

    string Deal();

    IReadOnlyList<string> Deal(int count);
}
