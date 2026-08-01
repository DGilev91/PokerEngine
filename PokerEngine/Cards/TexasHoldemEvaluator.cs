using System.Numerics;
using PokerEngine.Enums;
using PokerEngine.Interfaces;
using PokerEngine.Models;

namespace PokerEngine.Cards;

public sealed class TexasHoldemEvaluator : IHandEvaluator
{
    public HandRank Evaluate(
        IReadOnlyList<string> holeCards,
        IReadOnlyList<string> boardCards)
    {
        ArgumentNullException.ThrowIfNull(holeCards);
        ArgumentNullException.ThrowIfNull(boardCards);

        if (holeCards.Count != 2)
        {
            throw new ArgumentException(
                "Texas Hold'em требует ровно две карманные карты.",
                nameof(holeCards));
        }

        if (boardCards.Count is < 3 or > 5)
        {
            throw new ArgumentException(
                "Доска должна содержать от трёх до пяти карт.",
                nameof(boardCards));
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

    private static HandRank EvaluateEncoded(
        ReadOnlySpan<int> cards)
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

        // Straight flush / Royal flush
        for (int suit = 0; suit < CardTable.SuitCount; suit++)
        {
            if (suitCounts[suit] < 5)
            {
                continue;
            }

            int straightHigh =
                FindStraightHigh(suitRankMasks[suit]);

            if (straightHigh == 0)
            {
                continue;
            }

            Span<int> bestCards = stackalloc int[5];

            SelectStraight(
                cards,
                straightHigh,
                suit,
                bestCards);

            HandCategory category =
                straightHigh == 14
                    ? HandCategory.RoyalFlush
                    : HandCategory.StraightFlush;

            return CreateResult(
                category,
                bestCards,
                straightHigh);
        }

        int fourRank = FindRankWithCount(
            rankCounts,
            requiredCount: 4);

        // Four of a kind
        if (fourRank != 0)
        {
            int kicker = FindHighestRank(
                rankCounts,
                excludedRank1: fourRank);

            Span<int> bestCards = stackalloc int[5];

            int index = SelectRankCards(
                cards,
                fourRank,
                bestCards,
                destinationIndex: 0,
                maximumCount: 4);

            SelectRankCards(
                cards,
                kicker,
                bestCards,
                index,
                maximumCount: 1);

            return CreateResult(
                HandCategory.FourCard,
                bestCards,
                fourRank,
                kicker);
        }

        int firstThreeRank = FindRankWithCount(
            rankCounts,
            requiredCount: 3);

        int secondPairRank = firstThreeRank == 0
            ? 0
            : FindRankWithMinimumCount(
                rankCounts,
                minimumCount: 2,
                excludedRank: firstThreeRank);

        // Full house
        if (firstThreeRank != 0 &&
            secondPairRank != 0)
        {
            Span<int> bestCards = stackalloc int[5];

            int index = SelectRankCards(
                cards,
                firstThreeRank,
                bestCards,
                destinationIndex: 0,
                maximumCount: 3);

            SelectRankCards(
                cards,
                secondPairRank,
                bestCards,
                index,
                maximumCount: 2);

            return CreateResult(
                HandCategory.FullHouse,
                bestCards,
                firstThreeRank,
                secondPairRank);
        }

        // Flush
        for (int suit = 0; suit < CardTable.SuitCount; suit++)
        {
            if (suitCounts[suit] < 5)
            {
                continue;
            }

            Span<int> bestCards = stackalloc int[5];
            Span<int> comparisonRanks = stackalloc int[5];

            SelectHighestCards(
                cards,
                bestCards,
                comparisonRanks,
                requiredCount: 5,
                requiredSuit: suit);

            return CreateResult(
                HandCategory.Flush,
                bestCards,
                comparisonRanks);
        }

        int straightRank = FindStraightHigh(rankMask);

        // Straight
        if (straightRank != 0)
        {
            Span<int> bestCards = stackalloc int[5];

            SelectStraight(
                cards,
                straightRank,
                requiredSuit: -1,
                bestCards);

            return CreateResult(
                HandCategory.Straight,
                bestCards,
                straightRank);
        }

        // Three of a kind
        if (firstThreeRank != 0)
        {
            Span<int> bestCards = stackalloc int[5];
            Span<int> kickers = stackalloc int[2];

            int index = SelectRankCards(
                cards,
                firstThreeRank,
                bestCards,
                destinationIndex: 0,
                maximumCount: 3);

            FindHighestRanks(
                rankCounts,
                kickers,
                firstThreeRank);

            index = SelectRankCards(
                cards,
                kickers[0],
                bestCards,
                index,
                maximumCount: 1);

            SelectRankCards(
                cards,
                kickers[1],
                bestCards,
                index,
                maximumCount: 1);

            return CreateResult(
                HandCategory.ThreeCard,
                bestCards,
                firstThreeRank,
                kickers[0],
                kickers[1]);
        }

        int firstPairRank = FindRankWithCount(
            rankCounts,
            requiredCount: 2);

        int secondPairRankForTwoPair =
            firstPairRank == 0
                ? 0
                : FindRankWithCount(
                    rankCounts,
                    requiredCount: 2,
                    excludedRank: firstPairRank);

        // Two pair
        if (secondPairRankForTwoPair != 0)
        {
            int kicker = FindHighestRank(
                rankCounts,
                firstPairRank,
                secondPairRankForTwoPair);

            Span<int> bestCards = stackalloc int[5];

            int index = SelectRankCards(
                cards,
                firstPairRank,
                bestCards,
                destinationIndex: 0,
                maximumCount: 2);

            index = SelectRankCards(
                cards,
                secondPairRankForTwoPair,
                bestCards,
                index,
                maximumCount: 2);

            SelectRankCards(
                cards,
                kicker,
                bestCards,
                index,
                maximumCount: 1);

            return CreateResult(
                HandCategory.TwoPair,
                bestCards,
                firstPairRank,
                secondPairRankForTwoPair,
                kicker);
        }

        // One pair
        if (firstPairRank != 0)
        {
            Span<int> bestCards = stackalloc int[5];
            Span<int> kickers = stackalloc int[3];

            int index = SelectRankCards(
                cards,
                firstPairRank,
                bestCards,
                destinationIndex: 0,
                maximumCount: 2);

            FindHighestRanks(
                rankCounts,
                kickers,
                firstPairRank);

            for (int i = 0; i < kickers.Length; i++)
            {
                index = SelectRankCards(
                    cards,
                    kickers[i],
                    bestCards,
                    index,
                    maximumCount: 1);
            }

            return CreateResult(
                HandCategory.OnePair,
                bestCards,
                firstPairRank,
                kickers[0],
                kickers[1],
                kickers[2]);
        }

        // High card
        {
            Span<int> bestCards = stackalloc int[5];
            Span<int> comparisonRanks = stackalloc int[5];

            SelectHighestCards(
                cards,
                bestCards,
                comparisonRanks,
                requiredCount: 5,
                requiredSuit: -1);

            return CreateResult(
                HandCategory.HighCard,
                bestCards,
                comparisonRanks);
        }
    }

    private static int FindStraightHigh(int rankMask)
    {
        // Обычные стриты: A-high до 6-high.
        for (int highRank = 14; highRank >= 6; highRank--)
        {
            int requiredMask = 0b1_1111 << (highRank - 4);

            if ((rankMask & requiredMask) == requiredMask)
            {
                return highRank;
            }
        }

        // Колесо: A-2-3-4-5.
        const int wheelMask =
            (1 << 14) |
            (1 << 5) |
            (1 << 4) |
            (1 << 3) |
            (1 << 2);

        return (rankMask & wheelMask) == wheelMask
            ? 5
            : 0;
    }

    private static int FindRankWithCount(
        ReadOnlySpan<byte> rankCounts,
        int requiredCount,
        int excludedRank = 0)
    {
        for (int rank = 14; rank >= 2; rank--)
        {
            if (rank != excludedRank &&
                rankCounts[rank] == requiredCount)
            {
                return rank;
            }
        }

        return 0;
    }

    private static int FindRankWithMinimumCount(
        ReadOnlySpan<byte> rankCounts,
        int minimumCount,
        int excludedRank = 0)
    {
        for (int rank = 14; rank >= 2; rank--)
        {
            if (rank != excludedRank &&
                rankCounts[rank] >= minimumCount)
            {
                return rank;
            }
        }

        return 0;
    }

    private static int FindHighestRank(
        ReadOnlySpan<byte> rankCounts,
        int excludedRank1 = 0,
        int excludedRank2 = 0)
    {
        for (int rank = 14; rank >= 2; rank--)
        {
            if (rankCounts[rank] != 0 &&
                rank != excludedRank1 &&
                rank != excludedRank2)
            {
                return rank;
            }
        }

        throw new InvalidOperationException(
            "Не удалось найти старшую карту.");
    }

    private static void FindHighestRanks(
        ReadOnlySpan<byte> rankCounts,
        Span<int> destination,
        int excludedRank = 0)
    {
        int index = 0;

        for (int rank = 14;
             rank >= 2 && index < destination.Length;
             rank--)
        {
            if (rank == excludedRank ||
                rankCounts[rank] == 0)
            {
                continue;
            }

            destination[index++] = rank;
        }

        if (index != destination.Length)
        {
            throw new InvalidOperationException(
                "Недостаточно рангов для определения кикеров.");
        }
    }

    private static void SelectStraight(
        ReadOnlySpan<int> cards,
        int straightHigh,
        int requiredSuit,
        Span<int> destination)
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

        for (int rank = straightHigh;
             rank >= straightHigh - 4;
             rank--)
        {
            SelectCard(
                cards,
                rank,
                requiredSuit,
                destination,
                ref destinationIndex);
        }
    }

