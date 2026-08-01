using PokerEngine.Cards;
using PokerEngine.Enums;
using PokerEngine.Interfaces;
using PokerEngine.Models;

namespace PokerEngine.Hands;

internal sealed class PokerHand : IPokerHand
{

    private readonly PokerRules _rules;
    private readonly IDeck _deck = new Deck();

    private readonly List<PokerHandEvent> _events = [];
    private readonly List<Seat> _seats = [];
    private readonly List<List<string>> _boards = [];
    private readonly List<string> _burnedCards = [];

    private int _roundIndex;
    private bool _initialized;

    public IReadOnlyList<PokerHandEvent> Events => _events;

    public IReadOnlyList<Seat> Seats => _seats;

    public PotState PotState { get; } = new();

    public IReadOnlyList<string> BurnedCards => _burnedCards;

    public IReadOnlyList<IReadOnlyList<string>> Boards =>
        _boards
            .Select(board => (IReadOnlyList<string>)board)
            .ToArray();

    public int RemainingDeckCards => _deck.RemainingCount;

    public PokerHand(PokerRules rules)
    {
        ArgumentNullException.ThrowIfNull(rules);

        _rules = rules;
    }

    public void Initialize(IReadOnlyList<long> stacks)
    {
        ArgumentNullException.ThrowIfNull(stacks);

        if (stacks.Count < 2)
        {
            throw new ArgumentException(
                "Для раздачи необходимо минимум два игрока.",
                nameof(stacks));
        }

        if (stacks.Any(stack => stack <= 0))
        {
            throw new ArgumentException(
                "Стек игрока не может быть отрицательным или нулевым.",
                nameof(stacks));
        }

        _seats.Clear();

        for (int seatId = 0; seatId < stacks.Count; seatId++)
        {
            _seats.Add(new Seat(
                seatId: seatId,
                stack: stacks[seatId]));
        }

        _boards.Clear();

        for (int board = 0; board < _rules.BoardCount; board++)
        {
            _boards.Add([]);
        }

        PotState.Clear();
        _burnedCards.Clear();

        _roundIndex = 0;
        _initialized = true;

        if (IsAutomated(Automation.ShuffleDeck))
        {
            _deck.Shuffle();
        }

        if (IsAutomated(Automation.PostAntes) && _rules.Ante > 0)
        {
            foreach (Seat seat in _seats)
            {
                PostAnte(
                    seat.SeatId,
                    _rules.Ante);
            }
        }
    }

    public void PostAnte(
        int seatId,
        long amount)
    {
        EnsureInitialized();

        CommitChips(
            GetSeat(seatId),
            amount,
            totalAmount: false);

        RecalculatePots();
    }

    public void PostSmallBlind(
        int seatId,
        long amount)
    {
        EnsureInitialized();

        if (amount != _rules.SmallBlind)
        {
            throw new InvalidOperationException(
                $"Small blind должен быть равен {_rules.SmallBlind}.");
        }

        CommitChips(
            GetSeat(seatId),
            amount,
            totalAmount: false);

        RecalculatePots();
    }

    public void PostBigBlind(
        int seatId,
        long amount)
    {
        EnsureInitialized();

        if (amount != _rules.BigBlind)
        {
            throw new InvalidOperationException(
                $"Big blind должен быть равен {_rules.BigBlind}.");
        }

        CommitChips(
            GetSeat(seatId),
            amount,
            totalAmount: false);

        RecalculatePots();
    }

    public void PostStraddle(
        int seatId,
        long amount)
    {
        EnsureInitialized();

        if (_rules.Straddle is null)
        {
            throw new InvalidOperationException(
                "Страддлы в этой игре запрещены.");
        }

        if (!_rules.Straddle.Amounts.Contains(amount))
        {
            throw new InvalidOperationException(
                $"Размер страддла {amount} не разрешён.");
        }

        CommitChips(
            GetSeat(seatId),
            amount,
            totalAmount: false);

        RecalculatePots();
    }

    public IReadOnlyList<string> DealHole(
        int seatId,
        IReadOnlyList<string>? cards = null)
    {
        EnsureInitialized();

        Seat seat = GetSeat(seatId);

        if (seat.HoleCards.Count > 0)
        {
            throw new InvalidOperationException(
                $"Игроку на месте {seatId} уже выданы карты.");
        }

        int holeCardCount = 0;
        switch (_rules.GameType)
        { 
            case GameType.TexasHoldem:
                holeCardCount = 2;
                break;
            case GameType.Omaha4c:
                holeCardCount = 4;
                break;
            case GameType.Omaha5c:
                holeCardCount = 5;
                break;
            case GameType.Omaha6c:
                holeCardCount = 6;
                break;
            default:
                throw new InvalidOperationException(
                    $"Неизвестный тип игры: {_rules.GameType}.");
        }

        IReadOnlyList<string> dealtCards;

        if (IsAutomated(Automation.DealHoleCards))
        {
            if (cards is not null)
            {
                throw new InvalidOperationException(
                    "Карты раздаются автоматически.");
            }

            dealtCards = _deck.Deal(holeCardCount);
        }
        else
        {
            if (cards is null)
            {
                throw new InvalidOperationException(
                    "В ручном режиме необходимо указать карманные карты.");
            }

            ValidateCardCount(
                cards,
                holeCardCount,
                "карманных карт");

            _deck.Take(cards);
            dealtCards = cards.ToArray();
        }

        seat.SetHoleCards(dealtCards);

        return dealtCards;
    }

