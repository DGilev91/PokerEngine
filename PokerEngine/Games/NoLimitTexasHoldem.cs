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
        AnteRules? ante = null,
        StraddleRules? straddle = null,
        int boardCount = 1)
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

        if (boardCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(boardCount),
                "Количество досок должно быть больше нуля.");
        }

        _rules = new PokerRules
        {
            Automation = automation,
            GameType = GameType.TexasHoldem,
            BettingType = BettingType.NoLimit,

            Rounds =
            [
                new Round
                {
                    Type = RoundType.Preflop,
                    BoardCardCount = 0,
                    BetSize = bigBlind,
                    MaxRaises = null
                },
                new Round
                {
                    Type = RoundType.Flop,
                    BoardCardCount = 3,
                    BetSize = bigBlind,
                    MaxRaises = null
                },
                new Round
                {
                    Type = RoundType.Turn,
                    BoardCardCount = 1,
                    BetSize = bigBlind,
                    MaxRaises = null
                },
                new Round
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

            BoardCount = boardCount,
        };
    }

    public override IPokerState CreateState()
    {
        return new PokerState(_rules);
    }
}