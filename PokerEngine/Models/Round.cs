using PokerEngine.Enums;

namespace PokerEngine.Models;

public sealed class Round
{
    /// <summary>
    /// Тип улицы торговли.
    /// </summary>
    public required RoundType Type { get; init; }

    /// <summary>
    /// Количество карт, раздаваемых на каждую доску в начале улицы.
    /// </summary>
    public int BoardCardCount { get; init; }

    /// <summary>
    /// Размер ставки для этой улицы.
    ///
    /// В Fixed Limit определяет фиксированный размер bet и raise.
    /// В No Limit и Pot Limit обычно равен минимальному размеру bet.
    /// </summary>
    public required long BetSize { get; init; }

    /// <summary>
    /// Максимальное количество полных повышений на улице.
    /// Null означает отсутствие ограничения.
    /// Обычно используется только в Fixed Limit.
    /// </summary>
    public int? MaxRaises { get; init; }
}