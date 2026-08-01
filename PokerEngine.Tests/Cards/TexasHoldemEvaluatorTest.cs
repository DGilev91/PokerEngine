using PokerEngine.Cards;
using PokerEngine.Enums;
using PokerEngine.Interfaces;
using PokerEngine.Models;

namespace PokerEngine.Tests.Cards;

public sealed class TexasHoldemEvaluatorTest
{
    private readonly IHandEvaluator _evaluator = new TexasHoldemEvaluator();

    public static TheoryData<
        string[],
        string[],
        HandCategory,
        string[]> CombinationCases =>
        new()
        {
            // Royal flush: используются обе карманные карты.
            {
                ["As", "Ks"],
                ["Qs", "Js", "Ts", "2d", "3c"],
                HandCategory.RoyalFlush,
                ["As", "Ks", "Qs", "Js", "Ts"]
            },

            // Royal flush полностью лежит на доске.
            {
                ["2c", "3d"],
                ["Ah", "Kh", "Qh", "Jh", "Th"],
                HandCategory.RoyalFlush,
                ["Ah", "Kh", "Qh", "Jh", "Th"]
            },

            // Straight flush.
            {
                ["9s", "8s"],
                ["7s", "6s", "5s", "Ac", "Kd"],
                HandCategory.StraightFlush,
                ["9s", "8s", "7s", "6s", "5s"]
            },

            // Младший straight flush: A-2-3-4-5.
            {
                ["As", "2s"],
                ["3s", "4s", "5s", "Kh", "Qd"],
                HandCategory.StraightFlush,
                ["5s", "4s", "3s", "2s", "As"]
            },

            // Four of a kind с тузом-кикером.
            {
                ["Ks", "Kh"],
                ["Kc", "Kd", "As", "2h", "3c"],
                HandCategory.FourCard,
                ["Ks", "Kh", "Kc", "Kd", "As"]
            },

            // Каре находится на доске, лучший кикер берётся из руки.
            {
                ["As", "2s"],
                ["Kh", "Kd", "Kc", "Ks", "Qd"],
                HandCategory.FourCard,
                ["Kh", "Kd", "Kc", "Ks", "As"]
            },

            // Full house: три туза и две дамы.
            {
                ["As", "Ah"],
                ["Ac", "Qs", "Qh", "2d", "3c"],
                HandCategory.FullHouse,
                ["As", "Ah", "Ac", "Qs", "Qh"]
            },

            // Два трипса: старший используется как тройка,
            // младший — как пара.
            {
                ["As", "Ah"],
                ["Ac", "Ks", "Kh", "Kc", "2d"],
                HandCategory.FullHouse,
                ["As", "Ah", "Ac", "Ks", "Kh"]
            },

            // Из нескольких пар для full house выбирается старшая.
            {
                ["As", "Ah"],
                ["Ac", "Ks", "Kh", "Qs", "Qh"],
                HandCategory.FullHouse,
                ["As", "Ah", "Ac", "Ks", "Kh"]
            },

            // Flush: выбираются пять старших карт масти.
            {
                ["As", "8s"],
                ["Ks", "Js", "5s", "2s", "Qd"],
                HandCategory.Flush,
                ["As", "Ks", "Js", "8s", "5s"]
            },

            // При шести картах одной масти младшая отбрасывается.
            {
                ["As", "3s"],
                ["Ks", "Qs", "9s", "5s", "2d"],
                HandCategory.Flush,
                ["As", "Ks", "Qs", "9s", "5s"]
            },

            // Старший стрит.
            {
                ["As", "Kd"],
                ["Qh", "Jc", "Ts", "2d", "3c"],
                HandCategory.Straight,
                ["As", "Kd", "Qh", "Jc", "Ts"]
            },

            // Младший стрит: туз считается единицей.
            {
                ["As", "2d"],
                ["3h", "4c", "5s", "Kd", "Qc"],
                HandCategory.Straight,
                ["5s", "4c", "3h", "2d", "As"]
            },

            // Из шести последовательных карт выбирается старший стрит.
            {
                ["9s", "8d"],
                ["7h", "6c", "5s", "Td", "Ac"],
                HandCategory.Straight,
                ["Td", "9s", "8d", "7h", "6c"]
            },

            // Three of a kind с двумя старшими кикерами.
            {
                ["Qs", "Qh"],
                ["Qc", "As", "Kd", "5h", "2c"],
                HandCategory.ThreeCard,
                ["Qs", "Qh", "Qc", "As", "Kd"]
            },

            // Three of a kind на доске.
            {
                ["As", "Kd"],
                ["Qh", "Qc", "Qs", "5d", "2c"],
                HandCategory.ThreeCard,
                ["Qh", "Qc", "Qs", "As", "Kd"]
            },

            // Three pair: выбираются две старшие пары.
            {
                ["As", "Ah"],
                ["Ks", "Kh", "Qs", "Qh", "2d"],
                HandCategory.TwoPair,
                ["As", "Ah", "Ks", "Kh", "Qs"]
            },

            // Two pair с лучшим кикером.
            {
                ["As", "Ah"],
                ["Ks", "Kh", "Qd", "3c", "2s"],
                HandCategory.TwoPair,
                ["As", "Ah", "Ks", "Kh", "Qd"]
            },

            // Одна пара и три кикера.
            {
                ["Js", "Jh"],
                ["As", "Kd", "Qc", "5h", "2c"],
                HandCategory.OnePair,
                ["Js", "Jh", "As", "Kd", "Qc"]
            },

            // Пара находится на доске.
            {
                ["As", "Kd"],
                ["Qh", "Qc", "Js", "5d", "2c"],
                HandCategory.OnePair,
                ["Qh", "Qc", "As", "Kd", "Js"]
            },

            // High card: выбираются пять старших карт.
            {
                ["As", "Kd"],
                ["Qh", "9c", "7s", "4d", "2c"],
                HandCategory.HighCard,
                ["As", "Kd", "Qh", "9c", "7s"]
            }
        };

