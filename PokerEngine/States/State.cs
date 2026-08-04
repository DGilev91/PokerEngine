namespace PokerKit.States;

/// <summary>
/// Minimal forward declaration used by the utilities port.
/// The full PokerKit state implementation will replace/extend this class.
/// </summary>
public partial class State
{
    private readonly List<IReadOnlyList<Utilities.Card>> _boardCards = [];

    public IReadOnlyList<IReadOnlyList<Utilities.Card>> BoardCards => _boardCards;

    internal List<IReadOnlyList<Utilities.Card>> MutableBoardCards => _boardCards;
}
