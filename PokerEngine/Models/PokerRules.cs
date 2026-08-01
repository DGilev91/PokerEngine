using PokerEngine.Enums;

namespace PokerEngine.Models;

public sealed class PokerRules
{
    /// <summary>
    /// Определяет, какие части раздачи выполняются автоматически.
    /// Automation.None означает полностью ручной режим.
    /// Automation.All означает максимально автоматический режим.
    /// </summary>
    public required Automation Automation { get; init; } = Automation.None;

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
    public required IReadOnlyList<RoundRules> Rounds { get; init; }

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
    /// Размер анте для каждого игрока.
    /// Ноль означает отсутствие анте.
    /// </summary>
    public AnteRules? Ante { get; init; }


    /// <summary>
    /// Начальное количество независимых досок в раздаче.
    ///
    /// Обычно равно 1.
    /// Для double-board форматов, например double-board bomb pot,
    /// может быть равно 2.
    ///
    /// Это значение определяет количество досок,
    /// которые существуют с самого начала раздачи.
    /// </summary>
    public int InitialBoardCount { get; init; } = 1;

    /// <summary>
    /// Максимальное количество runout, разрешённых для одной доски.
    ///
    /// Значение 1 означает, что Run It Twice запрещён.
    /// Значение 2 разрешает Run It Twice.
    /// Значение 3 разрешает Run It Three Times.
    ///
    /// Фактическое количество runout выбирается отдельно
    /// для конкретной раздачи после all-in.
    /// </summary>
    public int MaxRunoutCount { get; init; } = 1;
}

public sealed class AnteRules
{
    public required AnteType Type { get; init; }

    public required long Amount { get; init; }
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

public sealed class RoundRules
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