    public string BurnCard(string? card = null)
    {
        EnsureInitialized();

        string burnedCard;

        if (IsAutomated(Automation.BurnCards))
        {
            if (card is not null)
            {
                throw new InvalidOperationException(
                    "Burn-карта выбирается автоматически.");
            }

            burnedCard = _deck.Deal();
        }
        else
        {
            if (string.IsNullOrWhiteSpace(card))
            {
                throw new InvalidOperationException(
                    "В ручном режиме необходимо указать burn-карту.");
            }

            _deck.Take(card);
            burnedCard = card;
        }

        _burnedCards.Add(burnedCard);

        return burnedCard;
    }

    public IReadOnlyList<string> DealBoard(
        int board = 0,
        IReadOnlyList<string>? cards = null)
    {
        EnsureInitialized();
        ValidateBoard(board);

        Round round = GetNextBoardRound();

        if (round.BurnCard &&
            IsAutomated(Automation.BurnCards))
        {
            BurnCard();
        }

        IReadOnlyList<string> dealtCards;

        if (IsAutomated(Automation.DealBoard))
        {
            if (cards is not null)
            {
                throw new InvalidOperationException(
                    "Карты доски раздаются автоматически.");
            }

            dealtCards = _deck.Deal(
                round.BoardDealingCount);
        }
        else
        {
            if (cards is null)
            {
                throw new InvalidOperationException(
                    "В ручном режиме необходимо указать карты доски.");
            }

            ValidateCardCount(
                cards,
                round.BoardDealingCount,
                $"карт для улицы {round.Type}");

            _deck.Take(cards);
            dealtCards = cards.ToArray();
        }

        _boards[board].AddRange(dealtCards);
        _roundIndex++;

        return dealtCards;
    }

    public void Fold(int seatId)
    {
        EnsureInitialized();

        Seat seat = GetSeat(seatId);

        if (seat.IsFolded)
        {
            throw new InvalidOperationException(
                $"Игрок на месте {seatId} уже сделал fold.");
        }

        seat.IsFolded = true;

        foreach (Pot pot in PotState.Pots)
        {
            pot.RemoveEligibility(seatId);
        }
    }

    public void Check(int seatId)
    {
        EnsureInitialized();

        Seat seat = GetActiveSeat(seatId);

        long highestBet = GetHighestRoundBet();

        if (seat.RoundBet != highestBet)
        {
            throw new InvalidOperationException(
                $"Игрок на месте {seatId} не может сделать check. " +
                $"Текущая ставка игрока: {seat.RoundBet}, " +
                $"максимальная: {highestBet}.");
        }
    }

    public void Call(int seatId)
    {
        EnsureInitialized();

        Seat seat = GetActiveSeat(seatId);

        long highestBet = GetHighestRoundBet();
        long amountToCall = highestBet - seat.RoundBet;

        if (amountToCall <= 0)
        {
            throw new InvalidOperationException(
                $"Игроку на месте {seatId} нечего коллировать.");
        }

        CommitChips(
            seat,
            highestBet,
            totalAmount: true);

        RecalculatePots();
    }

    public void Bet(
        int seatId,
        long amount)
    {
        EnsureInitialized();

        Seat seat = GetActiveSeat(seatId);

        if (GetHighestRoundBet() > 0)
        {
            throw new InvalidOperationException(
                "Нельзя сделать bet, когда на улице уже есть ставка. " +
                "Используйте RaiseTo.");
        }

        Round round = GetCurrentRound();

        if (amount < round.MinBet &&
            amount < seat.Stack)
        {
            throw new InvalidOperationException(
                $"Минимальная ставка равна {round.MinBet}.");
        }

        CommitChips(
            seat,
            amount,
            totalAmount: true);

        RecalculatePots();
    }

    public void RaiseTo(
        int seatId,
        long amount)
    {
        EnsureInitialized();

        Seat seat = GetActiveSeat(seatId);

        long highestBet = GetHighestRoundBet();

        if (highestBet == 0)
        {
            throw new InvalidOperationException(
                "Нельзя сделать raise, когда ставки ещё нет. " +
                "Используйте Bet.");
        }

        long maximumTotalBet =
            seat.RoundBet + seat.Stack;

        if (amount <= highestBet &&
            amount < maximumTotalBet)
        {
            throw new InvalidOperationException(
                $"RaiseTo должен быть больше текущей " +
                $"максимальной ставки {highestBet}.");
        }

        CommitChips(
            seat,
            amount,
            totalAmount: true);

        RecalculatePots();
    }

