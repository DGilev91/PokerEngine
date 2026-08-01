namespace PokerEngine.Models;

public sealed class Seat
{
    private readonly List<string> _holeCards = [];

    /// <summary>
    /// Номер места за столом.
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Стек игрока в начале раздачи.
    /// </summary>
    public long InitialStack { get; }

    /// <summary>
    /// Текущий оставшийся стек игрока.
    /// </summary>
    public long Stack { get; internal set; }

    /// <summary>
    /// Общая сумма фишек, вложенных игроком в текущую раздачу.
    /// </summary>
    public long TotalBet { get; internal set; }

    /// <summary>
    /// Сумма, вложенная игроком на текущей улице.
    /// </summary>
    public long RoundBet { get; internal set; }

    /// <summary>
    /// Карманные карты игрока.
    /// </summary>
    public IReadOnlyList<string> HoleCards => _holeCards;

    /// <summary>
    /// Сбросил ли игрок карты.
    /// </summary>
    public bool IsFolded { get; internal set; }

    /// <summary>
    /// Находится ли игрок в олл-ине.
    /// </summary>
    public bool IsAllIn => Stack == 0 && !IsFolded;

    public Seat(int number, long stack)
    {
        if (number < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(number),
                "Номер места не может быть отрицательным.");
        }

        if (stack < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stack),
                "Стек не может быть отрицательным.");
        }

        Number = number;
        InitialStack = stack;
        Stack = stack;
    }

    internal void SetHoleCards(IEnumerable<string> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        _holeCards.Clear();
        _holeCards.AddRange(cards);
    }

    internal void ClearRoundBet()
    {
        RoundBet = 0;
    }
}