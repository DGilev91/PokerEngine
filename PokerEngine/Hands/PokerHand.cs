using PokerEngine.Interfaces;
using PokerEngine.Models;

namespace PokerEngine.Hands;

internal sealed class PokerHand : IPokerHand
{
    private readonly PokerRules _rules;
    private int _roundIndex;

    public IPokerState State { get; }

    public IDeck Deck { get; }

    public PokerHand(
        PokerRules rules,
        IPokerState state,
        IDeck deck)
    {
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(deck);

        _rules = rules;
        State = state;
        Deck = deck;
    }

    public void Initialize(IReadOnlyList<long> stacks)
    {
        ArgumentNullException.ThrowIfNull(stacks);

        _roundIndex = 0;
        State.Initialize(stacks);
    }

    public void PostAnte(int seat, long amount)
    {
        State.PostAnte(seat, amount);
    }

    public void PostSmallBlind(int seat, long amount)
    {
        State.PostSmallBlind(seat, amount);
    }

    public void PostBigBlind(int seat, long amount)
    {
        State.PostBigBlind(seat, amount);
    }

    public void PostStraddle(int seat, long amount)
    {
        State.PostStraddle(seat, amount);
    }

    public IReadOnlyList<string> DealHole(int seat)
    {
        const int holeCardCount = 2;

        IReadOnlyList<string> cards = Deck.Deal(holeCardCount);

        State.DealHole(seat, cards);

        return cards;
    }

    public IReadOnlyList<string> DealBoard(int board = 0)
    {
        if (board < 0 || board >= _rules.BoardCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(board),
                $"Номер доски должен быть от 0 до {_rules.BoardCount - 1}.");
        }

        Round round = GetNextBoardRound();

        if (round.BurnCard)
        {
            Deck.Deal();
        }

        IReadOnlyList<string> cards =
            Deck.Deal(round.BoardDealingCount);

        State.DealBoard(cards, board);

        _roundIndex++;

        return cards;
    }

    public void Fold(int seat)
    {
        State.Fold(seat);
    }

    public void Check(int seat)
    {
        State.Check(seat);
    }

    public void Call(int seat)
    {
        State.Call(seat);
    }

    public void Bet(int seat, long amount)
    {
        State.Bet(seat, amount);
    }

    public void RaiseTo(int seat, long amount)
    {
        State.RaiseTo(seat, amount);
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
}