    private static void SelectCard(
        ReadOnlySpan<int> cards,
        int requiredRank,
        int requiredSuit,
        Span<int> destination,
        ref int destinationIndex)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            int card = cards[i];
            int rank = CardTable.GetRank(card) + 2;

            if (rank != requiredRank)
            {
                continue;
            }

            if (requiredSuit >= 0 &&
                CardTable.GetSuit(card) != requiredSuit)
            {
                continue;
            }

            destination[destinationIndex++] = card;
            return;
        }

        throw new InvalidOperationException(
            "Не удалось найти карту стрита.");
    }

    private static int SelectRankCards(
        ReadOnlySpan<int> cards,
        int requiredRank,
        Span<int> destination,
        int destinationIndex,
        int maximumCount)
    {
        int selected = 0;

        for (int i = 0;
             i < cards.Length && selected < maximumCount;
             i++)
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
            throw new InvalidOperationException(
                $"Не удалось выбрать {maximumCount} карт ранга {requiredRank}.");
        }

        return destinationIndex;
    }

    private static void SelectHighestCards(
        ReadOnlySpan<int> cards,
        Span<int> destinationCards,
        Span<int> destinationRanks,
        int requiredCount,
        int requiredSuit)
    {
        int destinationIndex = 0;

        for (int rank = 14;
             rank >= 2 && destinationIndex < requiredCount;
             rank--)
        {
            for (int i = 0;
                 i < cards.Length && destinationIndex < requiredCount;
                 i++)
            {
                int card = cards[i];

                if (CardTable.GetRank(card) + 2 != rank)
                {
                    continue;
                }

                if (requiredSuit >= 0 &&
                    CardTable.GetSuit(card) != requiredSuit)
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
            throw new InvalidOperationException(
                "Недостаточно карт для формирования комбинации.");
        }
    }

    private static HandRank CreateResult(
        HandCategory category,
        ReadOnlySpan<int> cards,
        params int[] comparisonRanks)
    {
        return CreateResult(
            category,
            cards,
            comparisonRanks.AsSpan());
    }

    private static HandRank CreateResult(
        HandCategory category,
        ReadOnlySpan<int> cards,
        ReadOnlySpan<int> comparisonRanks)
    {
        long strength = BuildStrength(
            category,
            comparisonRanks);

        var bestCards = new string[5];

        for (int i = 0; i < bestCards.Length; i++)
        {
            bestCards[i] = CardTable.Decode(cards[i]);
        }

        return new HandRank(
            category,
            strength,
            bestCards);
    }

    private static long BuildStrength(
        HandCategory category,
        ReadOnlySpan<int> comparisonRanks)
    {
        /*
         * Основание 15:
         *
         * [категория][ранг 1][ранг 2][ранг 3][ранг 4][ранг 5]
         *
         * Поэтому обычное сравнение long корректно сравнивает руки.
         */
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

    private static void ValidateUnique(
        ReadOnlySpan<int> cards)
    {
        ulong cardMask = 0;

        for (int i = 0; i < cards.Length; i++)
        {
            ulong bit = 1UL << cards[i];

            if ((cardMask & bit) != 0)
            {
                throw new ArgumentException(
                    $"Карта {CardTable.Decode(cards[i])} повторяется.");
            }

            cardMask |= bit;
        }

        if (BitOperations.PopCount(cardMask) != cards.Length)
        {
            throw new ArgumentException(
                "Набор содержит повторяющиеся карты.");
        }
    }
}