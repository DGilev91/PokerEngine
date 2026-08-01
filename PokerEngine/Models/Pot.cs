namespace PokerEngine.Models;

public sealed class Pot
{
    private readonly HashSet<int> _contributorSeats = [];
    private readonly HashSet<int> _eligibleSeats = [];

    /// <summary>
    /// Индекс пота:
    /// 0 — main pot, 1 и далее — side pots.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Общая сумма пота.
    /// </summary>
    public long Amount { get; internal set; }

    /// <summary>
    /// Места игроков, чьи фишки вошли в этот пот.
    /// Включает игроков, которые впоследствии сделали fold.
    /// </summary>
    public IReadOnlySet<int> ContributorSeats => _contributorSeats;

    /// <summary>
    /// Места игроков, которые имеют право выиграть этот пот.
    /// Игроки после fold отсюда исключаются.
    /// </summary>
    public IReadOnlySet<int> EligibleSeats => _eligibleSeats;

    /// <summary>
    /// Main pot имеет индекс 0.
    /// </summary>
    public bool IsMain => Index == 0;

    public Pot(int index)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index),
                "Индекс пота не может быть отрицательным.");
        }

        Index = index;
    }

    internal void AddContribution(int seat, long amount)
    {
        if (seat < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seat));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Сумма должна быть больше нуля.");
        }

        Amount += amount;
        _contributorSeats.Add(seat);
        _eligibleSeats.Add(seat);
    }

    internal void RemoveEligibility(int seat)
    {
        _eligibleSeats.Remove(seat);
    }
}