    [Theory]
    [MemberData(nameof(CombinationCases))]
    public void Evaluate_ReturnsExpectedCombination(
        string[] holeCards,
        string[] boardCards,
        HandCategory expectedCategory,
        string[] expectedCards)
    {
        HandRank result = _evaluator.Evaluate(
            holeCards,
            boardCards);

        Assert.Equal(expectedCategory, result.Category);
        Assert.Equal(expectedCards, result.Cards);
    }

    [Fact]
    public void Evaluate_WorksOnFlop()
    {
        HandRank result = _evaluator.Evaluate(
            ["As", "Ah"],
            ["Ac", "Ks", "Kd"]);

        Assert.Equal(
            HandCategory.FullHouse,
            result.Category);

        Assert.Equal(
            ["As", "Ah", "Ac", "Ks", "Kd"],
            result.Cards);
    }

    [Fact]
    public void Evaluate_WorksOnTurn()
    {
        HandRank result = _evaluator.Evaluate(
            ["As", "Ks"],
            ["Qs", "Js", "Ts", "2d"]);

        Assert.Equal(
            HandCategory.RoyalFlush,
            result.Category);
    }

    [Fact]
    public void Evaluate_StrongerCategoryWins()
    {
        HandRank flush = _evaluator.Evaluate(
            ["As", "8s"],
            ["Ks", "Js", "5s", "2s", "Qd"]);

        HandRank straight = _evaluator.Evaluate(
            ["9c", "8d"],
            ["7h", "6s", "5c", "Ad", "Kc"]);

        Assert.True(flush.CompareTo(straight) > 0);
    }

    [Fact]
    public void Evaluate_HigherPairWins()
    {
        HandRank aces = _evaluator.Evaluate(
            ["As", "Ah"],
            ["Kd", "Qc", "9s", "5h", "2c"]);

        HandRank kings = _evaluator.Evaluate(
            ["Ks", "Kh"],
            ["Ad", "Qc", "9s", "5h", "2c"]);

        Assert.True(aces.CompareTo(kings) > 0);
    }

    [Fact]
    public void Evaluate_PairKickerDecidesWinner()
    {
        HandRank aceKicker = _evaluator.Evaluate(
            ["Qs", "Qh"],
            ["As", "Kd", "9c", "5h", "2c"]);

        HandRank kingKicker = _evaluator.Evaluate(
            ["Qd", "Qc"],
            ["Ks", "Jh", "9c", "5h", "2c"]);

        Assert.True(
            aceKicker.CompareTo(kingKicker) > 0);
    }

    [Fact]
    public void Evaluate_TwoPairKickerDecidesWinner()
    {
        HandRank aceKicker = _evaluator.Evaluate(
            ["As", "3h"],
            ["Qs", "Qh", "Js", "Jh", "2c"]);

        HandRank kingKicker = _evaluator.Evaluate(
            ["Ks", "3d"],
            ["Qd", "Qc", "Jd", "Jc", "2s"]);

        Assert.True(
            aceKicker.CompareTo(kingKicker) > 0);
    }

    [Fact]
    public void Evaluate_FlushKickersDecideWinner()
    {
        HandRank aceHighFlush = _evaluator.Evaluate(
            ["As", "8s"],
            ["Ks", "Js", "5s", "2s", "Qd"]);

        HandRank kingHighFlush = _evaluator.Evaluate(
            ["Ks", "8h"],
            ["Qs", "Js", "5s", "2s", "Ad"]);

        Assert.True(
            aceHighFlush.CompareTo(kingHighFlush) > 0);
    }

    [Fact]
    public void Evaluate_EqualHandsAreTie()
    {
        HandRank first = _evaluator.Evaluate(
            ["2c", "3d"],
            ["As", "Ks", "Qs", "Js", "Ts"]);

        HandRank second = _evaluator.Evaluate(
            ["4c", "5d"],
            ["As", "Ks", "Qs", "Js", "Ts"]);

        Assert.Equal(0, first.CompareTo(second));
    }

    [Fact]
    public void Evaluate_ThrowsWhenHoleCardCountIsInvalid()
    {
        Assert.Throws<ArgumentException>(() =>
            _evaluator.Evaluate(
                ["As"],
                ["Ks", "Qs", "Js", "Ts", "2d"]));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(6)]
    public void Evaluate_ThrowsWhenBoardCardCountIsInvalid(
        int boardCardCount)
    {
        string[] board =
            CardTable.Cards
                .Take(boardCardCount)
                .ToArray();

        Assert.Throws<ArgumentException>(() =>
            _evaluator.Evaluate(
                ["As", "Ks"],
                board));
    }

    [Fact]
    public void Evaluate_ThrowsWhenCardIsRepeated()
    {
        Assert.Throws<ArgumentException>(() =>
            _evaluator.Evaluate(
                ["As", "Ks"],
                ["As", "Qs", "Js", "Ts", "2d"]));
    }

    [Theory]
    [InlineData("1s")]
    [InlineData("Ax")]
    [InlineData("AAA")]
    [InlineData("")]
    public void Evaluate_ThrowsWhenCardIsInvalid(
        string invalidCard)
    {
        Assert.ThrowsAny<ArgumentException>(() =>
            _evaluator.Evaluate(
                [invalidCard, "Ks"],
                ["Qs", "Js", "Ts", "2d", "3c"]));
    }
}