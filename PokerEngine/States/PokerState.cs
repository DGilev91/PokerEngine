using PokerEngine.Cards;
using PokerEngine.Enums;
using PokerEngine.Interfaces;
using PokerEngine.Models;

namespace PokerEngine.States;

public sealed class PokerState : IPokerState
{
    // Dependencies

    private readonly PokerRules _rules;
    private readonly IDeck _deck;
    private readonly IHandEvaluator _handEvaluator;

    // State

    private readonly List<PokerHandEvent> _events = [];
    private readonly List<Seat> _seats = [];
    private readonly List<List<string>> _boards = [];
    private readonly List<PotSlice> _potSlices = [];

    private readonly HashSet<int> _actedSeats = [];
    private readonly HashSet<int> _shownSeats = [];

    private readonly PotState _potState = new();

    private HandState _state = HandState.None;
    private int _roundIndex = -1;
    private int? _activeSeatId;
    private int? _lastAggressorSeatId;

    private int? _uncalledSeatId;
    private long _uncalledAmount;

    private int _runoutCount = 1;
    private bool _runoutCountWasSet;
    private bool _waitingForRunoutDecision;

    private long _lastFullRaiseSize;
    private int _raiseCount;

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

    public RoundType Round => _roundIndex == _rules.Rounds.Count ? RoundType.Showdown : _roundIndex >= 0 && _roundIndex < _rules.Rounds.Count ? _rules.Rounds[_roundIndex].Type : RoundType.None;

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

        ValidateRules();

        _events.Clear();
        _seats.Clear();
        _boards.Clear();
        _potSlices.Clear();
        _actedSeats.Clear();
        _shownSeats.Clear();
        _potState.Clear();

        _roundIndex = -1;
        _activeSeatId = null;
        _lastAggressorSeatId = null;

        _uncalledSeatId = null;
        _uncalledAmount = 0;

        _runoutCount = 1;
        _runoutCountWasSet = false;
        _waitingForRunoutDecision = false;

        _lastFullRaiseSize = 0;
        _raiseCount = 0;

        for (int seatId = 0; seatId < stacks.Count; seatId++)
        {
            _seats.Add(new Seat(seatId, stacks[seatId]));
        }

        for (int boardIndex = 0; boardIndex < _rules.InitialBoardCount; boardIndex++)
        {
            _boards.Add([]);
        }

        if (IsAutomated(Automation.ShuffleDeck))
        {
            _deck.Shuffle();
        }

        _state = HandState.Initialized;

