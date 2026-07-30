using PokerEngine.Enums;
using PokerEngine.Interfaces;
using PokerEngine.Models;
using PokerEngine.States;

namespace PokerEngine.Games;

public sealed class NoLimitTexasHoldem : PokerGame
{
    private readonly PokerGameDefinition _rules

    public NoLimitTexasHoldem(
        long smallBlind,
        long bigBlind,
        long ante = 0,
        IReadOnlyList<long>? straddles = null,
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
            BettingStructure = BettingStructure.NoLimit,

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

            HoleCardCount = 2,

            Ante = ante,
            SmallBlind = smallBlind,
            BigBlind = bigBlind,
            Straddles = straddles ?? [],

            BoardCount = boardCount,

            // В Hold'em можно использовать 0, 1 или 2 карманные карты.
            RequiredHoleCardsForHand = null,
            RequiredBoardCardsForHand = null
        };
    }

    public override IPokerState CreateState()
    {
        return new PokerState(_rules);
    }
}