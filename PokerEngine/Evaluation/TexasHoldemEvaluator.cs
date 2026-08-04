using System.Numerics;
using PokerEngine.Cards;
using PokerEngine.Enums;

namespace PokerEngine.Evaluation;

public sealed class TexasHoldemEvaluator : IHandEvaluator
{
    public HandRank Evaluate(IReadOnlyList<string> holeCards, IReadOnlyList<string> boardCards)
    {
        ArgumentNullException.ThrowIfNull(holeCards);
        ArgumentNullException.ThrowIfNull(boardCards);

        if (holeCards.Count != 2)
        {
            throw new ArgumentException("Texas Hold'em requires exactly two hole cards.", nameof(holeCards));
        }

        if (boardCards.Count is < 3 or > 5)
        {
            throw new ArgumentException("The board must contain between three and five cards.", nameof(boardCards));
        }

        int cardCount = holeCards.Count + boardCards.Count;
        Span<int> cards = stackalloc int[7];

        cards[0] = CardTable.Encode(holeCards[0]);
        cards[1] = CardTable.Encode(holeCards[1]);

        for (int i = 0; i < boardCards.Count; i++)
        {
            cards[i + 2] = CardTable.Encode(boardCards[i]);
        }

        cards = cards[..cardCount];

        ValidateUnique(cards);

        return EvaluateEncoded(cards);
    }

    private static HandRank EvaluateEncoded(ReadOnlySpan<int> cards)
    {
        Span<byte> rankCounts = stackalloc byte[15];
        Span<byte> suitCounts = stackalloc byte[4];
        Span<int> suitRankMasks = stackalloc int[4];

        int rankMask = 0;

        for (int i = 0; i < cards.Length; i++)
        {
            int encodedCard = cards[i];
            int rank = CardTable.GetRank(encodedCard) + 2;
            int suit = CardTable.GetSuit(encodedCard);

            rankCounts[rank]++;
            suitCounts[suit]++;

            int rankBit = 1 << rank;

            rankMask |= rankBit;
            suitRankMasks[suit] |= rankBit;
        }

        for (int suit = 0; suit < CardTable.SuitCount; suit++)
        {
            if (suitCounts[suit] < 5)
            {
                continue;
            }

            int straightHigh = FindStraightHigh(suitRankMasks[suit]);

            if (straightHigh == 0)
            {
                continue;
            }

            Span<int> bestCards = stackalloc int[5];

            SelectStraight(cards, straightHigh, suit, bestCards);

            HandCategory category = straightHigh == 14 ? HandCategory.RoyalFlush : HandCategory.StraightFlush;

            return CreateResult(category, bestCards, straightHigh);
        }

        int fourRank = FindRankWithCount(rankCounts, 4);

        if (fourRank != 0)
        {
            int kicker = FindHighestRank(rankCounts, fourRank);

            Span<int> bestCards = stackalloc int[5];

            int index = SelectRankCards(cards, fourRank, bestCards, 0, 4);

            SelectRankCards(cards, kicker, bestCards, index, 1);

            return CreateResult(HandCategory.FourCard, bestCards, fourRank, kicker);
        }

        int firstThreeRank = FindRankWithCount(rankCounts, 3);
        int secondPairRank = firstThreeRank == 0 ? 0 : FindRankWithMinimumCount(rankCounts, 2, firstThreeRank);

        if (firstThreeRank != 0 && secondPairRank != 0)
        {
            Span<int> bestCards = stackalloc int[5];

            int index = SelectRankCards(cards, firstThreeRank, bestCards, 0, 3);

            SelectRankCards(cards, secondPairRank, bestCards, index, 2);

            return CreateResult(HandCategory.FullHouse, bestCards, firstThreeRank, secondPairRank);
        }

        for (int suit = 0; suit < CardTable.SuitCount; suit++)
        {
            if (suitCounts[suit] < 5)
            {
                continue;
            }

            Span<int> bestCards = stackalloc int[5];
            Span<int> comparisonRanks = stackalloc int[5];

            SelectHighestCards(cards, bestCards, comparisonRanks, 5, suit);

            return CreateResult(HandCategory.Flush, bestCards, comparisonRanks);
        }

        int straightRank = FindStraightHigh(rankMask);

        if (straightRank != 0)
        {
            Span<int> bestCards = stackalloc int[5];

            SelectStraight(cards, straightRank, -1, bestCards);

            return CreateResult(HandCategory.Straight, bestCards, straightRank);
        }

        if (firstThreeRank != 0)
        {
            Span<int> bestCards = stackalloc int[5];
            Span<int> kickers = stackalloc int[2];

            int index = SelectRankCards(cards, firstThreeRank, bestCards, 0, 3);

            FindHighestRanks(rankCounts, kickers, firstThreeRank);

            index = SelectRankCards(cards, kickers[0], bestCards, index, 1);

            SelectRankCards(cards, kickers[1], bestCards, index, 1);

            return CreateResult(HandCategory.ThreeCard, bestCards, firstThreeRank, kickers[0], kickers[1]);
        }

        int firstPairRank = FindRankWithCount(rankCounts, 2);
        int secondPairRankForTwoPair = firstPairRank == 0 ? 0 : FindRankWithCount(rankCounts, 2, firstPairRank);

        if (secondPairRankForTwoPair != 0)
        {
            int kicker = FindHighestRank(rankCounts, firstPairRank, secondPairRankForTwoPair);

            Span<int> bestCards = stackalloc int[5];

            int index = SelectRankCards(cards, firstPairRank, bestCards, 0, 2);

            index = SelectRankCards(cards, secondPairRankForTwoPair, bestCards, index, 2);

            SelectRankCards(cards, kicker, bestCards, index, 1);

            return CreateResult(HandCategory.TwoPair, bestCards, firstPairRank, secondPairRankForTwoPair, kicker);
        }

        if (firstPairRank != 0)
        {
            Span<int> bestCards = stackalloc int[5];
            Span<int> kickers = stackalloc int[3];

            int index = SelectRankCards(cards, firstPairRank, bestCards, 0, 2);

            FindHighestRanks(rankCounts, kickers, firstPairRank);

            for (int i = 0; i < kickers.Length; i++)
            {
                index = SelectRankCards(cards, kickers[i], bestCards, index, 1);
            }

            return CreateResult(HandCategory.OnePair, bestCards, firstPairRank, kickers[0], kickers[1], kickers[2]);
        }

        Span<int> highCards = stackalloc int[5];
        Span<int> highRanks = stackalloc int[5];

        SelectHighestCards(cards, highCards, highRanks, 5, -1);

        return CreateResult(HandCategory.HighCard, highCards, highRanks);
    }

