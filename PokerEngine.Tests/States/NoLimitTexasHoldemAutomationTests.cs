using PokerEngine.Enums;
using PokerEngine.Models;
using PokerEngine.States;

namespace PokerEngine.Tests.Games;

public sealed class NoLimitTexasHoldemAutomationTests
{
    [Fact]
    public void PostAntes_AutomaticallyPostsEveryPlayerAnte()
    {
        PokerState state = NoLimitTexasHoldemTestFactory.CreateState(
            automation: Automation.PostAntes,
            ante: NoLimitTexasHoldemTestFactory.CreateAnte(25));

        state.Initialize([1_000, 1_000, 1_000]);

        Assert.All(state.Seats, seat => Assert.Equal(25, seat.TotalBet));
        Assert.All(state.Seats, seat => Assert.Equal(0, seat.RoundBet));
        Assert.Equal(3, state.Events.OfType<PlayerPostedEvent>().Count());
    }

    [Fact]
    public void PostBlinds_AutomaticallyPostsSmallAndBigBlind()
    {
        PokerState state = NoLimitTexasHoldemTestFactory.CreateState(
            automation: Automation.PostBlinds);

        state.Initialize([1_000, 1_000, 1_000]);

        Assert.Equal(50, state.Seats[0].RoundBet);
        Assert.Equal(100, state.Seats[1].RoundBet);
        Assert.Equal(0, state.Seats[2].RoundBet);
    }

    [Fact]
    public void PostStraddles_AutomaticallyPostsMandatoryUtgSequence()
    {
        PokerState state = NoLimitTexasHoldemTestFactory.CreateState(
            automation: Automation.PostStraddles,
            straddle: NoLimitTexasHoldemTestFactory.CreateStraddles(
                true,
                StraddleType.Utg,
                200,
                400));

        state.Initialize([1_000, 1_000, 1_000, 1_000, 1_000]);

        Assert.Equal(200, state.Seats[2].RoundBet);
        Assert.Equal(400, state.Seats[3].RoundBet);
    }

    [Fact]
    public void DealHoleCards_StartDealsTwoCardsToEveryPlayer()
    {
        PokerState state = NoLimitTexasHoldemTestFactory.CreateState(
            automation: Automation.PostBlinds | Automation.DealHoleCards);

        state.Initialize([1_000, 1_000, 1_000, 1_000]);
        state.Start();

        Assert.All(state.Seats, seat => Assert.Equal(2, seat.HoleCards.Count));

        string[] cards = state.Seats
            .SelectMany(seat => seat.HoleCards)
            .ToArray();

        Assert.Equal(8, cards.Length);
        Assert.Equal(8, cards.Distinct().Count());
    }

    [Fact]
    public void DealBoard_CompletingPreflopAutomaticallyDealsFlop()
    {
        PokerState state = NoLimitTexasHoldemTestFactory.CreateState(
            automation: Automation.PostBlinds | Automation.DealBoard);

        state.Initialize([1_000, 1_000, 1_000]);
        state.Start();

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        Assert.Equal(RoundType.Flop, state.Round);
        Assert.Equal(3, state.Boards[0].Count);
    }

    [Fact]
    public void All_ExecutesAllConfiguredAutomaticSetup()
    {
        PokerState state = NoLimitTexasHoldemTestFactory.CreateState(
            automation: Automation.All,
            ante: NoLimitTexasHoldemTestFactory.CreateAnte(25),
            straddle: NoLimitTexasHoldemTestFactory.CreateStraddles(
                true,
                StraddleType.Utg,
                200));

        state.Initialize([1_000, 1_000, 1_000, 1_000]);
        state.Start();

        Assert.Equal(25 + 50, state.Seats[0].TotalBet);
        Assert.Equal(25 + 100, state.Seats[1].TotalBet);
        Assert.Equal(25 + 200, state.Seats[2].TotalBet);
        Assert.Equal(25, state.Seats[3].TotalBet);

        Assert.All(state.Seats, seat => Assert.Equal(2, seat.HoleCards.Count));

        PlayerTurnEvent turn = Assert.IsType<PlayerTurnEvent>(state.Events.Last());

        Assert.Equal(3, turn.seatId);
        Assert.Equal(200, turn.callAmount);
        Assert.Equal(400, turn.minRaiseTo);
    }
}
