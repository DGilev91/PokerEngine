using PokerEngine.Enums;
using PokerEngine.Models;
using PokerEngine.States;

namespace PokerEngine.Tests;

internal static class PokerStateTestFactory
{
    public static PokerState CreateState(int maxRunoutCount = 1, Automation automation = Automation.None)
    {
        return new PokerState(CreateClassicRules(maxRunoutCount, automation));
    }

    public static PokerRules CreateClassicRules(int maxRunoutCount = 1, Automation automation = Automation.None)
    {
        return new PokerRules
        {
            Automation = automation,
            GameType = GameType.TexasHoldem,
            BettingType = BettingType.NoLimit,
            SmallBlind = 50,
            BigBlind = 100,
            Ante = null,
            Straddle = null,
            InitialBoardCount = 1,
            MaxRunoutCount = maxRunoutCount,
            Rounds =
            [
                new RoundRules
                {
                    Type = RoundType.Preflop,
                    BoardCardCount = 0,
                    BetSize = 100,
                    MaxRaises = null
                },
                new RoundRules
                {
                    Type = RoundType.Flop,
                    BoardCardCount = 3,
                    BetSize = 100,
                    MaxRaises = null
                },
                new RoundRules
                {
                    Type = RoundType.Turn,
                    BoardCardCount = 1,
                    BetSize = 100,
                    MaxRaises = null
                },
                new RoundRules
                {
                    Type = RoundType.River,
                    BoardCardCount = 1,
                    BetSize = 100,
                    MaxRaises = null
                }
            ]
        };
    }

    public static void CheckAroundThreePlayers(PokerState state)
    {
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(2, ActionType.Check);
    }

    public static void CheckAroundHeadsUp(PokerState state)
    {
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(0, ActionType.Check);
    }
}