    private static int FindStraightHigh(int rankMask)
    {
        for (int highRank = 14; highRank >= 6; highRank--)
        {
            int requiredMask = 0b1_1111 << (highRank - 4);

            if ((rankMask & requiredMask) == requiredMask)
            {
                return highRank;
            }
        }

        const int wheelMask = (1 << 14) | (1 << 5) | (1 << 4) | (1 << 3) | (1 << 2);

        return (rankMask & wheelMask) == wheelMask ? 5 : 0;
    }

    private static int FindRankWithCount(ReadOnlySpan<byte> rankCounts, int requiredCount, int excludedRank = 0)
    {
        for (int rank = 14; rank >= 2; rank--)
        {
            if (rank != excludedRank && rankCounts[rank] == requiredCount)
            {
                return rank;
            }
        }

        return 0;
    }

    private static int FindRankWithMinimumCount(ReadOnlySpan<byte> rankCounts, int minimumCount, int excludedRank = 0)
    {
        for (int rank = 14; rank >= 2; rank--)
        {
            if (rank != excludedRank && rankCounts[rank] >= minimumCount)
            {
                return rank;
            }
        }

        return 0;
    }

    private static int FindHighestRank(ReadOnlySpan<byte> rankCounts, int excludedRank1 = 0, int excludedRank2 = 0)
    {
        for (int rank = 14; rank >= 2; rank--)
        {
            if (rankCounts[rank] != 0 && rank != excludedRank1 && rank != excludedRank2)
            {
                return rank;
            }
        }

        throw new InvalidOperationException("Unable to find the highest card.");
    }

    private static void FindHighestRanks(ReadOnlySpan<byte> rankCounts, Span<int> destination, int excludedRank = 0)
    {
        int index = 0;

        for (int rank = 14; rank >= 2 && index < destination.Length; rank--)
        {
            if (rank == excludedRank || rankCounts[rank] == 0)
            {
                continue;
            }

            destination[index++] = rank;
        }

        if (index != destination.Length)
        {
            throw new InvalidOperationException("Not enough ranks to determine the kickers.");
        }
    }

