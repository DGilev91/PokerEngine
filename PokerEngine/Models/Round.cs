using PokerEngine.Enums;

namespace PokerEngine.Models;

public sealed class Round
{
    /// <summary>
    /// Тип раунда торговли: Preflop, Flop, Turn или River.
    /// Определяет текущий этап раздачи.
    /// </summary>
    public required RoundType Type { get; init; }

    /// <summary>
    /// Указывает, требуется ли сжечь одну карту
    /// перед раздачей карт на этом раунде.
    /// </summary>
    public bool BurnCard { get; init; }

    /// <summary>
    /// Количество общих карт, которые должны быть
    /// разданы на каждую доску в начале этого раунда.
    ///
    /// Для стандартного Hold'em:
    /// Preflop — 0, Flop — 3, Turn — 1, River — 1.
    /// </summary>
    public int BoardDealingCount { get; init; }

    /// <summary>
    /// Минимальный размер полной ставки или минимальный
    /// размер полного повышения на этом раунде.
    /// </summary>
    public required long MinBet { get; init; }

    /// <summary>
    /// Максимальное количество полных повышений,
    /// разрешённых в рамках этого раунда торговли.
    ///
    /// Значение null означает отсутствие фиксированного
    /// ограничения на количество повышений.
    /// </summary>
    public int? MaxRaises { get; init; }
}