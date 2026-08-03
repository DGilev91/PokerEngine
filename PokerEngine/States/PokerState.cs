using PokerEngine.Cards;
using PokerEngine.Enums;
using PokerEngine.Evaluation;
using PokerEngine.Rules;
using PokerEngine.States.Events;
using PokerEngine.States.Pots;
using PokerEngine.States.Seats;

namespace PokerEngine.States;

/// <summary>
/// Represents the mutable state of a single poker hand.
/// </summary>
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
    private readonly HashSet<int> _actedSeats = [];
    private readonly HashSet<(int SeatId, PostType PostType)> _madePosts = [];
    private readonly PotState _potState = new();

    private HandState _state = HandState.None;
    private int _roundIndex = -1;
    private int? _activeSeatId;
    private bool _runoutCountWasSet;
    private bool _waitingForRunoutDecision;
    private bool _dealingRemainingBoardsAutomatically;

    private long _lastFullRaiseSize;
    private int _raiseCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="PokerState"/> class.
    /// </summary>
    /// <param name="rules">
    /// The rules used to configure the hand.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="rules"/> is <see langword="null"/>.
    /// </exception>
    public PokerState(PokerRules rules)
    {
        _rules = rules ?? throw new ArgumentNullException(nameof(rules));
        _deck = new Deck();

        _handEvaluator = _rules.GameType switch
        {
            GameType.TexasHoldem => new TexasHoldemEvaluator(),
            _ => throw new NotSupportedException(
                $"Game type {_rules.GameType} is not supported yet.")
        };
    }

    /// <inheritdoc />
    public IReadOnlyList<PokerHandEvent> Events => _events;

    /// <inheritdoc />
    public IReadOnlyList<Seat> Seats => _seats;

    /// <inheritdoc />
    public PotState PotState => _potState;

    /// <inheritdoc />
    public IReadOnlyList<IReadOnlyList<string>> Boards => _boards;

    /// <inheritdoc />
    public HandState State => _state;

    /// <inheritdoc />
    public RoundType Round =>
        _roundIndex == _rules.Rounds.Count
            ? RoundType.Showdown
            : _roundIndex >= 0 && _roundIndex < _rules.Rounds.Count
                ? _rules.Rounds[_roundIndex].Type
                : RoundType.None;

    /// <inheritdoc />
    public void Initialize(IReadOnlyList<long> stacks)
    {
        ArgumentNullException.ThrowIfNull(stacks);

        if (_state != HandState.None)
        {
            throw new InvalidOperationException(
                "The hand has already been initialized.");
        }

        if (stacks.Count < 2)
        {
            throw new ArgumentException(
                "At least two players are required.",
                nameof(stacks));
        }

        if (stacks.Any(stack => stack <= 0))
        {
            throw new ArgumentException(
                "Every player stack must be greater than zero.",
                nameof(stacks));
        }

        ValidateRules(stacks.Count);

        _events.Clear();
        _seats.Clear();
        _boards.Clear();
        _actedSeats.Clear();
        _madePosts.Clear();
        _potState.Clear();

        _roundIndex = -1;
        _activeSeatId = null;

        _runoutCountWasSet = false;
        _waitingForRunoutDecision = false;
        _dealingRemainingBoardsAutomatically = false;

        _lastFullRaiseSize = 0;
        _raiseCount = 0;

        for (int seatId = 0; seatId < stacks.Count; seatId++)
        {
            _seats.Add(new Seat(seatId, stacks[seatId]));
        }

        for (int boardIndex = 0;
             boardIndex < _rules.InitialBoardCount;
             boardIndex++)
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

        PostAntesAutomatically();
        PostBlindsAutomatically();
        PostMandatoryStraddlesAutomatically();
    }

    /// <inheritdoc />
    public void PlayerPost(int seatId, PostType postType, long amount)
    {
        EnsureState(HandState.Initialized);

        Seat seat = GetSeat(seatId);

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Post amount must be greater than zero.");
        }

        ValidatePost(seat, postType, amount);

        long paid = Math.Min(amount, seat.Stack);

        seat.Stack -= paid;
        seat.TotalBet += paid;

        if (IsLivePost(postType))
        {
            seat.RoundBet += paid;
        }

        _madePosts.Add((seatId, postType));

        Emit(new PlayerPostedEvent(
            seatId,
            postType,
            paid,
            seat.IsAllIn));

        RecalculatePots();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void DealHole(
        int seatId,
        IReadOnlyList<string>? cards = null)
    {
        EnsureStarted();

        Seat seat = GetSeat(seatId);

        if (seat.HoleCards.Count > 0)
        {
            throw new InvalidOperationException(
                $"Seat {seatId} has already received hole cards.");
        }

        int cardCount = GetHoleCardCount();
        string[] dealtCards;

        if (cards is null)
        {
            if (!IsAutomated(Automation.DealHoleCards))
            {
                throw new InvalidOperationException(
                    "Hole cards must be provided in manual mode.");
            }

            dealtCards =
                _deck.Deal(cardCount).ToArray();
        }
        else
        {
            ValidateCards(
                cards,
                cardCount);

            dealtCards =
                cards.ToArray();

            EnsureCardsWereNotUsed(dealtCards);
            _deck.Take(dealtCards);
        }

        seat.SetHoleCards(dealtCards);

        Emit(new HoleCardsEvent(
            seatId,
            dealtCards));
    }

    /// <inheritdoc />
    public void SelectRunoutCount(int count)
    {
        EnsureStarted();

        if (count < 1 || count > _rules.MaxRunoutCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                $"Runout count must be between 1 and {_rules.MaxRunoutCount}.");
        }

        if (_runoutCountWasSet)
        {
            throw new InvalidOperationException(
                "Runout count has already been set.");
        }

        if (!_waitingForRunoutDecision && count > 1)
        {
            throw new InvalidOperationException(
                "Multiple runouts cannot be selected in the current state.");
        }

        _runoutCountWasSet = true;
        _waitingForRunoutDecision = false;

        int requiredBoardCount =
            _rules.InitialBoardCount * count;

        while (_boards.Count < requiredBoardCount)
        {
            int sourceBoardIndex =
                _boards.Count % _rules.InitialBoardCount;

            _boards.Add([.. _boards[sourceBoardIndex]]);
        }

        Emit(new RunoutCountSelectedEvent(count));

        ContinueAfterRunoutDecision();
    }

    /// <inheritdoc />
    public void DealBoard(
        int boardIndex = 0,
        IReadOnlyList<string>? cards = null)
    {
        EnsureStarted();
        ValidateBoardIndex(boardIndex);

        List<string> board = _boards[boardIndex];

        int roundIndex =
            FindNextBoardRoundIndex(board.Count);

        RoundRules round = _rules.Rounds[roundIndex];

        string[] dealtCards;

        if (cards is null)
        {
            if (!IsAutomated(Automation.DealBoard))
            {
                throw new InvalidOperationException(
                    "Board cards must be provided in manual mode.");
            }

            if (IsAutomated(Automation.BurnCards))
            {
                _deck.Deal();
            }

            dealtCards =
                _deck.Deal(round.BoardCardCount).ToArray();
        }
        else
        {
            ValidateCards(cards, round.BoardCardCount);

            dealtCards = cards.ToArray();
            _deck.Take(dealtCards);
        }

        board.AddRange(dealtCards);

        Emit(new BoardEvent(
            round.Type,
            boardIndex,
            dealtCards));

        if (!AllBoardsReachedRound(roundIndex))
        {
            return;
        }

        _roundIndex = roundIndex;

        if (_dealingRemainingBoardsAutomatically)
        {
            return;
        }

        if (IsFinalRound())
        {
            if (IsBettingClosedByAllIn())
            {
                StartShowdown();
            }
            else
            {
                BeginBettingRound();
            }

            return;
        }

        if (IsBettingClosedByAllIn())
        {
            ContinueAllInRunout();
            return;
        }

        BeginBettingRound();
    }

    /// <inheritdoc />
    public void PlayerAction(
        int seatId,
        ActionType actionType,
        long amount = 0)
    {
        EnsureStarted();

        if (_waitingForRunoutDecision)
        {
            throw new InvalidOperationException(
                "Runout count must be selected first.");
        }

        if (_activeSeatId != seatId)
        {
            throw new InvalidOperationException(
                $"It is currently seat {_activeSeatId}'s turn.");
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
                throw new ArgumentOutOfRangeException(
                    nameof(actionType),
                    actionType,
                    "Unknown action.");
        }

        ContinueAfterAction(seatId);
    }

    /// <inheritdoc />
    public void ShowCards(
        int seatId,
        IReadOnlyList<string> cards)
    {
        EnsureStarted();
        ArgumentNullException.ThrowIfNull(cards);

        Seat seat = GetSeat(seatId);
        int expectedCount = GetHoleCardCount();

        ValidateCards(
            cards,
            expectedCount);

        string[] shownCards =
            cards.ToArray();

        if (seat.HoleCards.Count == 0)
        {
            EnsureCardsWereNotUsed(shownCards);
            _deck.Take(shownCards);

            seat.SetHoleCards(shownCards);
        }
        else
        {
            ValidateShownCardsMatchDealtCards(
                seat,
                shownCards);
        }

        Emit(new ShowCardsEvent(
            seatId,
            shownCards));

        if (Round == RoundType.Showdown)
        {
            TryCompleteShowdown();
        }
    }

    // Automatic posts

    private void PostAntesAutomatically()
    {
        if (!IsAutomated(Automation.PostAntes))
        {
            return;
        }

        AnteRules? ante = _rules.Ante;

        if (ante is null || ante.Amount <= 0)
        {
            return;
        }

        switch (ante.Type)
        {
            case AnteType.EveryPlayer:
                foreach (Seat seat in _seats)
                {
                    PlayerPost(
                        seat.SeatId,
                        PostType.Ante,
                        ante.Amount);
                }

                break;

            default:
                throw new NotSupportedException(
                    $"Automatic ante type {ante.Type} is not supported yet.");
        }
    }

    private void PostBlindsAutomatically()
    {
        if (!IsAutomated(Automation.PostBlinds))
        {
            return;
        }

        if (_seats.Count < 2)
        {
            throw new InvalidOperationException(
                "At least two players are required for automatic blind posting.");
        }

        PlayerPost(
            0,
            PostType.SmallBlind,
            _rules.SmallBlind);

        PlayerPost(
            1,
            PostType.BigBlind,
            _rules.BigBlind);
    }

    private void PostMandatoryStraddlesAutomatically()
    {
        if (!IsAutomated(Automation.PostStraddles))
        {
            return;
        }

        StraddleRules? straddle = _rules.Straddle;

        if (straddle is null ||
            !straddle.IsMandatory ||
            straddle.Amounts.Count == 0)
        {
            return;
        }

        switch (straddle.Type)
        {
            case StraddleType.Utg:
                PostMandatoryUtgStraddles(straddle);
                break;

            case StraddleType.Button:
                PostMandatoryButtonStraddle(straddle);
                break;

            case StraddleType.Mississippi:
                throw new NotSupportedException(
                    "Automatic Mississippi straddle requires an explicit first seat.");

            case StraddleType.AnyPosition:
                throw new NotSupportedException(
                    "Automatic AnyPosition straddle requires an explicit seatId.");

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(straddle.Type),
                    straddle.Type,
                    "Unknown straddle type.");
        }
    }

    private void PostMandatoryUtgStraddles(
        StraddleRules straddle)
    {
        int availableSeatCount = _seats.Count - 2;

        if (straddle.Amounts.Count > availableSeatCount)
        {
            throw new InvalidOperationException(
                "The number of mandatory UTG straddles exceeds the available seats after the big blind.");
        }

        for (int index = 0;
             index < straddle.Amounts.Count;
             index++)
        {
            int seatId = 2 + index;

            PlayerPost(
                seatId,
                PostType.Straddle,
                straddle.Amounts[index]);
        }
    }

    private void PostMandatoryButtonStraddle(
        StraddleRules straddle)
    {
        if (straddle.Amounts.Count != 1)
        {
            throw new InvalidOperationException(
                "Only one automatic button straddle is supported.");
        }

        int buttonSeatId = _seats.Count - 1;

        PlayerPost(
            buttonSeatId,
            PostType.Straddle,
            straddle.Amounts[0]);
    }

    // Player actions

    private void Fold(Seat seat)
    {
        if (seat.IsFolded)
        {
            throw new InvalidOperationException(
                $"Seat {seat.SeatId} has already folded.");
        }

        seat.IsFolded = true;
        _actedSeats.Add(seat.SeatId);

        _potState.RefreshUncalledBet(_seats);

        Emit(new PlayerActionEvent(
            seat.SeatId,
            ActionType.Fold,
            0,
            false));

        RecalculatePots();
    }

    private void Check(Seat seat)
    {
        long highestBet = GetHighestRoundBet();

        if (seat.RoundBet != highestBet)
        {
            throw new InvalidOperationException(
                $"Seat {seat.SeatId} cannot check.");
        }

        _actedSeats.Add(seat.SeatId);

        Emit(new PlayerActionEvent(
            seat.SeatId,
            ActionType.Check,
            0,
            false));
    }

    private void Call(Seat seat)
    {
        long highestBet = GetHighestRoundBet();
        long amountToCall = highestBet - seat.RoundBet;

        if (amountToCall <= 0)
        {
            throw new InvalidOperationException(
                $"Seat {seat.SeatId} has nothing to call.");
        }

        long paid = CommitTo(seat, highestBet);

        _actedSeats.Add(seat.SeatId);

        _potState.RefreshUncalledBet(_seats);

        Emit(new PlayerActionEvent(
            seat.SeatId,
            ActionType.Call,
            paid,
            seat.IsAllIn));

        RecalculatePots();
    }

    private void Bet(Seat seat, long amount)
    {
        if (GetHighestRoundBet() > 0)
        {
            throw new InvalidOperationException(
                "Bet is not allowed when a wager already exists. Use RaiseTo.");
        }

        RoundRules round = GetCurrentRound();
        long maximum = GetMaximumBetTo(seat);

        if (amount <= 0 || amount > maximum)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                $"Bet amount must be between 1 and {maximum}.");
        }

        if (amount < round.BetSize &&
            amount < seat.RoundBet + seat.Stack)
        {
            throw new InvalidOperationException(
                $"Minimum bet is {round.BetSize}.");
        }

        long previousBet = seat.RoundBet;
        long paid = CommitTo(seat, amount);
        long betSize = seat.RoundBet - previousBet;

        _lastFullRaiseSize =
            Math.Max(betSize, round.BetSize);

        ResetActedAfterAggression(seat.SeatId);
        _potState.SetUncalledCandidate(seat.SeatId, _seats);

        Emit(new PlayerActionEvent(
            seat.SeatId,
            ActionType.Bet,
            paid,
            seat.IsAllIn));

        RecalculatePots();
    }

    private void RaiseTo(Seat seat, long amount)
    {
        long highestBet = GetHighestRoundBet();

        if (highestBet <= 0)
        {
            throw new InvalidOperationException(
                "Raise is not allowed when no wager exists. Use Bet.");
        }

        long maximumRaiseTo =
            GetMaximumRaiseTo(seat);

        if (amount <= highestBet ||
            amount > maximumRaiseTo)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                $"RaiseTo must be greater than {highestBet} and no greater than {maximumRaiseTo}.");
        }

        long minimumRaiseTo =
            highestBet + _lastFullRaiseSize;

        long availableTotal =
            seat.RoundBet + seat.Stack;

        bool isAllInRaise =
            amount >= availableTotal;

        if (amount < minimumRaiseTo && !isAllInRaise)
        {
            throw new InvalidOperationException(
                $"Minimum RaiseTo is {minimumRaiseTo}.");
        }

        RoundRules round = GetCurrentRound();

        if (round.MaxRaises.HasValue &&
            _raiseCount >= round.MaxRaises.Value)
        {
            throw new InvalidOperationException(
                "The maximum number of raises has been reached.");
        }

        long paid = CommitTo(seat, amount);
        long raiseSize = seat.RoundBet - highestBet;
        bool isFullRaise =
            raiseSize >= _lastFullRaiseSize;

        if (isFullRaise)
        {
            _lastFullRaiseSize = raiseSize;
            _raiseCount++;

            ResetActedAfterAggression(seat.SeatId);
        }
        else
        {
            _actedSeats.Add(seat.SeatId);
        }

        _potState.SetUncalledCandidate(seat.SeatId, _seats);

        Emit(new PlayerActionEvent(
            seat.SeatId,
            ActionType.RaiseTo,
            paid,
            seat.IsAllIn));

        RecalculatePots();
    }

    // Progression

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

        if (IsBettingClosedByAllIn())
        {
            ContinueAllInRunout();
            return;
        }

        MoveToNextRound();
    }

    private void ContinueAllInRunout()
    {
        _activeSeatId = null;

        if (HasUndealtBoardCards() &&
            _rules.MaxRunoutCount > 1 &&
            !_runoutCountWasSet)
        {
            _waitingForRunoutDecision = true;
            Emit(new WaitingRunoutEvent(_rules.MaxRunoutCount));
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

        long callAmount = Math.Min(
            Math.Max(0, highestBet - seat.RoundBet),
            seat.Stack);

        long minBet = 0;
        long maxBet = 0;
        long minRaiseTo = 0;
        long maxRaiseTo = 0;

        List<ActionType> actions = [];

        actions.Add(ActionType.Fold);
        if (callAmount > 0)
        {
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

                minBet = Math.Min(
                    GetCurrentRound().BetSize,
                    seat.Stack);

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

        if (minRaiseTo > maxRaiseTo)
        {
            minRaiseTo = maxRaiseTo;
        }

        _activeSeatId = seatId;

        Emit(new PlayerTurnEvent(
            seatId,
            actions,
            callAmount,
            minBet,
            maxBet,
            minRaiseTo,
            maxRaiseTo));
    }

    private bool IsBettingRoundCompleted()
    {
        long highestBet = GetHighestRoundBet();

        Seat[] actionableSeats = _seats
            .Where(seat =>
                !seat.IsFolded &&
                !seat.IsAllIn)
            .ToArray();

        if (actionableSeats.Length == 0)
        {
            return true;
        }

        return actionableSeats.All(seat =>
            _actedSeats.Contains(seat.SeatId) &&
            seat.RoundBet == highestBet);
    }

    private bool TryCompleteByFold()
    {
        Seat[] remaining = _seats
            .Where(seat => !seat.IsFolded)
            .ToArray();

        if (remaining.Length != 1)
        {
            return false;
        }

        Seat winner = remaining[0];

        if (_potState.UncalledBet is not null)
        {
            ReturnUncalledBet();
        }
        else
        {
            ReturnUncalledForcedPostOnPreflopWalk(
                winner);
        }

        RecalculatePots();

        foreach (Pot pot in _potState.Pots)
        {
            winner.Stack += pot.Amount;

            Emit(new PotAwardedEvent(
                pot.Index,
                0,
                winner.SeatId,
                pot.Amount));
        }

        CompleteHand();

        return true;
    }

    // Showdown

    private void StartShowdown()
    {
        if (_state == HandState.Completed)
        {
            return;
        }

        if (_roundIndex == _rules.Rounds.Count)
        {
            return;
        }

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

        Seat[] contenders = _seats
            .Where(seat => !seat.IsFolded)
            .ToArray();

        if (contenders.Any(
                seat =>
                    seat.HoleCards.Count != GetHoleCardCount()))
        {
            return;
        }

        RecalculatePots();
        EvaluateAndAwardPots();
        CompleteHand();
    }

    private void EvaluateAndAwardPots()
    {
        if (_state == HandState.Completed)
        {
            return;
        }

        Dictionary<
            (int seatId, int boardIndex),
            EvaluatedHand> evaluations = [];

        foreach (Seat seat in _seats.Where(
                     seat => !seat.IsFolded))
        {
            for (int boardIndex = 0;
                 boardIndex < _boards.Count;
                 boardIndex++)
            {
                EvaluatedHand evaluation =
                    EvaluateHand(seat, boardIndex);

                evaluations[(seat.SeatId, boardIndex)] =
                    evaluation;

                Emit(new HandEvaluatedEvent(
                    seat.SeatId,
                    boardIndex,
                    evaluation.Category,
                    evaluation.BestCards));
            }
        }

        foreach (Pot pot in _potState.Pots)
        {
            long boardShare =
                pot.Amount / _boards.Count;

            long boardRemainder =
                pot.Amount % _boards.Count;

            for (int boardIndex = 0;
                 boardIndex < _boards.Count;
                 boardIndex++)
            {
                long amountForBoard =
                    boardShare +
                    (boardIndex < boardRemainder ? 1 : 0);

                if (amountForBoard <= 0)
                {
                    continue;
                }

                int[] eligibleSeats = pot
                    .EligibleSeatIds
                    .Where(seatId =>
                        !_seats[seatId].IsFolded)
                    .ToArray();

                if (eligibleSeats.Length == 0)
                {
                    continue;
                }

                long bestStrength = eligibleSeats.Max(
                    seatId =>
                        evaluations[(seatId, boardIndex)]
                            .Strength);

                int[] winners = eligibleSeats
                    .Where(seatId =>
                        evaluations[(seatId, boardIndex)]
                            .Strength == bestStrength)
                    .Order()
                    .ToArray();

                long winnerShare =
                    amountForBoard / winners.Length;

                long winnerRemainder =
                    amountForBoard % winners.Length;

                for (int index = 0;
                     index < winners.Length;
                     index++)
                {
                    int winnerSeatId = winners[index];

                    long awarded =
                        winnerShare +
                        (index < winnerRemainder ? 1 : 0);

                    _seats[winnerSeatId].Stack += awarded;

                    Emit(new PotAwardedEvent(
                        pot.Index,
                        boardIndex,
                        winnerSeatId,
                        awarded));
                }
            }
        }
    }

    private EvaluatedHand EvaluateHand(
        Seat seat,
        int boardIndex)
    {
        if (seat.HoleCards.Count !=
            GetHoleCardCount())
        {
            throw new InvalidOperationException(
                $"Cannot evaluate seat {seat.SeatId}: hole cards are not known.");
        }

        HandRank result = _handEvaluator.Evaluate(
            seat.HoleCards,
            _boards[boardIndex]);

        return new EvaluatedHand(
            result.Strength,
            result.Category,
            result.Cards.ToArray());
    }

    private void CompleteHand()
    {
        if (_state == HandState.Completed)
        {
            return;
        }

        _activeSeatId = null;
        _waitingForRunoutDecision = false;
        _dealingRemainingBoardsAutomatically = false;
        _state = HandState.Completed;

        Emit(new EndHandEvent());
    }

    // Uncalled bet

    private void ReturnUncalledBet()
    {
        UncalledBet? uncalledBet =
            _potState.TakeUncalledBet(_seats);

        if (uncalledBet is null)
        {
            return;
        }

        Seat seat = GetSeat(uncalledBet.SeatId);

        seat.RoundBet -= uncalledBet.Amount;
        seat.TotalBet -= uncalledBet.Amount;
        seat.Stack += uncalledBet.Amount;

        Emit(new UncalledBetReturnedEvent(
            seat.SeatId,
            uncalledBet.Amount));

        RecalculatePots();
    }

    private void ReturnUncalledForcedPostOnPreflopWalk(
        Seat winner)
    {
        if (Round != RoundType.Preflop)
        {
            return;
        }

        bool hasVoluntaryAggression = _events
            .OfType<PlayerActionEvent>()
            .Any(action =>
                action.ActionType is
                    ActionType.Bet or
                    ActionType.RaiseTo);

        if (hasVoluntaryAggression)
        {
            return;
        }

        long winnerLivePostAmount = _events
            .OfType<PlayerPostedEvent>()
            .Where(post =>
                post.SeatId == winner.SeatId &&
                post.PostType is
                    PostType.BigBlind or
                    PostType.ExtraBlind or
                    PostType.Straddle)
            .Sum(post => post.Amount);

        long amount = Math.Min(
            winner.RoundBet,
            winnerLivePostAmount);

        if (amount <= 0)
        {
            return;
        }

        winner.RoundBet -= amount;
        winner.TotalBet -= amount;
        winner.Stack += amount;

        Emit(new UncalledBetReturnedEvent(
            winner.SeatId,
            amount));
    }

    // Pots

    private void RecalculatePots()
    {
        _potState.Rebuild(_seats);
    }

    private long CommitTo(Seat seat, long amountTo)
    {
        if (amountTo < seat.RoundBet)
        {
            throw new InvalidOperationException(
                "The target wager cannot be lower than the seat's current wager.");
        }

        long requested =
            amountTo - seat.RoundBet;

        long paid =
            Math.Min(requested, seat.Stack);

        seat.Stack -= paid;
        seat.RoundBet += paid;
        seat.TotalBet += paid;

        return paid;
    }

    // Round state

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
        _potState.ClearUncalledBet();

        _raiseCount = 0;

        RoundRules round = GetCurrentRound();

        if (round.Type == RoundType.Preflop)
        {
            long highestForcedBet = GetHighestRoundBet();

            _lastFullRaiseSize = highestForcedBet > 0
                ? highestForcedBet
                : _rules.BigBlind;
        }
        else
        {
            _lastFullRaiseSize = round.BetSize > 0
                ? round.BetSize
                : _rules.BigBlind;
        }
    }

    private void ResetActedAfterAggression(
        int aggressorSeatId)
    {
        _actedSeats.Clear();
        _actedSeats.Add(aggressorSeatId);
    }

    // Betting values

    private long GetHighestRoundBet()
    {
        long highestBet = 0;

        foreach (Seat seat in _seats)
        {
            if (seat.RoundBet > highestBet)
            {
                highestBet = seat.RoundBet;
            }
        }

        return highestBet;
    }

    private long GetMinimumRaiseTo()
    {
        return GetHighestRoundBet() +
               _lastFullRaiseSize;
    }

    private long GetMaximumBetTo(Seat seat)
    {
        long availableTotal =
            seat.RoundBet + seat.Stack;

        return _rules.GameLimit switch
        {
            GameLimit.FixedLimit =>
                Math.Min(
                    GetCurrentRound().BetSize,
                    availableTotal),

            GameLimit.PotLimit =>
                Math.Min(
                    GetCurrentPotAmount(),
                    availableTotal),

            GameLimit.NoLimit =>
                availableTotal,

            _ => throw new NotSupportedException(
                $"Game limit {_rules.GameLimit} is not supported.")
        };
    }

    private long GetMaximumRaiseTo(Seat seat)
    {
        long highestBet = GetHighestRoundBet();

        long amountToCall = Math.Max(
            0,
            highestBet - seat.RoundBet);

        long availableTotal =
            seat.RoundBet + seat.Stack;

        return _rules.GameLimit switch
        {
            GameLimit.FixedLimit =>
                Math.Min(
                    highestBet +
                    GetCurrentRound().BetSize,
                    availableTotal),

            GameLimit.PotLimit =>
                Math.Min(
                    highestBet +
                    GetCurrentPotAmount() +
                    amountToCall,
                    availableTotal),

            GameLimit.NoLimit =>
                availableTotal,

            _ => throw new NotSupportedException(
                $"Game limit {_rules.GameLimit} is not supported.")
        };
    }

    private long GetCurrentPotAmount()
    {
        return _seats.Sum(
            seat => seat.TotalBet);
    }

    private bool CanRaise(Seat seat)
    {
        if (seat.Stack <= 0)
        {
            return false;
        }

        RoundRules round = GetCurrentRound();

        if (round.MaxRaises.HasValue &&
            _raiseCount >= round.MaxRaises.Value)
        {
            return false;
        }

        return GetMaximumRaiseTo(seat) >
               GetHighestRoundBet();
    }

    // Action order

    private int GetFirstSeatToAct()
    {
        if (Round == RoundType.Preflop)
        {
            int? lastStraddleSeatId = _events
                .OfType<PlayerPostedEvent>()
                .Where(post =>
                    post.PostType == PostType.Straddle)
                .Select(post => (int?)post.SeatId)
                .LastOrDefault();

            if (lastStraddleSeatId.HasValue)
            {
                return FindNextActionableSeat(
                    lastStraddleSeatId.Value);
            }

            return _seats.Count == 2
                ? FindNextActionableSeat(-1)
                : FindNextActionableSeat(1);
        }

        return _seats.Count == 2
            ? FindNextActionableSeat(0)
            : FindNextActionableSeat(_seats.Count - 1);
    }

    private int FindNextActionableSeat(int seatId)
    {
        for (int offset = 1;
             offset <= _seats.Count;
             offset++)
        {
            int nextSeatId =
                (seatId + offset + _seats.Count) %
                _seats.Count;

            Seat seat = _seats[nextSeatId];

            if (!seat.IsFolded && !seat.IsAllIn)
            {
                return nextSeatId;
            }
        }

        throw new InvalidOperationException(
            "No actionable seat is available.");
    }

    private int CountActionableSeats()
    {
        int count = 0;

        foreach (Seat seat in _seats)
        {
            if (!seat.IsFolded && !seat.IsAllIn)
            {
                count++;
            }
        }

        return count;
    }

    private bool IsBettingClosedByAllIn()
    {
        int remainingCount = 0;
        int actionableCount = 0;

        foreach (Seat seat in _seats)
        {
            if (seat.IsFolded)
            {
                continue;
            }

            remainingCount++;

            if (!seat.IsAllIn)
            {
                actionableCount++;
            }
        }

        return remainingCount > 1 &&
               actionableCount <= 1;
    }

    // Boards

    private bool HasUndealtBoardCards()
    {
        int totalBoardCards = _rules.Rounds.Sum(
            round => round.BoardCardCount);

        return _boards.Any(
            board => board.Count < totalBoardCards);
    }

    private void DealCurrentRoundBoardsAutomatically()
    {
        int roundIndex = _roundIndex;

        int requiredCount =
            GetBoardCardCountAfter(roundIndex);

        for (int boardIndex = 0;
             boardIndex < _boards.Count;
             boardIndex++)
        {
            if (_boards[boardIndex].Count < requiredCount)
            {
                DealBoard(boardIndex);
            }
        }
    }

    private void DealRemainingBoardsAutomatically()
    {
        if (_dealingRemainingBoardsAutomatically ||
            _state == HandState.Completed)
        {
            return;
        }

        _dealingRemainingBoardsAutomatically = true;

        try
        {
            int totalBoardCards = _rules.Rounds.Sum(
                round => round.BoardCardCount);

            while (HasUndealtBoardCards())
            {
                bool dealtAnyBoard = false;

                for (int boardIndex = 0;
                     boardIndex < _boards.Count;
                     boardIndex++)
                {
                    if (_boards[boardIndex].Count >=
                        totalBoardCards)
                    {
                        continue;
                    }

                    DealBoard(boardIndex);
                    dealtAnyBoard = true;
                }

                if (!dealtAnyBoard)
                {
                    throw new InvalidOperationException(
                        "Unable to continue automatic board dealing.");
                }
            }
        }
        finally
        {
            _dealingRemainingBoardsAutomatically = false;
        }

        StartShowdown();
    }

    private int FindNextBoardRoundIndex(
        int boardCardCount)
    {
        for (int roundIndex = 0;
             roundIndex < _rules.Rounds.Count;
             roundIndex++)
        {
            RoundRules round =
                _rules.Rounds[roundIndex];

            if (round.BoardCardCount <= 0)
            {
                continue;
            }

            if (boardCardCount ==
                GetBoardCardCountBefore(roundIndex))
            {
                return roundIndex;
            }
        }

        throw new InvalidOperationException(
            $"Unable to determine the next street. The board already contains {boardCardCount} cards.");
    }

    private bool AllBoardsReachedRound(int roundIndex)
    {
        int requiredCardCount =
            GetBoardCardCountAfter(roundIndex);

        return _boards.All(
            board => board.Count >= requiredCardCount);
    }

    private int GetBoardCardCountBefore(
        int roundIndex)
    {
        int cardCount = 0;

        for (int index = 0;
             index < roundIndex;
             index++)
        {
            cardCount +=
                _rules.Rounds[index].BoardCardCount;
        }

        return cardCount;
    }

    private int GetBoardCardCountAfter(
        int roundIndex)
    {
        int cardCount = 0;

        for (int index = 0;
             index <= roundIndex;
             index++)
        {
            cardCount +=
                _rules.Rounds[index].BoardCardCount;
        }

        return cardCount;
    }

    // Post validation

    private void ValidatePost(
        Seat seat,
        PostType postType,
        long amount)
    {
        if (!Enum.IsDefined(postType))
        {
            throw new ArgumentOutOfRangeException(
                nameof(postType),
                postType,
                "Unknown post type.");
        }

        if (seat.Stack <= 0)
        {
            throw new InvalidOperationException(
                $"Seat {seat.SeatId} has no chips available.");
        }

        switch (postType)
        {
            case PostType.Ante:
                ValidateAntePost(seat, amount);
                break;

            case PostType.SmallBlind:
                ValidateSmallBlindPost(seat, amount);
                break;

            case PostType.BigBlind:
                ValidateBigBlindPost(seat, amount);
                break;

            case PostType.Straddle:
                ValidateStraddlePost(seat, amount);
                break;

            case PostType.ExtraBlind:
                ValidateExtraBlindPost(seat);
                break;

            case PostType.DeadBlind:
                ValidateDeadBlindPost(seat);
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(postType),
                    postType,
                    "Unsupported post type.");
        }
    }

    private void ValidateAntePost(
        Seat seat,
        long amount)
    {
        AnteRules? ante = _rules.Ante;

        if (ante is null || ante.Amount <= 0)
        {
            throw new InvalidOperationException(
                "Ante is not enabled for this game.");
        }

        EnsurePostWasNotAlreadyMade(
            seat.SeatId,
            PostType.Ante);

        if (amount != ante.Amount)
        {
            throw new InvalidOperationException(
                $"Ante must be exactly {ante.Amount}.");
        }

        if (ante.Type != AnteType.EveryPlayer)
        {
            throw new NotSupportedException(
                $"Ante type {ante.Type} is not supported yet.");
        }
    }

    private void ValidateSmallBlindPost(
        Seat seat,
        long amount)
    {
        if (seat.SeatId != 0)
        {
            throw new InvalidOperationException(
                "The small blind must be posted by seat 0.");
        }

        EnsurePostWasNotAlreadyMade(
            seat.SeatId,
            PostType.SmallBlind);

        if (amount != _rules.SmallBlind)
        {
            throw new InvalidOperationException(
                $"Small blind must be exactly {_rules.SmallBlind}.");
        }
    }

    private void ValidateBigBlindPost(
        Seat seat,
        long amount)
    {
        if (seat.SeatId != 1)
        {
            throw new InvalidOperationException(
                "The big blind must be posted by seat 1.");
        }

        EnsurePostWasNotAlreadyMade(
            seat.SeatId,
            PostType.BigBlind);

        if (amount != _rules.BigBlind)
        {
            throw new InvalidOperationException(
                $"Big blind must be exactly {_rules.BigBlind}.");
        }
    }

    private void ValidateStraddlePost(
        Seat seat,
        long amount)
    {
        if (_seats.Count == 2)
        {
            throw new InvalidOperationException(
                "Straddles are not supported in heads-up games.");
        }

        StraddleRules? straddle = _rules.Straddle;

        if (straddle is null ||
            straddle.Amounts.Count == 0)
        {
            throw new InvalidOperationException(
                "Straddle is not enabled for this game.");
        }

        EnsurePostWasNotAlreadyMade(
            seat.SeatId,
            PostType.Straddle);

        long highestLivePost = GetHighestRoundBet();

        if (amount <= highestLivePost)
        {
            throw new InvalidOperationException(
                $"Straddle must exceed the current live wager of {highestLivePost}.");
        }

        if (!straddle.Amounts.Contains(amount))
        {
            throw new InvalidOperationException(
                $"Straddle amount {amount} is not allowed.");
        }

        if (straddle.IsMandatory)
        {
            ValidateMandatoryStraddleSeat(
                seat.SeatId,
                straddle);
        }
    }

    private void ValidateMandatoryStraddleSeat(
        int seatId,
        StraddleRules straddle)
    {
        int postedStraddleCount = CountPosts(
            PostType.Straddle);

        int expectedSeatId = straddle.Type switch
        {
            StraddleType.Utg =>
                2 + postedStraddleCount,

            StraddleType.Button =>
                _seats.Count - 1,

            StraddleType.Mississippi =>
                throw new NotSupportedException(
                    "Automatic Mississippi straddle requires an explicit first seat."),

            StraddleType.AnyPosition =>
                throw new NotSupportedException(
                    "Automatic AnyPosition straddle requires an explicit seatId."),

            _ => throw new ArgumentOutOfRangeException(
                nameof(straddle.Type),
                straddle.Type,
                "Unknown straddle type.")
        };

        if (seatId != expectedSeatId)
        {
            throw new InvalidOperationException(
                $"The next mandatory straddle must be posted by seat {expectedSeatId}.");
        }
    }

    private void ValidateExtraBlindPost(
        Seat seat)
    {
        EnsurePostWasNotAlreadyMade(
            seat.SeatId,
            PostType.ExtraBlind);

        if (HasPost(seat.SeatId, PostType.SmallBlind) ||
            HasPost(seat.SeatId, PostType.BigBlind) ||
            HasPost(seat.SeatId, PostType.Straddle))
        {
            throw new InvalidOperationException(
                $"Seat {seat.SeatId} cannot post an extra blind after posting another live forced wager.");
        }
    }

    private void ValidateDeadBlindPost(
        Seat seat)
    {
        EnsurePostWasNotAlreadyMade(
            seat.SeatId,
            PostType.DeadBlind);
    }

    private bool HasPost(
        int seatId,
        PostType postType)
    {
        return _madePosts.Contains(
            (seatId, postType));
    }

    private int CountPosts(PostType postType)
    {
        int count = 0;

        foreach ((int _, PostType madePostType) in _madePosts)
        {
            if (madePostType == postType)
            {
                count++;
            }
        }

        return count;
    }

    private void EnsurePostWasNotAlreadyMade(
        int seatId,
        PostType postType)
    {
        if (HasPost(seatId, postType))
        {
            throw new InvalidOperationException(
                $"Seat {seatId} has already posted {postType}.");
        }
    }

    // Validation and helpers

    private RoundRules GetCurrentRound()
    {
        if (_roundIndex < 0 ||
            _roundIndex >= _rules.Rounds.Count)
        {
            throw new InvalidOperationException(
                "The current round is not defined.");
        }

        return _rules.Rounds[_roundIndex];
    }

    private bool IsFinalRound()
    {
        return _roundIndex ==
               _rules.Rounds.Count - 1;
    }

    private int GetHoleCardCount()
    {
        return _rules.GameType switch
        {
            GameType.TexasHoldem => 2,
            GameType.Omaha4c => 4,
            GameType.Omaha5c => 5,
            GameType.Omaha6c => 6,

            _ => throw new NotSupportedException(
                $"Game type {_rules.GameType} is not supported.")
        };
    }

    private Seat GetSeat(int seatId)
    {
        if (seatId < 0 ||
            seatId >= _seats.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(seatId),
                $"SeatId must be between 0 and {_seats.Count - 1}.");
        }

        return _seats[seatId];
    }

    private Seat GetActionSeat(int seatId)
    {
        Seat seat = GetSeat(seatId);

        if (seat.IsFolded)
        {
            throw new InvalidOperationException(
                $"Seat {seatId} has already folded.");
        }

        if (seat.IsAllIn)
        {
            throw new InvalidOperationException(
                $"Seat {seatId} is already all-in.");
        }

        return seat;
    }

    private void ValidateBoardIndex(int boardIndex)
    {
        if (boardIndex < 0 ||
            boardIndex >= _boards.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(boardIndex),
                $"BoardIndex must be between 0 and {_boards.Count - 1}.");
        }
    }

    private static void ValidateCards(
    IReadOnlyList<string> cards,
    int expectedCount)
    {
        ArgumentNullException.ThrowIfNull(cards);

        if (cards.Count != expectedCount)
        {
            throw new ArgumentException(
                $"Exactly {expectedCount} cards are required.",
                nameof(cards));
        }

        if (cards.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException(
                "The card list contains an empty value.",
                nameof(cards));
        }

        foreach (string card in cards)
        {
            if (!CardTable.IsValid(card))
            {
                throw new ArgumentException(
                    $"Invalid card: {card}.",
                    nameof(cards));
            }
        }

        if (cards
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count() != cards.Count)
        {
            throw new ArgumentException(
                "The card list contains duplicate cards.",
                nameof(cards));
        }
    }


    private void EnsureCardsWereNotUsed(
        IReadOnlyList<string> cards)
    {
        foreach (string card in cards)
        {
            if (DeckWasAlreadyUsed(card))
            {
                throw new InvalidOperationException(
                    $"Card {card} is already in use.");
            }
        }
    }

    private void ValidateRules(int seatCount)
    {
        if (_rules.InitialBoardCount <= 0)
        {
            throw new InvalidOperationException(
                "Initial board count must be greater than zero.");
        }

        if (_rules.MaxRunoutCount <= 0)
        {
            throw new InvalidOperationException(
                "Maximum runout count must be greater than zero.");
        }

        if (_rules.Rounds.Count == 0)
        {
            throw new InvalidOperationException(
                "No rounds are configured.");
        }

        if (_rules.Rounds
                .Select(round => round.Type)
                .Distinct()
                .Count() != _rules.Rounds.Count)
        {
            throw new InvalidOperationException(
                "Round types must be unique.");
        }

        ValidateAnteRules();
        ValidateStraddleRules(seatCount);
    }

    private void ValidateAnteRules()
    {
        AnteRules? ante = _rules.Ante;

        if (ante is null)
        {
            return;
        }

        if (ante.Amount < 0)
        {
            throw new InvalidOperationException(
                "Ante cannot be negative.");
        }
    }

    private void ValidateStraddleRules(int seatCount)
    {
        StraddleRules? straddle = _rules.Straddle;

        if (straddle is null ||
            straddle.Amounts.Count == 0)
        {
            return;
        }

        if (seatCount == 2)
        {
            throw new InvalidOperationException(
                "Straddles are not supported in heads-up games.");
        }

        if (straddle.Amounts.Any(amount => amount <= 0))
        {
            throw new InvalidOperationException(
                "Every straddle amount must be greater than zero.");
        }

        long previousAmount = _rules.BigBlind;

        for (int index = 0;
             index < straddle.Amounts.Count;
             index++)
        {
            long amount = straddle.Amounts[index];

            if (amount <= previousAmount)
            {
                throw new InvalidOperationException(
                    $"Straddle #{index + 1} must exceed the previous live wager of {previousAmount}.");
            }

            previousAmount = amount;
        }

        if (!straddle.IsMandatory)
        {
            return;
        }

        if (straddle.Type == StraddleType.Utg &&
            straddle.Amounts.Count > seatCount - 2)
        {
            throw new InvalidOperationException(
                "The number of mandatory UTG straddles exceeds the available seats after the big blind.");
        }

        if (straddle.Type == StraddleType.Button &&
            straddle.Amounts.Count != 1)
        {
            throw new InvalidOperationException(
                "Only one mandatory button straddle is supported.");
        }
    }

    private static bool IsLivePost(PostType postType)
    {
        return postType is
            PostType.SmallBlind or
            PostType.BigBlind or
            PostType.ExtraBlind or
            PostType.Straddle;
    }

    private bool IsAutomated(Automation automation)
    {
        return (_rules.Automation & automation) ==
               automation;
    }

    private void EnsureStarted()
    {
        if (_state != HandState.Started)
        {
            throw new InvalidOperationException(
                "The hand has not started.");
        }
    }

    private void EnsureState(HandState expectedState)
    {
        if (_state != expectedState)
        {
            throw new InvalidOperationException(
                $"Expected state {expectedState}, current state {_state}.");
        }
    }

    private void Emit(PokerHandEvent handEvent)
    {
        _events.Add(handEvent);
    }


    private static void ValidateShownCardsMatchDealtCards(
    Seat seat,
    IReadOnlyList<string> shownCards)
    {
        if (seat.HoleCards.Count != shownCards.Count)
        {
            throw new InvalidOperationException(
                $"The number of shown cards for seat {seat.SeatId} does not match the number of dealt cards.");
        }

        for (int index = 0;
             index < shownCards.Count;
             index++)
        {
            if (!string.Equals(
                    seat.HoleCards[index],
                    shownCards[index],
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Shown card {shownCards[index]} for seat {seat.SeatId} does not match the dealt card {seat.HoleCards[index]}.");
            }
        }
    }

    private bool DeckWasAlreadyUsed(string card)
    {
        return _seats.Any(
                   seat => seat.HoleCards.Any(
                       existing =>
                           string.Equals(
                               existing,
                               card,
                               StringComparison.OrdinalIgnoreCase))) ||
               _boards.Any(
                   board => board.Any(
                       existing =>
                           string.Equals(
                               existing,
                               card,
                               StringComparison.OrdinalIgnoreCase)));
    }

    private sealed record EvaluatedHand(
        long Strength,
        HandCategory Category,
        IReadOnlyList<string> BestCards);

}