    private static void SelectStraight(ReadOnlySpan<int> cards, int straightHigh, int requiredSuit, Span<int> destination)
    {
        int destinationIndex = 0;

        if (straightHigh == 5)
        {
            SelectCard(cards, 5, requiredSuit, destination, ref destinationIndex);
            SelectCard(cards, 4, requiredSuit, destination, ref destinationIndex);
            SelectCard(cards, 3, requiredSuit, destination, ref destinationIndex);
            SelectCard(cards, 2, requiredSuit, destination, ref destinationIndex);
            SelectCard(cards, 14, requiredSuit, destination, ref destinationIndex);

            return;
        }

        for (int rank = straightHigh; rank >= straightHigh - 4; rank--)
        {
            SelectCard(cards, rank, requiredSuit, destination, ref destinationIndex);
        }
    }

    private static void SelectCard(ReadOnlySpan<int> cards, int requiredRank, int requiredSuit, Span<int> destination, ref int destinationIndex)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            int card = cards[i];
            int rank = CardTable.GetRank(card) + 2;

            if (rank != requiredRank)
            {
                continue;
            }

            if (requiredSuit >= 0 && CardTable.GetSuit(card) != requiredSuit)
            {
                continue;
            }

            destination[destinationIndex++] = card;
            return;
        }

        throw new InvalidOperationException("Unable to find a card for the straight.");
    }

    private static int SelectRankCards(ReadOnlySpan<int> cards, int requiredRank, Span<int> destination, int destinationIndex, int maximumCount)
    {
        int selected = 0;

        for (int i = 0; i < cards.Length && selected < maximumCount; i++)
        {
            int card = cards[i];
            int rank = CardTable.GetRank(card) + 2;

            if (rank != requiredRank)
            {
                continue;
            }

            destination[destinationIndex++] = card;
            selected++;
        }

        if (selected != maximumCount)
        {
            throw new InvalidOperationException($"Unable to select {maximumCount} cards of rank {requiredRank}.");
        }

        return destinationIndex;
    }

    private static void SelectHighestCards(ReadOnlySpan<int> cards, Span<int> destinationCards, Span<int> destinationRanks, int requiredCount, int requiredSuit)
    {
        int destinationIndex = 0;

        for (int rank = 14; rank >= 2 && destinationIndex < requiredCount; rank--)
        {
            for (int i = 0; i < cards.Length && destinationIndex < requiredCount; i++)
            {
                int card = cards[i];

                if (CardTable.GetRank(card) + 2 != rank)
                {
                    continue;
                }

                if (requiredSuit >= 0 && CardTable.GetSuit(card) != requiredSuit)
                {
                    continue;
                }

                destinationCards[destinationIndex] = card;
                destinationRanks[destinationIndex] = rank;
                destinationIndex++;
            }
        }

        if (destinationIndex != requiredCount)
        {
            throw new InvalidOperationException("Not enough cards to build the hand.");
        }
    }

    private static HandRank CreateResult(HandCategory category, ReadOnlySpan<int> cards, params int[] comparisonRanks)
    {
        return CreateResult(category, cards, comparisonRanks.AsSpan());
    }

    private static HandRank CreateResult(HandCategory category, ReadOnlySpan<int> cards, ReadOnlySpan<int> comparisonRanks)
    {
        long strength = BuildStrength(category, comparisonRanks);
        var bestCards = new string[5];

        for (int i = 0; i < bestCards.Length; i++)
        {
            bestCards[i] = CardTable.Decode(cards[i]);
        }

        return new HandRank(category, strength, bestCards);
    }

    private static long BuildStrength(HandCategory category, ReadOnlySpan<int> comparisonRanks)
    {
        long strength = (int)category;

        for (int i = 0; i < 5; i++)
        {
            strength *= 15;

            if (i < comparisonRanks.Length)
            {
                strength += comparisonRanks[i];
            }
        }

        return strength;
    }

    private static void ValidateUnique(ReadOnlySpan<int> cards)
    {
        ulong cardMask = 0;

        for (int i = 0; i < cards.Length; i++)
        {
            ulong bit = 1UL << cards[i];

            if ((cardMask & bit) != 0)
            {
                throw new ArgumentException($"Card {CardTable.Decode(cards[i])} is duplicated.");
            }

            cardMask |= bit;
        }

        if (BitOperations.PopCount(cardMask) != cards.Length)
        {
            throw new ArgumentException("The card set contains duplicate cards.");
        }
    }
}