    private void CommitChips(
        Seat seat,
        long amount,
        bool totalAmount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Сумма не может быть отрицательной.");
        }

        if (seat.IsFolded)
        {
            throw new InvalidOperationException(
                $"Игрок на месте {seat.SeatId} уже сделал fold.");
        }

        long contribution = totalAmount
            ? amount - seat.RoundBet
            : amount;

        if (contribution < 0)
        {
            throw new InvalidOperationException(
                "Новая ставка не может быть меньше " +
                "текущей ставки игрока.");
        }

        if (contribution == 0)
        {
            return;
        }

        long paid = Math.Min(
            contribution,
            seat.Stack);

        seat.Stack -= paid;
        seat.RoundBet += paid;
        seat.TotalBet += paid;
    }

    private void RecalculatePots()
    {
        PotState.Clear();

        long[] levels = _seats
            .Select(seat => seat.TotalBet)
            .Where(amount => amount > 0)
            .Distinct()
            .Order()
            .ToArray();

        long previousLevel = 0;

        foreach (long level in levels)
        {
            Seat[] contributors = _seats
                .Where(seat => seat.TotalBet >= level)
                .ToArray();

            long contributionPerSeat =
                level - previousLevel;

            if (contributionPerSeat <= 0)
            {
                continue;
            }

            if (contributors.Length == 1)
            {
                Seat owner = contributors[0];

                PotState.SetUncalledBet(
                    seatId: owner.SeatId,
                    amount: contributionPerSeat);

                break;
            }

            var pot = new Pot(
                index: PotState.Pots.Count);

            foreach (Seat seat in contributors)
            {
                pot.AddContribution(
                    seatId: seat.SeatId,
                    amount: contributionPerSeat);

                if (seat.IsFolded)
                {
                    pot.RemoveEligibility(
                        seat.SeatId);
                }
            }

            PotState.AddPot(pot);
            previousLevel = level;
        }
    }

    private Seat GetSeat(int seatId)
    {
        if (seatId < 0 ||
            seatId >= _seats.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(seatId),
                $"SeatId должен быть от 0 до {_seats.Count - 1}.");
        }

        return _seats[seatId];
    }

    private Seat GetActiveSeat(int seatId)
    {
        Seat seat = GetSeat(seatId);

        if (seat.IsFolded)
        {
            throw new InvalidOperationException(
                $"Игрок на месте {seatId} уже сделал fold.");
        }

        if (seat.IsAllIn)
        {
            throw new InvalidOperationException(
                $"Игрок на месте {seatId} уже находится в all-in.");
        }

        return seat;
    }

    private void ValidateBoard(int board)
    {
        if (board < 0 ||
            board >= _boards.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(board),
                $"Номер доски должен быть от 0 до {_boards.Count - 1}.");
        }
    }

    private static void ValidateCardCount(
        IReadOnlyList<string> cards,
        int expectedCount,
        string description)
    {
        ArgumentNullException.ThrowIfNull(cards);

        if (cards.Count != expectedCount)
        {
            throw new ArgumentException(
                $"Необходимо передать {expectedCount} {description}.",
                nameof(cards));
        }

        if (cards.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException(
                "Список содержит пустую карту.",
                nameof(cards));
        }

        if (cards.Count != cards.Distinct().Count())
        {
            throw new ArgumentException(
                "Список содержит повторяющиеся карты.",
                nameof(cards));
        }
    }

    private Round GetNextBoardRound()
    {
        while (
            _roundIndex < _rules.Rounds.Count &&
            _rules.Rounds[_roundIndex].BoardDealingCount == 0)
        {
            _roundIndex++;
        }

        if (_roundIndex >= _rules.Rounds.Count)
        {
            throw new InvalidOperationException(
                "Все карты доски уже были выданы.");
        }

        return _rules.Rounds[_roundIndex];
    }

    private Round GetCurrentRound()
    {
        if (_roundIndex < 0 ||
            _roundIndex >= _rules.Rounds.Count)
        {
            throw new InvalidOperationException(
                "Текущая улица не определена.");
        }

        return _rules.Rounds[_roundIndex];
    }

    private long GetHighestRoundBet()
    {
        return _seats.Count == 0
            ? 0
            : _seats.Max(seat => seat.RoundBet);
    }

    private bool IsAutomated(
        Automation automation)
    {
        return (_rules.Automation & automation) ==
               automation;
    }

    private void EnsureInitialized()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException(
                "Раздача не была инициализирована.");
        }
    }

    private void Emit(PokerHandEvent handEvent)
    {
        ArgumentNullException.ThrowIfNull(handEvent);
        _events.Add(handEvent);
    }
}