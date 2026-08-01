using PokerEngine.Cards;
using PokerEngine.Enums;
using PokerEngine.Interfaces;
using PokerEngine.Models;

namespace PokerEngine.States;

internal sealed class PokerState : IPokerState
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
    private int _roundIndex = -1;
    private int? _activeSeatId;
    private int _runoutCount = 1;

    public PokerState(PokerRules rules)
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

    public RoundType Round => _roundIndex >= 0 && _roundIndex < _rules.Rounds.Count ? _rules.Rounds[_roundIndex].Type : RoundType.None;

    public void Initialize(IReadOnlyList<long> stacks)
    {
        ArgumentNullException.ThrowIfNull(stacks);

        if (_state != HandState.None)
        {
            throw new InvalidOperationException("Раздача уже была инициализирована.");
        }

        if (stacks.Count < 2)
        {
            throw new ArgumentException("Для раздачи необходимо минимум два игрока.", nameof(stacks));
        }

        if (stacks.Any(stack => stack <= 0))
        {
            throw new ArgumentException("Стек каждого игрока должен быть больше нуля.", nameof(stacks));
        }

        if (_rules.InitialBoardCount <= 0)
        {
            throw new InvalidOperationException("Начальное количество досок должно быть больше нуля.");
        }

        if (_rules.MaxRunoutCount <= 0)
        {
            throw new InvalidOperationException("Максимальное количество runout должно быть больше нуля.");
        }

        if (_rules.Rounds.Count == 0)
        {
            throw new InvalidOperationException("В настройках отсутствуют раунды.");
        }

        _roundIndex = -1;
        _activeSeatId = null;
        _runoutCount = 1;

        for (int seatId = 0; seatId < stacks.Count; seatId++)
        {
            _seats.Add(new Seat(seatId, stacks[seatId]));
        }

        for (int boardIndex = 0; boardIndex < _rules.InitialBoardCount; boardIndex++)
        {
            _boards.Add([]);
        }

        _state = HandState.Initialized;

        _events.Add(new NewHandEvent());
        _events.Add(new SeatsEvent(stacks.ToArray()));
    }

    public void PlayerPost(int seatId, PostType postType, long amount)
    {
        Seat seat = GetSeat(seatId);
        bool isAllIn = amount >= seat.Stack;

        _events.Add(new PlayerPostedEvent(seatId, postType, amount, isAllIn));
    }

    public void Start()
    {
        if (_state != HandState.Initialized)
        {
            throw new InvalidOperationException("Раздачу можно запустить только после инициализации.");
        }

        _state = HandState.Started;
        _roundIndex = 0;

        _events.Add(new HandStartedEvent());
    }

    public void DealHole(int seatId, IReadOnlyList<string>? cards = null)
    {
        cards ??= [];

        Seat seat = GetSeat(seatId);
        string[] dealtCards = cards.ToArray();

        seat.SetHoleCards(dealtCards);

        _events.Add(new HoleCardsEvent(seatId, dealtCards));
    }

    public void SetRunoutCount(int count)
    {
        if (_state != HandState.Started)
        {
            throw new InvalidOperationException("Количество runout можно установить только после запуска раздачи.");
        }

        if (count < 1 || count > _rules.MaxRunoutCount)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Количество runout должно быть от 1 до {_rules.MaxRunoutCount}.");
        }

        if (_runoutCount != 1)
        {
            throw new InvalidOperationException("Количество runout уже было установлено.");
        }

        _runoutCount = count;

        int requiredBoardCount = _rules.InitialBoardCount * count;

        while (_boards.Count < requiredBoardCount)
        {
            int sourceBoardIndex = _boards.Count % _rules.InitialBoardCount;
            _boards.Add([.. _boards[sourceBoardIndex]]);
        }

        _events.Add(new RunoutCountEvent(count));
    }

    public void DealBoard(int boardIndex = 0, IReadOnlyList<string>? cards = null)
    {
        if (boardIndex < 0 || boardIndex >= _boards.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(boardIndex));
        }

        cards ??= [];

        List<string> board = _boards[boardIndex];
        int roundIndex = FindNextBoardRoundIndex(board.Count);
        RoundRules round = _rules.Rounds[roundIndex];

        if (cards.Count != round.BoardCardCount)
        {
            throw new ArgumentException($"Для улицы {round.Type} необходимо передать {round.BoardCardCount} карт.", nameof(cards));
        }

        string[] dealtCards = cards.ToArray();

        board.AddRange(dealtCards);

        _events.Add(new BoardEvent(round.Type, boardIndex, dealtCards));

        if (AllBoardsReachedRound(roundIndex))
        {
            _roundIndex = roundIndex;
        }
    }

    public void PlayerAction(int seatId, ActionType actionType, long amount = 0)
    {
        throw new NotImplementedException();
    }

    public void ShowCards(int seatId, IReadOnlyList<string> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        GetSeat(seatId);

        string[] shownCards = cards.ToArray();

        _events.Add(new ShowCardsEvent(seatId, shownCards));
    }

    private Seat GetSeat(int seatId)
    {
        if (seatId < 0 || seatId >= _seats.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(seatId), $"SeatId должен быть от 0 до {_seats.Count - 1}.");
        }

        return _seats[seatId];
    }

    private int FindNextBoardRoundIndex(int boardCardCount)
    {
        for (int roundIndex = 0; roundIndex < _rules.Rounds.Count; roundIndex++)
        {
            RoundRules round = _rules.Rounds[roundIndex];

            if (round.BoardCardCount <= 0)
            {
                continue;
            }

            if (boardCardCount == GetBoardCardCountBefore(roundIndex))
            {
                return roundIndex;
            }
        }

        throw new InvalidOperationException($"Нельзя определить следующую улицу. На доске уже {boardCardCount} карт.");
    }

    private bool AllBoardsReachedRound(int roundIndex)
    {
        int requiredCardCount = GetBoardCardCountAfter(roundIndex);

        return _boards.All(board => board.Count >= requiredCardCount);
    }

    private int GetBoardCardCountBefore(int roundIndex)
    {
        int cardCount = 0;

        for (int index = 0; index < roundIndex; index++)
        {
            cardCount += _rules.Rounds[index].BoardCardCount;
        }

        return cardCount;
    }

    private int GetBoardCardCountAfter(int roundIndex)
    {
        int cardCount = 0;

        for (int index = 0; index <= roundIndex; index++)
        {
            cardCount += _rules.Rounds[index].BoardCardCount;
        }

        return cardCount;
    }
}