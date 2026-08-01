using PokerEngine.Cards;
using PokerEngine.Enums;
using PokerEngine.Hands;
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
        long ante = 0,
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

        if (ante < 0)
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
                    BurnCard = false,
                    BoardDealingCount = 0,
                    MinBet = bigBlind,
                    MaxRaises = null
                },
                new Round
                {
                    Type = RoundType.Flop,
                    BurnCard = true,
                    BoardDealingCount = 3,
                    MinBet = bigBlind,
                    MaxRaises = null
                },
                new Round
                {
                    Type = RoundType.Turn,
                    BurnCard = true,
                    BoardDealingCount = 1,
                    MinBet = bigBlind,
                    MaxRaises = null
                },
                new Round
                {
                    Type = RoundType.River,
                    BurnCard = true,
                    BoardDealingCount = 1,
                    MinBet = bigBlind,
                    MaxRaises = null
                }
            ],

            Ante = ante,
            SmallBlind = smallBlind,
            BigBlind = bigBlind,
            Straddle = straddle,

            BoardCount = boardCount,

            RequiredHoleCardsForHand = null,
            RequiredBoardCardsForHand = null
        };
    }

    public override IPokerHand CreateHand()
    {
        return new PokerHand(_rules);
    }
}