        Emit(new NewHandEvent());
        Emit(new SeatsEvent(stacks.ToArray()));
    }

    public void PlayerPost(int seatId, PostType postType, long amount)
    {
        EnsureState(HandState.Initialized);

        Seat seat = GetSeat(seatId);

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Размер поста должен быть больше нуля.");
        }

        long paid = Math.Min(amount, seat.Stack);

        seat.Stack -= paid;
        seat.TotalBet += paid;

        if (IsLivePost(postType))
        {
            seat.RoundBet += paid;
        }

        Emit(new PlayerPostedEvent(seatId, postType, paid, seat.IsAllIn));

        RecalculatePots();
    }

    public void Start()
    {
        EnsureState(HandState.Initialized);

        _state = HandState.Started;
        _roundIndex = 0;

        ResetRoundState();

        Emit(new HandStartedEvent());

        if (IsAutomated(Automation.DealHoleCards))
        {
            foreach (Seat seat in _seats)
            {
                DealHole(seat.SeatId);
            }
        }

        RoundRules round = GetCurrentRound();

        if (round.BoardCardCount > 0)
        {
            if (IsAutomated(Automation.DealBoard))
            {
                DealCurrentRoundBoardsAutomatically();
            }

            return;
        }

        BeginBettingRound();
    }

    public void DealHole(int seatId, IReadOnlyList<string>? cards = null)
    {
        EnsureStarted();

        Seat seat = GetSeat(seatId);

        if (seat.HoleCards.Count > 0)
        {
            throw new InvalidOperationException($"Игроку на месте {seatId} уже выданы карты.");
        }

        int cardCount = GetHoleCardCount();
        string[] dealtCards;

        if (cards is null)
        {
            if (!IsAutomated(Automation.DealHoleCards))
            {
                throw new InvalidOperationException("В ручном режиме необходимо передать карманные карты.");
            }

            dealtCards = _deck.Deal(cardCount).ToArray();
        }
        else
        {
            ValidateCards(cards, cardCount);

            dealtCards = cards.ToArray();
            _deck.Take(dealtCards);
        }

        seat.SetHoleCards(dealtCards);

        Emit(new HoleCardsEvent(seatId, dealtCards));
    }

    public void SetRunoutCount(int count)
    {
        EnsureStarted();

        if (count < 1 || count > _rules.MaxRunoutCount)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Количество runout должно быть от 1 до {_rules.MaxRunoutCount}.");
        }

        if (_runoutCountWasSet)
        {
            throw new InvalidOperationException("Количество runout уже было установлено.");
        }

        if (!_waitingForRunoutDecision && count > 1)
        {
            throw new InvalidOperationException("Сейчас нет ситуации, в которой можно выбрать несколько runout.");
        }

        _runoutCount = count;
        _runoutCountWasSet = true;
        _waitingForRunoutDecision = false;

        int requiredBoardCount = _rules.InitialBoardCount * count;

        while (_boards.Count < requiredBoardCount)
        {
            int sourceBoardIndex = _boards.Count % _rules.InitialBoardCount;
            _boards.Add([.. _boards[sourceBoardIndex]]);
        }

        Emit(new RunoutCountEvent(count));

        ContinueAfterRunoutDecision();
    }

    public void DealBoard(int boardIndex = 0, IReadOnlyList<string>? cards = null)
    {
        EnsureStarted();
        ValidateBoardIndex(boardIndex);

        List<string> board = _boards[boardIndex];
        int roundIndex = FindNextBoardRoundIndex(board.Count);
        RoundRules round = _rules.Rounds[roundIndex];

        string[] dealtCards;

        if (cards is null)
        {
            if (!IsAutomated(Automation.DealBoard))
            {
                throw new InvalidOperationException("В ручном режиме необходимо передать карты доски.");
            }

            if (IsAutomated(Automation.BurnCards))
            {
                _deck.Deal();
            }

            dealtCards = _deck.Deal(round.BoardCardCount).ToArray();
        }
        else
        {
            ValidateCards(cards, round.BoardCardCount);

            dealtCards = cards.ToArray();
            _deck.Take(dealtCards);
        }

        board.AddRange(dealtCards);

        Emit(new BoardEvent(round.Type, boardIndex, dealtCards));

        if (!AllBoardsReachedRound(roundIndex))
        {
            return;
        }

        _roundIndex = roundIndex;

        if (IsFinalRound())
        {
            if (AllRemainingPlayersAllIn())
            {
                StartShowdown();
            }
            else
            {
                BeginBettingRound();
            }

            return;
        }

        if (AllRemainingPlayersAllIn())
        {
            ContinueAllInRunout();
            return;
        }

        BeginBettingRound();
    }

    public void PlayerAction(int seatId, ActionType actionType, long amount = 0)
    {
        EnsureStarted();

        if (_waitingForRunoutDecision)
        {
            throw new InvalidOperationException("Сначала необходимо выбрать количество runout.");
        }

        if (_activeSeatId != seatId)
        {
            throw new InvalidOperationException($"Сейчас ход игрока на месте {_activeSeatId}.");
        }

        Seat seat = GetActionSeat(seatId);

        switch (actionType)
        {
            case ActionType.Fold:
                Fold(seat);
                break;

            case ActionType.Check:
                Check(seat);
                break;

            case ActionType.Call:
                Call(seat);
                break;

            case ActionType.Bet:
                Bet(seat, amount);
                break;

            case ActionType.RaiseTo:
                RaiseTo(seat, amount);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(actionType), actionType, "Неизвестное действие.");
        }

        ContinueAfterAction(seatId);
    }

    public void ShowCards(int seatId, IReadOnlyList<string> cards)
    {
        EnsureStarted();
        ArgumentNullException.ThrowIfNull(cards);

        Seat seat = GetSeat(seatId);

        if (cards.Count > 0)
        {
            string[] shownCards = cards.ToArray();

            if (seat.HoleCards.Count == 0)
            {
                seat.SetHoleCards(shownCards);
            }
            else if (!seat.HoleCards.SequenceEqual(shownCards))
            {
                throw new InvalidOperationException($"Показанные карты игрока {seatId} не совпадают с выданными картами.");
            }

            string[] unknownCards = shownCards.Where(card => !DeckWasAlreadyUsed(card)).ToArray();

            if (unknownCards.Length > 0)
            {
                _deck.Take(unknownCards);
            }
        }

        _shownSeats.Add(seatId);

        Emit(new ShowCardsEvent(seatId, cards.ToArray()));

        if (Round == RoundType.Showdown)
        {
            TryCompleteShowdown();
        }
    }

    private void Fold(Seat seat)
    {
        if (seat.IsFolded)
        {
            throw new InvalidOperationException($"Игрок на месте {seat.SeatId} уже сделал fold.");
        }

        seat.IsFolded = true;
        _actedSeats.Add(seat.SeatId);

        if (_uncalledSeatId == seat.SeatId)
        {
            ClearUncalledCandidate();
        }
        else
        {
            RefreshUncalledCandidate();
        }

        Emit(new PlayerActionEvent(seat.SeatId, ActionType.Fold, 0, seat.RoundBet, false));
    }

    private void Check(Seat seat)
    {
        long highestBet = GetHighestRoundBet();

        if (seat.RoundBet != highestBet)
        {
            throw new InvalidOperationException($"Игрок на месте {seat.SeatId} не может сделать check.");
        }

        _actedSeats.Add(seat.SeatId);

        Emit(new PlayerActionEvent(seat.SeatId, ActionType.Check, 0, seat.RoundBet, false));
    }

    private void Call(Seat seat)
    {
        long highestBet = GetHighestRoundBet();
        long amountToCall = highestBet - seat.RoundBet;

        if (amountToCall <= 0)
        {
            throw new InvalidOperationException($"Игроку на месте {seat.SeatId} нечего коллировать.");
        }

        long paid = CommitTo(seat, highestBet);

        _actedSeats.Add(seat.SeatId);

        RefreshUncalledCandidate();

        Emit(new PlayerActionEvent(seat.SeatId, ActionType.Call, paid, seat.RoundBet, seat.IsAllIn));

        RecalculatePots();
    }

    private void Bet(Seat seat, long amount)
    {
        if (GetHighestRoundBet() > 0)
        {
            throw new InvalidOperationException("Нельзя сделать bet, когда ставка уже существует. Используйте RaiseTo.");
        }

        RoundRules round = GetCurrentRound();
        long maximum = GetMaximumBetTo(seat);

        if (amount <= 0 || amount > maximum)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), $"Размер bet должен быть от 1 до {maximum}.");
        }

        if (amount < round.BetSize && amount < seat.RoundBet + seat.Stack)
        {
            throw new InvalidOperationException($"Минимальный bet равен {round.BetSize}.");
        }

        long previousBet = seat.RoundBet;
        long paid = CommitTo(seat, amount);
        long betSize = seat.RoundBet - previousBet;

        _lastFullRaiseSize = Math.Max(betSize, round.BetSize);
        _lastAggressorSeatId = seat.SeatId;

        ResetActedAfterAggression(seat.SeatId);
        SetUncalledCandidate(seat.SeatId);

        Emit(new PlayerActionEvent(seat.SeatId, ActionType.Bet, paid, seat.RoundBet, seat.IsAllIn));

        RecalculatePots();
    }

    private void RaiseTo(Seat seat, long amount)
    {
        long highestBet = GetHighestRoundBet();

        if (highestBet <= 0)
        {
            throw new InvalidOperationException("Нельзя сделать raise, когда ставки ещё нет. Используйте Bet.");
        }

        long maximumRaiseTo = GetMaximumRaiseTo(seat);

        if (amount <= highestBet || amount > maximumRaiseTo)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), $"RaiseTo должен быть больше {highestBet} и не больше {maximumRaiseTo}.");
        }

        long minimumRaiseTo = highestBet + _lastFullRaiseSize;
        long availableTotal = seat.RoundBet + seat.Stack;
        bool isAllInRaise = amount >= availableTotal;

        if (amount < minimumRaiseTo && !isAllInRaise)
        {
            throw new InvalidOperationException($"Минимальный RaiseTo равен {minimumRaiseTo}.");
        }

        RoundRules round = GetCurrentRound();

        if (round.MaxRaises.HasValue && _raiseCount >= round.MaxRaises.Value)
        {
            throw new InvalidOperationException("Достигнуто максимальное количество повышений.");
        }

        long paid = CommitTo(seat, amount);
        long raiseSize = seat.RoundBet - highestBet;
        bool isFullRaise = raiseSize >= _lastFullRaiseSize;

        if (isFullRaise)
        {
            _lastFullRaiseSize = raiseSize;
            _lastAggressorSeatId = seat.SeatId;
            _raiseCount++;

            ResetActedAfterAggression(seat.SeatId);
        }
        else
        {
            _actedSeats.Add(seat.SeatId);
        }

        SetUncalledCandidate(seat.SeatId);

        Emit(new PlayerActionEvent(seat.SeatId, ActionType.RaiseTo, paid, seat.RoundBet, seat.IsAllIn));

        RecalculatePots();
    }

    private void ContinueAfterAction(int actingSeatId)
    {
        _activeSeatId = null;

        if (TryCompleteByFold())
        {
            return;
        }

        if (!IsBettingRoundCompleted())
        {
            SetNextTurn(actingSeatId);
            return;
        }

        ReturnUncalledBet();
        RecalculatePots();

        if (TryCompleteByFold())
        {
            return;
        }

        ResetRoundBets();

        if (IsFinalRound())
        {
            StartShowdown();
            return;
        }

        if (AllRemainingPlayersAllIn())
        {
            ContinueAllInRunout();
            return;
        }

        MoveToNextRound();
    }

    private void ContinueAllInRunout()
    {
        _activeSeatId = null;

        if (HasUndealtBoardCards() && _rules.MaxRunoutCount > 1 && !_runoutCountWasSet)
        {
            _waitingForRunoutDecision = true;
            return;
        }

        ContinueAfterRunoutDecision();
    }

    private void ContinueAfterRunoutDecision()
    {
        if (!HasUndealtBoardCards())
        {
            StartShowdown();
            return;
        }

        if (IsAutomated(Automation.DealBoard))
        {
            DealRemainingBoardsAutomatically();
        }
    }

    private void MoveToNextRound()
    {
        int nextRoundIndex = _roundIndex + 1;

        if (nextRoundIndex >= _rules.Rounds.Count)
        {
            StartShowdown();
            return;
        }

        _roundIndex = nextRoundIndex;
        ResetRoundState();

        RoundRules round = GetCurrentRound();

        if (round.BoardCardCount > 0)
        {
            if (IsAutomated(Automation.DealBoard))
            {
                DealCurrentRoundBoardsAutomatically();
            }

            return;
        }

        BeginBettingRound();
    }

    private void BeginBettingRound()
    {
        ResetRoundState();

        if (CountActionableSeats() <= 1)
        {
            ContinueAllInRunout();
            return;
        }

        SetTurn(GetFirstSeatToAct());
    }

    private void SetNextTurn(int previousSeatId)
    {
        SetTurn(FindNextActionableSeat(previousSeatId));
    }

    private void SetTurn(int seatId)
    {
        Seat seat = GetActionSeat(seatId);

        long highestBet = GetHighestRoundBet();
        long callAmount = Math.Min(Math.Max(0, highestBet - seat.RoundBet), seat.Stack);
        long minBet = 0;
        long maxBet = 0;
        long minRaiseTo = 0;
        long maxRaiseTo = 0;

        List<ActionType> actions = [];

        if (callAmount > 0)
        {
            actions.Add(ActionType.Fold);
            actions.Add(ActionType.Call);

            if (CanRaise(seat))
            {
                actions.Add(ActionType.RaiseTo);
                minRaiseTo = GetMinimumRaiseTo();
                maxRaiseTo = GetMaximumRaiseTo(seat);
            }
        }
        else if (highestBet == 0)
        {
            actions.Add(ActionType.Check);

            if (seat.Stack > 0)
            {
                actions.Add(ActionType.Bet);
                minBet = Math.Min(GetCurrentRound().BetSize, seat.Stack);
                maxBet = GetMaximumBetTo(seat);
            }
        }
        else
        {
            actions.Add(ActionType.Check);

            if (CanRaise(seat))
            {
                actions.Add(ActionType.RaiseTo);
                minRaiseTo = GetMinimumRaiseTo();
                maxRaiseTo = GetMaximumRaiseTo(seat);
            }
        }

        _activeSeatId = seatId;

        Emit(new PlayerTurnEvent(seatId, actions, callAmount, minBet, maxBet, minRaiseTo, maxRaiseTo));
    }

    private bool IsBettingRoundCompleted()
    {
        long highestBet = GetHighestRoundBet();

        Seat[] actionableSeats = _seats.Where(seat => !seat.IsFolded && !seat.IsAllIn).ToArray();

        if (actionableSeats.Length == 0)
        {
            return true;
        }

        return actionableSeats.All(seat => _actedSeats.Contains(seat.SeatId) && seat.RoundBet == highestBet);
    }

    private bool TryCompleteByFold()
    {
        Seat[] remaining = _seats.Where(seat => !seat.IsFolded).ToArray();

        if (remaining.Length != 1)
        {
            return false;
        }

        ReturnUncalledBet();
        RecalculatePots();

        Seat winner = remaining[0];

        foreach (PotSlice pot in _potSlices)
        {
            winner.Stack += pot.Amount;

            Emit(new PotAwardedEvent(pot.Index, 0, winner.SeatId, pot.Amount));
        }

        CompleteHand();

        return true;
    }

    private void StartShowdown()
    {
        _activeSeatId = null;
        _roundIndex = _rules.Rounds.Count;

        TryCompleteShowdown();
    }

    private void TryCompleteShowdown()
    {
        if (_state == HandState.Completed)
        {
            return;
        }

        Seat[] contenders = _seats.Where(seat => !seat.IsFolded).ToArray();

        if (contenders.Any(seat => seat.HoleCards.Count == 0))
        {
            return;
        }

        RecalculatePots();
        EvaluateAndAwardPots();
        CompleteHand();
    }

    private void EvaluateAndAwardPots()
    {
        Dictionary<(int seatId, int boardIndex), EvaluatedHand> evaluations = [];

        foreach (Seat seat in _seats.Where(seat => !seat.IsFolded))
        {
            for (int boardIndex = 0; boardIndex < _boards.Count; boardIndex++)
            {
                EvaluatedHand evaluation = EvaluateHand(seat, boardIndex);

                evaluations[(seat.SeatId, boardIndex)] = evaluation;

                Emit(new HandEvaluatedEvent(seat.SeatId, boardIndex, evaluation.Category, evaluation.BestCards));
            }
        }

        foreach (PotSlice pot in _potSlices)
        {
            long boardShare = pot.Amount / _boards.Count;
            long boardRemainder = pot.Amount % _boards.Count;

            for (int boardIndex = 0; boardIndex < _boards.Count; boardIndex++)
            {
                long amountForBoard = boardShare + (boardIndex < boardRemainder ? 1 : 0);

                if (amountForBoard <= 0)
                {
                    continue;
                }

                int[] eligibleSeats = pot.EligibleSeatIds.Where(seatId => !_seats[seatId].IsFolded).ToArray();

                if (eligibleSeats.Length == 0)
                {
                    continue;
                }

                long bestStrength = eligibleSeats.Max(seatId => evaluations[(seatId, boardIndex)].Strength);
                int[] winners = eligibleSeats.Where(seatId => evaluations[(seatId, boardIndex)].Strength == bestStrength).Order().ToArray();

                long winnerShare = amountForBoard / winners.Length;
                long winnerRemainder = amountForBoard % winners.Length;

                for (int index = 0; index < winners.Length; index++)
                {
                    int winnerSeatId = winners[index];
                    long awarded = winnerShare + (index < winnerRemainder ? 1 : 0);

                    _seats[winnerSeatId].Stack += awarded;

                    Emit(new PotAwardedEvent(pot.Index, boardIndex, winnerSeatId, awarded));
                }
            }
        }
    }

    private EvaluatedHand EvaluateHand(Seat seat, int boardIndex)
    {
        HandRank result = _handEvaluator.Evaluate(seat.HoleCards, _boards[boardIndex]);

        return new EvaluatedHand(result.Strength, result.Category, result.Cards.ToArray());
    }

    private void CompleteHand()
    {
        _activeSeatId = null;
        _waitingForRunoutDecision = false;
        _state = HandState.Completed;

        Emit(new EndHandEvent());
    }

    private void SetUncalledCandidate(int seatId)
    {
        _uncalledSeatId = seatId;

        RefreshUncalledCandidate();
    }

    private void RefreshUncalledCandidate()
    {
        if (!_uncalledSeatId.HasValue)
        {
            _uncalledAmount = 0;
            return;
        }

        Seat candidate = GetSeat(_uncalledSeatId.Value);

        if (candidate.IsFolded)
        {
            ClearUncalledCandidate();
            return;
        }

        long highestOtherLiveBet = _seats.Where(seat => seat.SeatId != candidate.SeatId).Select(seat => seat.RoundBet).DefaultIfEmpty(0).Max();

        _uncalledAmount = Math.Max(0, candidate.RoundBet - highestOtherLiveBet);

        if (_uncalledAmount == 0)
        {
            ClearUncalledCandidate();
        }
    }

    private void ClearUncalledCandidate()
    {
        _uncalledSeatId = null;
        _uncalledAmount = 0;
    }

    private void ReturnUncalledBet()
    {
        RefreshUncalledCandidate();

        if (!_uncalledSeatId.HasValue || _uncalledAmount <= 0)
        {
            return;
        }

        Seat seat = GetSeat(_uncalledSeatId.Value);
        long amount = _uncalledAmount;

        seat.RoundBet -= amount;
        seat.TotalBet -= amount;
        seat.Stack += amount;

        Emit(new UncalledBetReturnedEvent(seat.SeatId, amount));

        ClearUncalledCandidate();
    }

    private void RecalculatePots()
    {
        _potSlices.Clear();
        _potState.Clear();

        long[] levels = _seats.Select(seat => seat.TotalBet).Where(amount => amount > 0).Distinct().Order().ToArray();
        long previousLevel = 0;

        List<PotBuilder> builders = [];

        foreach (long level in levels)
        {
            Seat[] contributors = _seats.Where(seat => seat.TotalBet >= level).ToArray();
            long contributionPerSeat = level - previousLevel;

            if (contributionPerSeat <= 0 || contributors.Length == 0)
            {
                previousLevel = level;
                continue;
            }

            long amount = contributionPerSeat * contributors.Length;
            int[] eligibleSeatIds = contributors.Where(seat => !seat.IsFolded).Select(seat => seat.SeatId).Order().ToArray();

            Dictionary<int, long> contributions = contributors.ToDictionary(seat => seat.SeatId, _ => contributionPerSeat);

            PotBuilder? previousBuilder = builders.LastOrDefault();

            if (previousBuilder is not null && previousBuilder.EligibleSeatIds.SequenceEqual(eligibleSeatIds))
            {
                previousBuilder.Amount += amount;

                foreach ((int seatId, long contribution) in contributions)
                {
                    previousBuilder.Contributions[seatId] = previousBuilder.Contributions.GetValueOrDefault(seatId) + contribution;
                }
            }
            else
            {
                builders.Add(new PotBuilder
                {
                    Amount = amount,
                    EligibleSeatIds = eligibleSeatIds,
                    Contributions = contributions
                });
            }

            previousLevel = level;
        }

        for (int potIndex = 0; potIndex < builders.Count; potIndex++)
        {
            PotBuilder builder = builders[potIndex];

            _potSlices.Add(new PotSlice(potIndex, builder.Amount, builder.EligibleSeatIds));

            Pot pot = new Pot(potIndex);

            foreach ((int seatId, long contribution) in builder.Contributions)
            {
                pot.AddContribution(seatId, contribution);
            }

            foreach (Seat seat in _seats.Where(seat => seat.IsFolded))
            {
                pot.RemoveEligibility(seat.SeatId);
            }

            _potState.AddPot(pot);
        }
    }

    private long CommitTo(Seat seat, long amountTo)
    {
        if (amountTo < seat.RoundBet)
        {
            throw new InvalidOperationException("Новая ставка не может быть меньше текущей ставки игрока.");
        }

        long requested = amountTo - seat.RoundBet;
        long paid = Math.Min(requested, seat.Stack);

        seat.Stack -= paid;
        seat.RoundBet += paid;
        seat.TotalBet += paid;

        return paid;
    }

    private void ResetRoundBets()
    {
        foreach (Seat seat in _seats)
        {
            seat.RoundBet = 0;
        }
    }

    private void ResetRoundState()
    {
        _actedSeats.Clear();

        _activeSeatId = null;
        _lastAggressorSeatId = null;

        _uncalledSeatId = null;
        _uncalledAmount = 0;

        _raiseCount = 0;

        RoundRules round = GetCurrentRound();

        _lastFullRaiseSize = round.BetSize > 0 ? round.BetSize : _rules.BigBlind;
    }

    private void ResetActedAfterAggression(int aggressorSeatId)
    {
        _actedSeats.Clear();
        _actedSeats.Add(aggressorSeatId);
    }

    private long GetHighestRoundBet()
    {
        return _seats.Count == 0 ? 0 : _seats.Max(seat => seat.RoundBet);
    }

    private long GetMinimumRaiseTo()
    {
        return GetHighestRoundBet() + _lastFullRaiseSize;
    }

    private long GetMaximumBetTo(Seat seat)
    {
        long availableTotal = seat.RoundBet + seat.Stack;

        return _rules.BettingType switch
        {
            BettingType.FixedLimit => Math.Min(GetCurrentRound().BetSize, availableTotal),
            BettingType.PotLimit => Math.Min(GetCurrentPotAmount(), availableTotal),
            BettingType.NoLimit => availableTotal,
            _ => throw new NotSupportedException($"Структура ставок {_rules.BettingType} не поддерживается.")
        };
    }

    private long GetMaximumRaiseTo(Seat seat)
    {
        long highestBet = GetHighestRoundBet();
        long amountToCall = Math.Max(0, highestBet - seat.RoundBet);
        long availableTotal = seat.RoundBet + seat.Stack;

        return _rules.BettingType switch
        {
            BettingType.FixedLimit => Math.Min(highestBet + GetCurrentRound().BetSize, availableTotal),
            BettingType.PotLimit => Math.Min(highestBet + GetCurrentPotAmount() + amountToCall, availableTotal),
            BettingType.NoLimit => availableTotal,
            _ => throw new NotSupportedException($"Структура ставок {_rules.BettingType} не поддерживается.")
        };
    }

    private long GetCurrentPotAmount()
    {
        return _seats.Sum(seat => seat.TotalBet);
    }

    private bool CanRaise(Seat seat)
    {
        if (seat.Stack <= 0)
        {
            return false;
        }

        RoundRules round = GetCurrentRound();

        if (round.MaxRaises.HasValue && _raiseCount >= round.MaxRaises.Value)
        {
            return false;
        }

        return GetMaximumRaiseTo(seat) > GetHighestRoundBet();
    }

    private int GetFirstSeatToAct()
    {
        if (Round == RoundType.Preflop)
        {
            int? lastStraddleSeatId = _events.OfType<PlayerPostedEvent>().Where(post => post.postType == PostType.Straddle).Select(post => (int?)post.seatId).LastOrDefault();

            if (lastStraddleSeatId.HasValue)
            {
                return FindNextActionableSeat(lastStraddleSeatId.Value);
            }

            return _seats.Count == 2 ? FindNextActionableSeat(-1) : FindNextActionableSeat(1);
        }

        return _seats.Count == 2 ? FindNextActionableSeat(0) : FindNextActionableSeat(_seats.Count - 1);
    }

    private int FindNextActionableSeat(int seatId)
    {
        for (int offset = 1; offset <= _seats.Count; offset++)
        {
            int nextSeatId = (seatId + offset + _seats.Count) % _seats.Count;
            Seat seat = _seats[nextSeatId];

            if (!seat.IsFolded && !seat.IsAllIn)
            {
                return nextSeatId;
            }
        }

        throw new InvalidOperationException("Нет игрока, способного выполнить действие.");
    }

    private int CountActionableSeats()
    {
        return _seats.Count(seat => !seat.IsFolded && !seat.IsAllIn);
    }

    private bool AllRemainingPlayersAllIn()
    {
        Seat[] remaining = _seats.Where(seat => !seat.IsFolded).ToArray();

        return remaining.Length > 1 && remaining.All(seat => seat.IsAllIn);
    }

    private bool HasUndealtBoardCards()
    {
        int totalBoardCards = _rules.Rounds.Sum(round => round.BoardCardCount);

        return _boards.Any(board => board.Count < totalBoardCards);
    }

    private void DealCurrentRoundBoardsAutomatically()
    {
        int roundIndex = _roundIndex;
        int requiredCount = GetBoardCardCountAfter(roundIndex);

        for (int boardIndex = 0; boardIndex < _boards.Count; boardIndex++)
        {
            if (_boards[boardIndex].Count < requiredCount)
            {
                DealBoard(boardIndex);
            }
        }
    }

    private void DealRemainingBoardsAutomatically()
    {
        while (HasUndealtBoardCards())
        {
            for (int boardIndex = 0; boardIndex < _boards.Count; boardIndex++)
            {
                int totalBoardCards = _rules.Rounds.Sum(round => round.BoardCardCount);

                if (_boards[boardIndex].Count < totalBoardCards)
                {
                    DealBoard(boardIndex);
                }
            }
        }

        StartShowdown();
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

    private RoundRules GetCurrentRound()
    {
        if (_roundIndex < 0 || _roundIndex >= _rules.Rounds.Count)
        {
            throw new InvalidOperationException("Текущий раунд не определён.");
        }

        return _rules.Rounds[_roundIndex];
    }

    private bool IsFinalRound()
    {
        return _roundIndex == _rules.Rounds.Count - 1;
    }

    private int GetHoleCardCount()
    {
        return _rules.GameType switch
        {
            GameType.TexasHoldem => 2,
            GameType.Omaha4c => 4,
            GameType.Omaha5c => 5,
            GameType.Omaha6c => 6,
            _ => throw new NotSupportedException($"Тип игры {_rules.GameType} не поддерживается.")
        };
    }

    private Seat GetSeat(int seatId)
    {
        if (seatId < 0 || seatId >= _seats.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(seatId), $"SeatId должен быть от 0 до {_seats.Count - 1}.");
        }

        return _seats[seatId];
    }

    private Seat GetActionSeat(int seatId)
    {
        Seat seat = GetSeat(seatId);

        if (seat.IsFolded)
        {
            throw new InvalidOperationException($"Игрок на месте {seatId} уже сделал fold.");
        }

        if (seat.IsAllIn)
        {
            throw new InvalidOperationException($"Игрок на месте {seatId} уже находится в all-in.");
        }

        return seat;
    }

    private void ValidateBoardIndex(int boardIndex)
    {
        if (boardIndex < 0 || boardIndex >= _boards.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(boardIndex), $"BoardIndex должен быть от 0 до {_boards.Count - 1}.");
        }
    }

    private static void ValidateCards(IReadOnlyList<string> cards, int expectedCount)
    {
        ArgumentNullException.ThrowIfNull(cards);

        if (cards.Count != expectedCount)
        {
            throw new ArgumentException($"Необходимо передать {expectedCount} карт.", nameof(cards));
        }

        if (cards.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Список содержит пустую карту.", nameof(cards));
        }

        if (cards.Distinct().Count() != cards.Count)
        {
            throw new ArgumentException("Список содержит повторяющиеся карты.", nameof(cards));
        }
    }

    private void ValidateRules()
    {
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

        if (_rules.Rounds.Select(round => round.Type).Distinct().Count() != _rules.Rounds.Count)
        {
            throw new InvalidOperationException("Типы раундов не должны повторяться.");
        }
    }

    private static bool IsLivePost(PostType postType)
    {
        return postType is PostType.SmallBlind or PostType.BigBlind or PostType.ExtraBlind or PostType.Straddle;
    }

    private bool IsAutomated(Automation automation)
    {
        return (_rules.Automation & automation) == automation;
    }

    private void EnsureStarted()
    {
        if (_state != HandState.Started)
        {
            throw new InvalidOperationException("Раздача не запущена.");
        }
    }

    private void EnsureState(HandState expectedState)
    {
        if (_state != expectedState)
        {
            throw new InvalidOperationException($"Ожидалось состояние {expectedState}, текущее состояние {_state}.");
        }
    }

    private void Emit(PokerHandEvent handEvent)
    {
        _events.Add(handEvent);
    }

    private bool DeckWasAlreadyUsed(string card)
    {
        return _seats.Any(seat => seat.HoleCards.Contains(card)) || _boards.Any(board => board.Contains(card));
    }

    private sealed record PotSlice(int Index, long Amount, IReadOnlyList<int> EligibleSeatIds);

    private sealed record EvaluatedHand(long Strength, HandCategory Category, IReadOnlyList<string> BestCards);

    private sealed class PotBuilder
    {
        public long Amount { get; set; }

        public required IReadOnlyList<int> EligibleSeatIds { get; init; }

        public required Dictionary<int, long> Contributions { get; init; }
    }
}