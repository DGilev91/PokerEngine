namespace PokerEngine.Enums;

public enum StraddleType
{
    /// <summary>
    /// Классический страддл от UTG —
    /// игрока сразу после большого блайнда.
    /// </summary>
    Utg,

    /// <summary>
    /// Mississippi straddle.
    /// Страддл может поставить игрок в любой разрешённой позиции,
    /// часто включая баттон.
    /// </summary>
    Mississippi,

    /// <summary>
    /// Страддл только с баттона.
    /// </summary>
    Button,

    /// <summary>
    /// Страддл с конкретного места,
    /// которое передаётся движку при выставлении.
    /// </summary>
    AnyPosition
}