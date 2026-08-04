using PokerEngine.Rules;

namespace PokerEngine.Hand.Context;

public sealed class PokerHandContext
{
    private List<Player> _players;
    private List<Board> _boards;
    private List<Pot> _pots;

    public PokerRules Rules { get; }

    public IReadOnlyList<Player> Players => _players;

    public IReadOnlyList<Board> Boards => _boards;

    public UncalledBet? UncalledBet { get; private set; }

    public IReadOnlyList<Pot> Pots => _pots;

    public int? ActingSeatId { get; internal set; }

    public PokerHandContext(PokerRules rules)
    {
        ArgumentNullException.ThrowIfNull(rules);

        Rules = rules;
    }

    public void Initialize(IReadOnlyList<long> stacks)
    {
        _players = stacks.Select((stack, seatId) => new Player(seatId, stack)).ToList();

        _boards = Enumerable
            .Range(0, Rules.InitialBoardCount)
            .Select(e => new Board(e))
            .ToList();

        _pots = [];
    }
}