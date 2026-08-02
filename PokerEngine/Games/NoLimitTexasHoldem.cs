using PokerEngine.Enums;
using PokerEngine.Interfaces;
using PokerEngine.Models;
using PokerEngine.States;

namespace PokerEngine.Games;

public sealed class NoLimitTexasHoldem : PokerGame
{
    private readonly PokerRules _rules;

    public NoLimitTexasHoldem(
        Automation automation,
        long smallBlind,
        long bigBlind,
        AnteRules? ante,
        StraddleRules? straddle,
        int initialBoardCount,
        int maxRunoutCount)
    {
        if (smallBlind <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(smallBlind),
                smallBlind,
                "Small blind must be greater than zero.");
        }

        if (bigBlind <= smallBlind)
        {
            throw new ArgumentOutOfRangeException(
                nameof(bigBlind),
                bigBlind,
                "Big blind must be greater than the small blind.");
        }

        if (ante is not null && ante.Amount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(ante),
                ante.Amount,
                "Ante amount cannot be negative.");
        }

        if (initialBoardCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(initialBoardCount),
                initialBoardCount,
                "Initial board count must be greater than zero.");
        }

        if (maxRunoutCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxRunoutCount),
                maxRunoutCount,
                "Maximum runout count must be greater than zero.");
        }

        _rules = new PokerRules
        {
            Automation = automation,
            GameType = GameType.TexasHoldem,
            GameLimit = GameLimit.NoLimit,

            Rounds =
            [
                new RoundRules
                {
                    Type = RoundType.Preflop,
                    BoardCardCount = 0,
                    BetSize = bigBlind,
                    MaxRaises = null
                },
                new RoundRules
                {
                    Type = RoundType.Flop,
                    BoardCardCount = 3,
                    BetSize = bigBlind,
                    MaxRaises = null
                },
                new RoundRules
                {
                    Type = RoundType.Turn,
                    BoardCardCount = 1,
                    BetSize = bigBlind,
                    MaxRaises = null
                },
                new RoundRules
                {
                    Type = RoundType.River,
                    BoardCardCount = 1,
                    BetSize = bigBlind,
                    MaxRaises = null
                }
            ],

            Ante = ante,
            SmallBlind = smallBlind,
            BigBlind = bigBlind,
            Straddle = straddle,

            InitialBoardCount = initialBoardCount,
            MaxRunoutCount = maxRunoutCount
        };
    }

    public override IPokerState CreateState()
    {
        return new PokerState(_rules);
    }
}