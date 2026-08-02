namespace PokerEngine.Enums;

public enum ActionType
{
    /// <summary>
    /// The player discards their hand and gives up any claim to the pot.
    /// </summary>
    Fold,

    /// <summary>
    /// The player passes the action without committing additional chips.
    /// This is only allowed when there is no outstanding amount to call.
    /// </summary>
    Check,

    /// <summary>
    /// The player matches the current highest wager.
    /// </summary>
    Call,

    /// <summary>
    /// The player makes the first wager on the current betting round.
    /// </summary>
    Bet,

    /// <summary>
    /// The player increases the current wager to the specified total amount.
    /// </summary>
    RaiseTo
}