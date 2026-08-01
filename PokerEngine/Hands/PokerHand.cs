using PokerEngine.Cards;
using PokerEngine.Enums;
using PokerEngine.Interfaces;
using PokerEngine.Models;

namespace PokerEngine.Hands;

internal sealed class PokerHand : IPokerHand
{
    // Dependencies

    private readonly PokerRules _rules;
    private readonly IDeck _deck;
    private readonly IHandEvaluator _handEvaluator;

    // State

    private readonly List<PokerHandEvent> _events = [];
    private readonly List<Seat> _seats = [];
    private readonly List<List<string>> _boards = [];

    private readonly PotState _potState = new();

    private HandState _state = HandState.None;
    private RoundType _round = RoundType.None;
    private int _roundIndex;
    private int? _activeSeatId;



    public PokerHand(PokerRules rules)
    {
        _rules = rules ?? throw new ArgumentNullException(nameof(rules));

        _deck = new Deck();

        _handEvaluator = _rules.GameType switch
        {
            GameType.TexasHoldem => new TexasHoldemEvaluator(),
            _ => throw new NotSupportedException($"Тип игры {_rules.GameType} пока не поддерживается.")
        };
    }


    public IReadOnlyList<PokerHandEvent> Events => _events;

    public IReadOnlyList<Seat> Seats => _seats;

    public PotState PotState => _potState;

    public IReadOnlyList<IReadOnlyList<string>> Boards => _boards;

    public HandState State => _state;

    public RoundType Round => _round;


    public void Initialize(IReadOnlyList<long> stacks)
    {
        throw new NotImplementedException();
    }

    public void PlayerPost(int seatId, PostType postType, long amount)
    {
        throw new NotImplementedException();
    }

    public void Start()
    {
        throw new NotImplementedException();
    }


    public IReadOnlyList<string> DealHole(int seatId, IReadOnlyList<string>? cards = null)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<string> DealBoard(int boardIndex = 0, IReadOnlyList<string>? cards = null)
    {
        throw new NotImplementedException();
    }

    public void PlayerAction(int seatId, ActionType actionType, long amount = 0)
    {
        throw new NotImplementedException();
    }

    public void ShowCards(int seatId, IReadOnlyList<string> cards)
    {
        throw new NotImplementedException();
    }

    public void MuckCards(int seatId)
    {
        throw new NotImplementedException();
    }
}