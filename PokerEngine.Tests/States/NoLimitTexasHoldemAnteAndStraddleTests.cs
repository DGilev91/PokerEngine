using PokerEngine.Enums;
using PokerEngine.States;
using PokerEngine.States.Events;

namespace PokerEngine.Tests.States;

public sealed class NoLimitTexasHoldemAnteAndStraddleTests
{
    [Fact]
    public void EveryPlayerAnte_IsDeadMoneyAndDoesNotReduceCallAmount()
    {
        PokerState state = NoLimitTexasHoldemTestFactory.CreateState(
            automation: Automation.PostAntes | Automation.PostBlinds,
            ante: NoLimitTexasHoldemTestFactory.CreateAnte(25));

        state.Initialize([1_000, 1_000, 1_000]);
        state.Start();

        PlayerTurnEvent turn = Assert.IsType<PlayerTurnEvent>(state.Events.Last());

        Assert.Equal(2, turn.SeatId);
        Assert.Equal(100, turn.CallAmount);
        Assert.Equal(25, state.Seats[2].TotalBet);
        Assert.Equal(0, state.Seats[2].RoundBet);
    }

    [Fact]
    public void MandatoryUtgStraddle_FirstActionIsAfterLastStraddle()
    {
        PokerState state = NoLimitTexasHoldemTestFactory.CreateState(
            automation: Automation.PostBlinds | Automation.PostStraddles,
            straddle: NoLimitTexasHoldemTestFactory.CreateStraddles(
                true,
                StraddleType.Utg,
                200,
                400));

        state.Initialize([1_000, 1_000, 1_000, 1_000, 1_000]);
        state.Start();

        PlayerTurnEvent turn = Assert.IsType<PlayerTurnEvent>(state.Events.Last());

        Assert.Equal(4, turn.SeatId);
        Assert.Equal(400, turn.CallAmount);
        Assert.Equal(800, turn.MinRaiseTo);
    }

    [Fact]
    public void OptionalStraddle_IsNotAutomaticallyPosted()
    {
        PokerState state = NoLimitTexasHoldemTestFactory.CreateState(
            automation: Automation.PostStraddles,
            straddle: NoLimitTexasHoldemTestFactory.CreateStraddles(
                false,
                StraddleType.Utg,
                200));

        state.Initialize([1_000, 1_000, 1_000, 1_000]);

        Assert.Empty(
            state.Events
                .OfType<PlayerPostedEvent>()
                .Where(post => post.PostType == PostType.Straddle));
    }

    [Fact]
    public void MandatoryButtonStraddle_IsPostedByLastSeat()
    {
        PokerState state = NoLimitTexasHoldemTestFactory.CreateState(
            automation: Automation.PostStraddles,
            straddle: NoLimitTexasHoldemTestFactory.CreateStraddles(
                true,
                StraddleType.Button,
                200));

        state.Initialize([1_000, 1_000, 1_000, 1_000]);

        Assert.Equal(200, state.Seats[3].RoundBet);

        PlayerPostedEvent post = Assert.Single(
            state.Events
                .OfType<PlayerPostedEvent>()
                .Where(e => e.PostType == PostType.Straddle));

        Assert.Equal(3, post.SeatId);
        Assert.Equal(200, post.Amount);
    }

    [Fact]
    public void AnteBlindsAndStraddle_ArePostedInCorrectOrder()
    {
        PokerState state = NoLimitTexasHoldemTestFactory.CreateState(
            automation:
                Automation.PostAntes |
                Automation.PostBlinds |
                Automation.PostStraddles,
            ante: NoLimitTexasHoldemTestFactory.CreateAnte(25),
            straddle: NoLimitTexasHoldemTestFactory.CreateStraddles(
                true,
                StraddleType.Utg,
                200));

        state.Initialize([1_000, 1_000, 1_000, 1_000]);

        PlayerPostedEvent[] posts =
            state.Events.OfType<PlayerPostedEvent>().ToArray();

        Assert.Equal(7, posts.Length);

        Assert.All(
            posts.Take(4),
            post => Assert.Equal(PostType.Ante, post.PostType));

        Assert.Equal(PostType.SmallBlind, posts[4].PostType);
        Assert.Equal(PostType.BigBlind, posts[5].PostType);
        Assert.Equal(PostType.Straddle, posts[6].PostType);
    }
}
