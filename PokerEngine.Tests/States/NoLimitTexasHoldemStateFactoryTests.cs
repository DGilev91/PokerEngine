using PokerEngine.States;

namespace PokerEngine.Tests.Games;

public sealed class NoLimitTexasHoldemStateFactoryTests
{
    [Fact]
    public void CreateState_ReturnsPokerState()
    {
        var game = NoLimitTexasHoldemTestFactory.CreateGame();

        IPokerState state = game.CreateState();

        Assert.IsType<PokerState>(state);
    }

    [Fact]
    public void CreateState_ReturnsNewInstanceEveryTime()
    {
        var game = NoLimitTexasHoldemTestFactory.CreateGame();

        IPokerState first = game.CreateState();
        IPokerState second = game.CreateState();

        Assert.NotSame(first, second);
    }

    [Fact]
    public void CreateState_StatesAreIndependent()
    {
        var game = NoLimitTexasHoldemTestFactory.CreateGame();

        PokerState first = (PokerState)game.CreateState();
        PokerState second = (PokerState)game.CreateState();

        first.Initialize([1_000, 1_000, 1_000]);

        Assert.Equal(3, first.Seats.Count);
        Assert.Empty(second.Seats);
        Assert.NotEqual(first.State, second.State);
    }

    [Fact]
    public void CreateState_UsesConfiguredInitialBoardCount()
    {
        var game = NoLimitTexasHoldemTestFactory.CreateGame(
            initialBoardCount: 3);

        PokerState state = (PokerState)game.CreateState();

        state.Initialize([1_000, 1_000]);

        Assert.Equal(3, state.Boards.Count);
        Assert.All(state.Boards, board => Assert.Empty(board));
    }

    [Fact]
    public void CreateState_UsesConfiguredBlinds()
    {
        PokerState state = NoLimitTexasHoldemTestFactory.CreateState(
            automation: PokerEngine.Enums.Automation.PostBlinds,
            smallBlind: 250,
            bigBlind: 500);

        state.Initialize([10_000, 10_000, 10_000]);

        Assert.Equal(250, state.Seats[0].RoundBet);
        Assert.Equal(500, state.Seats[1].RoundBet);
        Assert.Equal(9_750, state.Seats[0].Stack);
        Assert.Equal(9_500, state.Seats[1].Stack);
    }
}
