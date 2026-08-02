namespace PokerEngine.Enums;

public enum GameLimit
{
    /// <summary>
    /// Players may bet or raise up to their entire available stack.
    /// </summary>
    NoLimit,

    /// <summary>
    /// The maximum bet or raise is limited by the current pot size.
    /// </summary>
    PotLimit,

    /// <summary>
    /// Bets and raises use predefined fixed amounts.
    /// </summary>
    FixedLimit
}