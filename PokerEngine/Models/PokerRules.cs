using PokerEngine.Enums;

namespace PokerEngine.Models;

internal sealed class PokerRules
{
    /// <summary>
    /// Структура ставок: Fixed Limit, Pot Limit или No Limit.
    /// </summary>
    public required BettingStructure BettingStructure { get; init; }

    /// <summary>
    /// Последовательность улиц игры:
    /// preflop, flop, turn, river и т. д.
    /// </summary>
    public required IReadOnlyList<Round> Rounds { get; init; }

    /// <summary>
    /// Размеры обязательных ставок по порядку:
    /// small blind, big blind и дополнительные straddles.
    /// </summary>
    public required IReadOnlyList<long> BlindsOrStraddles { get; init; }

    /// <summary>
    /// Размер анте для каждого игрока.
    /// Ноль означает отсутствие анте.
    /// </summary>
    public long Ante { get; init; }

    /// <summary>
    /// Размер малого блайнда.
    /// Должен быть больше 0 и меньше размера большого блайнда.
    /// </summary>
    public required long SmallBlind { get; init; }
    
    /// <summary>
    /// Размер большого блайнда.
    /// Обычно также определяет минимальный размер полной ставки
    /// для игр с одинаковым минимальным бетом на всех улицах.
    /// </summary>
    public required long BigBlind { get; init; }
    
    /// <summary>
    /// Последовательность дополнительных обязательных страддлов.
    /// Значения указываются в порядке их выставления после большого блайнда.
    /// Пустой список означает, что страддлы не используются.
    /// </summary>
    /// <example>
    /// <code>
    /// Straddles = [200, 400];
    /// </code>
    /// Означает первый страддл 200 и второй страддл 400.
    /// </example>
    public IReadOnlyList<long> Straddles { get; init; } = [];


    /// <summary>
    /// Начальное количество досок.
    /// Обычно одна, для double board — две.
    /// </summary>
    public int BoardCount { get; init; } = 1;

    /// <summary>
    /// Количество карманных карт, которые необходимо использовать
    /// при формировании итоговой комбинации.
    /// Null означает, что строгого ограничения нет.
    /// </summary>
    public int? RequiredHoleCardsForHand { get; init; }

    /// <summary>
    /// Количество общих карт, которые необходимо использовать
    /// при формировании итоговой комбинации.
    /// Null означает, что строгого ограничения нет.
    /// </summary>
    public int? RequiredBoardCardsForHand { get; init; }
}