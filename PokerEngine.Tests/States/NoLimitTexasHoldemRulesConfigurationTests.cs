using PokerEngine.Enums;
using PokerEngine.Rules;

namespace PokerEngine.Tests.States;

public sealed class NoLimitTexasHoldemRulesConfigurationTests
{
    [Fact]
    public void Constructor_ConfiguresTexasHoldemNoLimitRules()
    {
        var game = NoLimitTexasHoldemTestFactory.CreateGame();
        PokerRules rules = NoLimitTexasHoldemTestFactory.GetRules(game);

        Assert.Equal(GameType.TexasHoldem, rules.GameType);
        Assert.Equal(GameLimit.NoLimit, rules.GameLimit);
    }

    [Fact]
    public void Constructor_PreservesAutomationFlags()
    {
        Automation automation =
            Automation.ShuffleDeck |
            Automation.PostAntes |
            Automation.PostBlinds |
            Automation.PostStraddles |
            Automation.DealHoleCards |
            Automation.DealBoard |
            Automation.BurnCards;

        var game = NoLimitTexasHoldemTestFactory.CreateGame(
            automation: automation);

        PokerRules rules = NoLimitTexasHoldemTestFactory.GetRules(game);

        Assert.Equal(automation, rules.Automation);
        Assert.Equal(Automation.All, rules.Automation);
    }

    [Fact]
    public void Constructor_PreservesBlinds()
    {
        var game = NoLimitTexasHoldemTestFactory.CreateGame(
            smallBlind: 250,
            bigBlind: 500);

        PokerRules rules = NoLimitTexasHoldemTestFactory.GetRules(game);

        Assert.Equal(250, rules.SmallBlind);
        Assert.Equal(500, rules.BigBlind);
    }

    [Fact]
    public void Constructor_PreservesAnteReference()
    {
        AnteRules ante = NoLimitTexasHoldemTestFactory.CreateAnte(
            50,
            AnteType.EveryPlayer);

        var game = NoLimitTexasHoldemTestFactory.CreateGame(
            ante: ante);

        PokerRules rules = NoLimitTexasHoldemTestFactory.GetRules(game);

        Assert.Same(ante, rules.Ante);
        Assert.Equal(AnteType.EveryPlayer, rules.Ante!.Type);
        Assert.Equal(50, rules.Ante.Amount);
    }

    [Fact]
    public void Constructor_PreservesStraddleReference()
    {
        StraddleRules straddle =
            NoLimitTexasHoldemTestFactory.CreateStraddles(
                true,
                StraddleType.Utg,
                200,
                400);

        var game = NoLimitTexasHoldemTestFactory.CreateGame(
            straddle: straddle);

        PokerRules rules = NoLimitTexasHoldemTestFactory.GetRules(game);

        Assert.Same(straddle, rules.Straddle);
        Assert.True(rules.Straddle!.IsMandatory);
        Assert.Equal(StraddleType.Utg, rules.Straddle.Type);
        Assert.Equal([200L, 400L], rules.Straddle.Amounts);
    }

    [Fact]
    public void Constructor_PreservesBoardAndRunoutCounts()
    {
        var game = NoLimitTexasHoldemTestFactory.CreateGame(
            initialBoardCount: 2,
            maxRunoutCount: 4);

        PokerRules rules = NoLimitTexasHoldemTestFactory.GetRules(game);

        Assert.Equal(2, rules.InitialBoardCount);
        Assert.Equal(4, rules.MaxRunoutCount);
    }

    [Fact]
    public void Constructor_ConfiguresFourRoundsInCorrectOrder()
    {
        var game = NoLimitTexasHoldemTestFactory.CreateGame();
        PokerRules rules = NoLimitTexasHoldemTestFactory.GetRules(game);

        Assert.Equal(4, rules.Rounds.Count);

        Assert.Equal(
            [
                RoundType.Preflop,
                RoundType.Flop,
                RoundType.Turn,
                RoundType.River
            ],
            rules.Rounds.Select(round => round.Type).ToArray());
    }

    [Fact]
    public void Constructor_ConfiguresCorrectBoardCardCounts()
    {
        var game = NoLimitTexasHoldemTestFactory.CreateGame();
        PokerRules rules = NoLimitTexasHoldemTestFactory.GetRules(game);

        Assert.Equal(
            [0, 3, 1, 1],
            rules.Rounds.Select(round => round.BoardCardCount).ToArray());
    }

    [Fact]
    public void Constructor_UsesBigBlindAsBetSizeForEveryRound()
    {
        var game = NoLimitTexasHoldemTestFactory.CreateGame(
            smallBlind: 250,
            bigBlind: 500);

        PokerRules rules = NoLimitTexasHoldemTestFactory.GetRules(game);

        Assert.All(
            rules.Rounds,
            round => Assert.Equal(500, round.BetSize));
    }

    [Fact]
    public void Constructor_AllowsUnlimitedRaisesForEveryRound()
    {
        var game = NoLimitTexasHoldemTestFactory.CreateGame();
        PokerRules rules = NoLimitTexasHoldemTestFactory.GetRules(game);

        Assert.All(
            rules.Rounds,
            round => Assert.Null(round.MaxRaises));
    }
}
