using PokerEngine.Enums;

namespace PokerEngine.Models;

public sealed class PokerRules
{
    /// <summary>
    /// Тип игры: Texas Hold'em, Omaha, и т. д.
    /// </summary>
    public required GameType GameType { get; init; }

    /// <summary>
    /// Структура ставок: Fixed Limit, Pot Limit или No Limit.
    /// </summary>
    public required BettingType BettingType { get; init; }

    /// <summary>
    /// Последовательность улиц игры:
    /// preflop, flop, turn, river и т. д.
    /// </summary>
    public required IReadOnlyList<Round> Rounds { get; init; }

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
    /// Правила использования страддлов.
    /// Null означает, что страддлы запрещены.
    /// </summary>
    public StraddleRules? Straddle { get; init; }


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

public sealed class StraddleRules
{
    /// <summary>
    /// Режим определения позиции первого игрока,
    /// который может поставить страддл.
    /// </summary>
    public required StraddleType Type { get; init; }

    /// <summary>
    /// Размеры последовательных страддлов.
    /// Первый элемент — первый страддл,
    /// второй — restraddle и так далее.
    /// Пустой список означает отсутствие страддлов.
    /// </summary>
    public required IReadOnlyList<long> Amounts { get; init; }

    /// <summary>
    /// Является ли первый страддл обязательным.
    /// </summary>
    public bool IsMandatory { get; init; }
}

