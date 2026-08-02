using PokerEngine.Enums;
using PokerEngine.Rules;

namespace PokerEngine.Tests.States;

public sealed class NoLimitTexasHoldemConstructorValidationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_SmallBlindNotPositive_Throws(long smallBlind)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            NoLimitTexasHoldemTestFactory.CreateGame(
                smallBlind: smallBlind,
                bigBlind: 100));

        Assert.Equal("smallBlind", exception.ParamName);
    }

    [Theory]
    [InlineData(50, 50)]
    [InlineData(50, 40)]
    [InlineData(100, 50)]
    public void Constructor_BigBlindNotGreaterThanSmallBlind_Throws(
        long smallBlind,
        long bigBlind)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            NoLimitTexasHoldemTestFactory.CreateGame(
                smallBlind: smallBlind,
                bigBlind: bigBlind));

        Assert.Equal("bigBlind", exception.ParamName);
    }

    [Fact]
    public void Constructor_NegativeAnte_Throws()
    {
        AnteRules ante = NoLimitTexasHoldemTestFactory.CreateAnte(-1);

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            NoLimitTexasHoldemTestFactory.CreateGame(ante: ante));

        Assert.Equal("ante", exception.ParamName);
    }

    [Fact]
    public void Constructor_ZeroAnte_IsAllowed()
    {
        AnteRules ante = NoLimitTexasHoldemTestFactory.CreateAnte(0);

        var game = NoLimitTexasHoldemTestFactory.CreateGame(ante: ante);

        Assert.NotNull(game);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_InitialBoardCountNotPositive_Throws(int initialBoardCount)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            NoLimitTexasHoldemTestFactory.CreateGame(
                initialBoardCount: initialBoardCount));

        Assert.Equal("initialBoardCount", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_MaxRunoutCountNotPositive_Throws(int maxRunoutCount)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            NoLimitTexasHoldemTestFactory.CreateGame(
                maxRunoutCount: maxRunoutCount));

        Assert.Equal("maxRunoutCount", exception.ParamName);
    }

    [Fact]
    public void Constructor_NullAnte_IsAllowed()
    {
        var game = NoLimitTexasHoldemTestFactory.CreateGame(ante: null);

        Assert.NotNull(game);
    }

    [Fact]
    public void Constructor_NullStraddle_IsAllowed()
    {
        var game = NoLimitTexasHoldemTestFactory.CreateGame(straddle: null);

        Assert.NotNull(game);
    }

    [Fact]
    public void Constructor_ValidArguments_DoesNotThrow()
    {
        AnteRules ante = NoLimitTexasHoldemTestFactory.CreateAnte(25);

        StraddleRules straddle =
            NoLimitTexasHoldemTestFactory.CreateStraddles(
                true,
                StraddleType.Utg,
                200,
                400);

        var game = NoLimitTexasHoldemTestFactory.CreateGame(
            automation: Automation.All,
            smallBlind: 50,
            bigBlind: 100,
            ante: ante,
            straddle: straddle,
            initialBoardCount: 2,
            maxRunoutCount: 3);

        Assert.NotNull(game);
    }
}
