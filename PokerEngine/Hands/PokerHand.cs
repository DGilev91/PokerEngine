using PokerEngine.Enums;
using PokerEngine.Interfaces;
using PokerEngine.Models;

namespace PokerEngine.Hands;

internal sealed class PokerHand : IPokerHand
{
    private readonly PokerRules _rules;
    private readonly IDeck _deck;

    private readonly List<Seat> _seats = [];
    private readonly List<List<string>> _boards = [];
    private readonly List<Pot> _pots = [];
    private readonly List<string> _burnedCards = [];

    private int _roundIndex;
    private bool _initialized;

    public IReadOnlyList<Seat> Seats => _seats;

    public IReadOnlyList<Pot> Pots => _pots;

    public IReadOnlyList<string> BurnedCards => _burnedCards;

    public IReadOnlyList<IReadOnlyList<string>> Boards =>
        _boards
            .Select(board => (IReadOnlyList<string>)board)
            .ToArray();

    public int RemainingDeckCards => _deck.RemainingCount;

    public PokerHand(
        PokerRules rules,
        IDeck deck)
    {
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(deck);

        _rules = rules;
        _deck = deck;
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

        if (stacks.Any(stack => stack < 0))
        {
            throw new ArgumentException(
                "Стек игрока не может быть отрицательным.",
                nameof(stacks));
        }

        _seats.Clear();

        for (int seat = 0; seat < stacks.Count; seat++)
        {
            _seats.Add(new Seat(
                number: seat,
                stack: stacks[seat]));
        }

        _boards.Clear();

        for (int board = 0; board < _rules.BoardCount; board++)
        {
            _boards.Add([]);
        }

        _pots.Clear();
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
                PostAnte(seat.Number, _rules.Ante);
            }
        }
    }

    public void PostAnte(int seat, long amount)
    {
        EnsureInitialized();

        CommitChips(
            GetSeat(seat),
            amount,
            totalAmount: false);

        RebuildPots();
    }

    public void PostSmallBlind(int seat, long amount)
    {
        EnsureInitialized();

        if (amount != _rules.SmallBlind)
        {
            throw new InvalidOperationException(
                $"Small blind должен быть равен {_rules.SmallBlind}.");
        }

        CommitChips(
            GetSeat(seat),
            amount,
            totalAmount: false);

        RebuildPots();
    }

    public void PostBigBlind(int seat, long amount)
    {
        EnsureInitialized();

        if (amount != _rules.BigBlind)
        {
            throw new InvalidOperationException(
                $"Big blind должен быть равен {_rules.BigBlind}.");
        }

        CommitChips(
            GetSeat(seat),
            amount,
            totalAmount: false);

        RebuildPots();
    }

    public void PostStraddle(int seat, long amount)
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
            GetSeat(seat),
            amount,
            totalAmount: false);

        RebuildPots();
    }

    public IReadOnlyList<string> DealHole(
        int seat,
        IReadOnlyList<string>? cards = null)
    {
        EnsureInitialized();

        Seat player = GetSeat(seat);

        if (player.HoleCards.Count > 0)
        {
            throw new InvalidOperationException(
                $"Игроку на месте {seat} уже выданы карты.");
        }

        const int holeCardCount = 2;

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

        player.SetHoleCards(dealtCards);

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
        int board = 0, IReadOnlyList<string>? cards = null)
    {
        EnsureInitialized();
        ValidateBoard(board);

        Round round = GetNextBoardRound();

        IReadOnlyList<string> dealtCards;

        if (IsAutomated(Automation.DealBoard))
        {
            if (cards is not null)
            {
                throw new InvalidOperationException(
                    "Карты доски раздаются автоматически.");
            }

            if (round.BurnCard)
            {
                BurnCard();
            }

            dealtCards = _deck.Deal(round.BoardDealingCount);
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

    public void Fold(int seat)
    {
        EnsureInitialized();

        Seat player = GetSeat(seat);

        if (player.IsFolded)
        {
            throw new InvalidOperationException(
                $"Игрок на месте {seat} уже сделал fold.");
        }

        player.IsFolded = true;

        foreach (Pot pot in _pots)
        {
            pot.RemoveEligibility(seat);
        }
    }

    public void Check(int seat)
    {
        EnsureInitialized();

        Seat player = GetActiveSeat(seat);

        long highestBet = GetHighestRoundBet();

        if (player.RoundBet != highestBet)
        {
            throw new InvalidOperationException(
                $"Игрок на месте {seat} не может сделать check. " +
                $"Текущая ставка игрока: {player.RoundBet}, максимальная: {highestBet}.");
        }
    }

    public void Call(int seat)
    {
        EnsureInitialized();

        Seat player = GetActiveSeat(seat);

        long highestBet = GetHighestRoundBet();
        long amountToCall = highestBet - player.RoundBet;

        if (amountToCall <= 0)
        {
            throw new InvalidOperationException(
                $"Игроку на месте {seat} нечего коллировать.");
        }

        CommitChips(
            player,
            highestBet,
            totalAmount: true);

        RebuildPots();
    }

    public void Bet(int seat, long amount)
    {
        EnsureInitialized();

        Seat player = GetActiveSeat(seat);

        if (GetHighestRoundBet() > 0)
        {
            throw new InvalidOperationException(
                "Нельзя сделать bet, когда на улице уже есть ставка. Используйте RaiseTo.");
        }

        if (amount < GetCurrentRound().MinBet && amount < player.Stack)
        {
            throw new InvalidOperationException(
                $"Минимальная ставка равна {GetCurrentRound().MinBet}.");
        }

        CommitChips(
            player,
            amount,
            totalAmount: true);

        RebuildPots();
    }

    public void RaiseTo(int seat, long amount)
    {
        EnsureInitialized();

        Seat player = GetActiveSeat(seat);

        long highestBet = GetHighestRoundBet();

        if (highestBet == 0)
        {
            throw new InvalidOperationException(
                "Нельзя сделать raise, когда ставки ещё нет. Используйте Bet.");
        }

        if (amount <= highestBet && amount < player.RoundBet + player.Stack)
        {
            throw new InvalidOperationException(
                $"RaiseTo должен быть больше текущей максимальной ставки {highestBet}.");
        }

        CommitChips(
            player,
            amount,
            totalAmount: true);

        RebuildPots();
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
                $"Игрок на месте {seat.Number} уже сделал fold.");
        }

        long contribution = totalAmount
            ? amount - seat.RoundBet
            : amount;

        if (contribution < 0)
        {
            throw new InvalidOperationException(
                "Новая ставка не может быть меньше текущей ставки игрока.");
        }

        if (contribution == 0)
        {
            return;
        }

        long paid = Math.Min(contribution, seat.Stack);

        seat.Stack -= paid;
        seat.RoundBet += paid;
        seat.TotalBet += paid;
    }

    private void RebuildPots()
    {
        _pots.Clear();

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

            if (contributors.Length < 2)
            {
                break;
            }

            long contributionPerSeat = level - previousLevel;

            if (contributionPerSeat <= 0)
            {
                continue;
            }

            var pot = new Pot(_pots.Count);

            foreach (Seat seat in contributors)
            {
                pot.AddContribution(
                    seat.Number,
                    contributionPerSeat);

                if (seat.IsFolded)
                {
                    pot.RemoveEligibility(seat.Number);
                }
            }

            _pots.Add(pot);
            previousLevel = level;
        }
    }

    private Seat GetSeat(int seat)
    {
        if (seat < 0 || seat >= _seats.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(seat),
                $"Номер места должен быть от 0 до {_seats.Count - 1}.");
        }

        return _seats[seat];
    }

    private Seat GetActiveSeat(int seat)
    {
        Seat player = GetSeat(seat);

        if (player.IsFolded)
        {
            throw new InvalidOperationException(
                $"Игрок на месте {seat} уже сделал fold.");
        }

        if (player.IsAllIn)
        {
            throw new InvalidOperationException(
                $"Игрок на месте {seat} уже находится в all-in.");
        }

        return player;
    }

    private void ValidateBoard(int board)
    {
        if (board < 0 || board >= _boards.Count)
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
        if (_roundIndex < 0 || _roundIndex >= _rules.Rounds.Count)
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

    private bool IsAutomated(Automation automation)
    {
        return (_rules.Automation & automation) == automation;
    }

    private void EnsureInitialized()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException(
                "Раздача не была инициализирована.");
        }
    }
}