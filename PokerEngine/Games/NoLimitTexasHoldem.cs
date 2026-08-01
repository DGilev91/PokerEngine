using PokerEngine.Enums;
using PokerEngine.States;
using PokerEngine.Interfaces;
using PokerEngine.Models;

namespace PokerEngine.Games;

public sealed class NoLimitTexasHoldem : PokerGame
{
    private readonly PokerRules _rules;

    public NoLimitTexasHoldem(
        Automation automation,
        long smallBlind,
        long bigBlind,
        AnteRules ante,
        StraddleRules? straddle,
        int initialBoardCount,
        int maxRunoutCount)
    {
        if (smallBlind <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(smallBlind),
                "Small blind должен быть больше нуля.");
        }

        if (bigBlind <= smallBlind)
        {
            throw new ArgumentOutOfRangeException(
                nameof(bigBlind),
                "Big blind должен быть больше small blind.");
        }

        if (ante != null && ante.Amount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(ante),
                "Ante не может быть отрицательным.");
        }

        if (initialBoardCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(initialBoardCount),
                "Количество досок должно быть больше нуля.");
        }

        if (maxRunoutCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxRunoutCount),
                "Максимальное количество runout должно быть больше нуля.");
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
            MaxRunoutCount = maxRunoutCount,
        };
    }

    public override IPokerState CreateState()
    {
        return new PokerState(_rules);
    }
}