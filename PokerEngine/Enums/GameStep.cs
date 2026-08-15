namespace PokerEngine.Enums;

public enum GameStep
{
    Initial,
    DealHole,
    PlayerAction,
    ReturnUncalledBet,
    CollectBets,
    DealBoard,
    Showdown,
    EvaluateHands,
    AwardPots,
    Complete
}