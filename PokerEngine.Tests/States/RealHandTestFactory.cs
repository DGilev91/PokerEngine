using PokerEngine.Enums;
using PokerEngine.Games;
using PokerEngine.Models;
using PokerEngine.States;

namespace PokerEngine.Tests.RealHands;

internal static class RealHandTestFactory
{
    public static PokerState CreateState(int maxRunoutCount = 1)
    {
        NoLimitTexasHoldem game = new(
            Automation.None,
            smallBlind: 100,
            bigBlind: 200,
            ante: null,
            straddle: null,
            initialBoardCount: 1,
            maxRunoutCount: maxRunoutCount);

        return (PokerState)game.CreateState();
    }

    public static void AssertCompletedAndConserved(
        PokerState state,
        long initialChipCount)
    {
        Assert.Equal(HandState.Completed, state.State);
        Assert.Equal(initialChipCount, state.Seats.Sum(seat => seat.Stack));
        Assert.Single(state.Events.OfType<EndHandEvent>());
    }
}
