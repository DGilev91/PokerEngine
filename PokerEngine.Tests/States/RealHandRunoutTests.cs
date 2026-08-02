using PokerEngine.Enums;
using PokerEngine.States;
using PokerEngine.States.Events;

namespace PokerEngine.Tests.RealHands;

public sealed class RealHandRunoutTests
{
    // Physical seats rotated to state seats: 4->state0, 5->state1, 6->state2, 7->state3, 8->state4, 1->state5, 2->state6, 3->state7
    [Fact]
    public void Hand1723265531_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 2);

        state.Initialize([64_571, 21_750, 46_323, 88_089, 12_374, 29_447, 15_788, 3_275]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.DealHole(0, ["xx", "xx"]);
        state.DealHole(1, ["xx", "xx"]);
        state.DealHole(2, ["xx", "xx"]);
        state.DealHole(3, ["xx", "xx"]);
        state.DealHole(4, ["xx", "xx"]);
        state.DealHole(5, ["xx", "xx"]);
        state.DealHole(6, ["xx", "xx"]);
        state.DealHole(7, ["xx", "xx"]);

        state.PlayerAction(2, ActionType.RaiseTo, 500);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(7, ActionType.RaiseTo, 3_275);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(2, ActionType.Call);
        state.SelectRunoutCount(2);

        state.DealBoard(0, ["3d", "6d", "6c"]);
        state.DealBoard(0, ["Qc"]);
        state.DealBoard(0, ["Ks"]);
        state.DealBoard(1, ["Qh", "3s", "2s"]);
        state.DealBoard(1, ["Kh"]);
        state.DealBoard(1, ["Ac"]);

        state.ShowCards(2, ["8s", "8c"]);
        state.ShowCards(7, ["Js", "8d"]);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([2], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 281_617);
    }

    // Physical seats rotated to state seats: 7->state0, 8->state1, 1->state2, 2->state3, 3->state4, 4->state5, 5->state6, 6->state7
    [Fact]
    public void Hand1723268999_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 2);

        state.Initialize([94_704, 10_000, 29_447, 15_788, 4_175, 64_271, 20_500, 49_007]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.DealHole(0, ["xx", "xx"]);
        state.DealHole(1, ["xx", "xx"]);
        state.DealHole(2, ["xx", "xx"]);
        state.DealHole(3, ["xx", "xx"]);
        state.DealHole(4, ["xx", "xx"]);
        state.DealHole(5, ["xx", "xx"]);
        state.DealHole(6, ["xx", "xx"]);
        state.DealHole(7, ["xx", "xx"]);

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.RaiseTo, 800);
        state.PlayerAction(4, ActionType.Call);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Call);
        state.PlayerAction(7, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.RaiseTo, 2_450);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.Call);
        state.PlayerAction(6, ActionType.Fold);
        state.DealBoard(0, ["Ts", "Ac", "9c"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(3, ActionType.Bet, 4_125);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(1, ActionType.RaiseTo, 7_550);
        state.PlayerAction(3, ActionType.Call);
        state.SelectRunoutCount(2);

        state.DealBoard(0, ["4h"]);
        state.DealBoard(0, ["9d"]);
        state.DealBoard(1, ["Ad"]);
        state.DealBoard(1, ["9s"]);

        state.ShowCards(1, ["9h", "8h"]);
        state.ShowCards(3, ["As", "Jd"]);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([1, 3], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 287_892);
    }

    // Physical seats rotated to state seats: 8->state0, 1->state1, 2->state2, 3->state3, 4->state4, 5->state5, 6->state6, 7->state7
    [Fact]
    public void Hand1723270650_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 2);

        state.Initialize([10_935, 29_447, 16_722, 1_725, 64_271, 19_700, 49_007, 94_604]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.DealHole(0, ["xx", "xx"]);
        state.DealHole(1, ["xx", "xx"]);
        state.DealHole(2, ["xx", "xx"]);
        state.DealHole(3, ["xx", "xx"]);
        state.DealHole(4, ["xx", "xx"]);
        state.DealHole(5, ["xx", "xx"]);
        state.DealHole(6, ["xx", "xx"]);
        state.DealHole(7, ["xx", "xx"]);

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.Call);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(7, ActionType.Call);
        state.PlayerAction(0, ActionType.RaiseTo, 600);
        state.PlayerAction(1, ActionType.Call);
        state.PlayerAction(3, ActionType.RaiseTo, 1_725);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(7, ActionType.Fold);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Call);
        state.DealBoard(0, ["9s", "Ad", "7h"]);
        state.PlayerAction(0, ActionType.Bet, 2_787);
        state.PlayerAction(1, ActionType.Fold);
        state.SelectRunoutCount(2);

        state.DealBoard(0, ["Ah"]);
        state.DealBoard(0, ["Jd"]);
        state.DealBoard(1, ["3c"]);
        state.DealBoard(1, ["2c"]);

        state.ShowCards(0, ["Th", "8s"]);
        state.ShowCards(3, ["Qd", "5d"]);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Single(uncalled);
        Assert.Equal(0, uncalled[0].SeatId);
        Assert.Equal(2_787, uncalled[0].Amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([0, 3], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 286_411);
    }

    // Physical seats rotated to state seats: 3->state0, 4->state1, 5->state2, 6->state3, 7->state4, 8->state5, 1->state6
    [Fact]
    public void Hand1723296671_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 2);

        state.Initialize([9_438, 12_089, 33_392, 51_574, 109_932, 39_700, 23_097]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.DealHole(0, ["xx", "xx"]);
        state.DealHole(1, ["xx", "xx"]);
        state.DealHole(2, ["xx", "xx"]);
        state.DealHole(3, ["xx", "xx"]);
        state.DealHole(4, ["xx", "xx"]);
        state.DealHole(5, ["xx", "xx"]);
        state.DealHole(6, ["xx", "xx"]);

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.RaiseTo, 500);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Call);
        state.PlayerAction(0, ActionType.RaiseTo, 1_061);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(3, ActionType.RaiseTo, 6_000);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(0, ActionType.Call);
        state.DealBoard(0, ["2c", "Qd", "3h"]);
        state.PlayerAction(0, ActionType.Bet, 3_438);
        state.PlayerAction(3, ActionType.Call);
        state.SelectRunoutCount(2);

        state.DealBoard(0, ["2d"]);
        state.DealBoard(0, ["5s"]);
        state.DealBoard(1, ["6d"]);
        state.DealBoard(1, ["5h"]);

        state.ShowCards(0, ["Ac", "6c"]);
        state.ShowCards(3, ["Ad", "7d"]);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([0, 3], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 279_222);
    }

    // Physical seats rotated to state seats: 8->state0, 1->state1, 3->state2, 4->state3, 5->state4, 6->state5, 7->state6
    [Fact]
    public void Hand1723303237_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 3);

        state.Initialize([40_900, 22_197, 11_308, 10_236, 32_592, 50_497, 109_632]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.DealHole(0, ["xx", "xx"]);
        state.DealHole(1, ["xx", "xx"]);
        state.DealHole(2, ["xx", "xx"]);
        state.DealHole(3, ["xx", "xx"]);
        state.DealHole(4, ["xx", "xx"]);
        state.DealHole(5, ["xx", "xx"]);
        state.DealHole(6, ["xx", "xx"]);

        state.PlayerAction(2, ActionType.RaiseTo, 700);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Call);
        state.PlayerAction(5, ActionType.RaiseTo, 3_400);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(4, ActionType.Fold);
        state.DealBoard(0, ["6h", "6s", "Jc"]);
        state.PlayerAction(2, ActionType.Check);
        state.PlayerAction(5, ActionType.Bet, 3_900);
        state.PlayerAction(2, ActionType.RaiseTo, 7_908);
        state.PlayerAction(5, ActionType.Call);
        state.SelectRunoutCount(3);

        state.DealBoard(0, ["Js"]);
        state.DealBoard(0, ["3s"]);
        state.DealBoard(1, ["6c"]);
        state.DealBoard(1, ["Ah"]);
        state.DealBoard(2, ["7c"]);
        state.DealBoard(2, ["5s"]);

        state.ShowCards(2, ["Ts", "Tc"]);
        state.ShowCards(5, ["Qd", "Qh"]);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([5], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 277_362);
    }

    [Fact]
    public void Hand1723270650_AutomaticallyDealsTwoRunoutsAfterSelection()
    {
        PokerState state = RealHandTestFactory.CreateState(
            maxRunoutCount: 2,
            automation: Automation.DealBoard);

        state.Initialize(
        [
            10_935,
            29_447,
            16_722,
            1_725,
            64_271,
            19_700,
            49_007,
            94_604
        ]);

        state.PlayerPost(
            0,
            PostType.SmallBlind,
            100);

        state.PlayerPost(
            1,
            PostType.BigBlind,
            200);

        state.Start();

        state.DealHole(0, ["Th", "8s"]);
        state.DealHole(1, ["Kh", "Kd"]);
        state.DealHole(2, ["2c", "2d"]);
        state.DealHole(3, ["Qd", "5d"]);
        state.DealHole(4, ["Ac", "Jc"]);
        state.DealHole(5, ["7h", "6h"]);
        state.DealHole(6, ["9c", "9d"]);
        state.DealHole(7, ["As", "Ks"]);

        // Preflop
        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.Call);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(7, ActionType.Call);

        state.PlayerAction(
            0,
            ActionType.RaiseTo,
            600);

        state.PlayerAction(
            1,
            ActionType.Call);

        state.PlayerAction(
            3,
            ActionType.RaiseTo,
            1_725);

        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(7, ActionType.Fold);

        state.PlayerAction(
            0,
            ActionType.Call);

        state.PlayerAction(
            1,
            ActionType.Call);

        // The flop must be dealt automatically.
        Assert.Single(state.Boards);
        Assert.Equal(3, state.Boards[0].Count);

        BoardEvent flopEvent = Assert.Single(
            state.Events
                .OfType<BoardEvent>()
                .Where(e =>
                    e.Round == RoundType.Flop));

        Assert.Equal(0, flopEvent.BoardIndex);
        Assert.Equal(3, flopEvent.Cards.Count);

        // Flop action.
        state.PlayerAction(
            0,
            ActionType.Bet,
            2_787);

        state.PlayerAction(
            1,
            ActionType.Fold);

        // Seat 3 is all-in and seat 0 is the only actionable player.
        // The engine must wait for the runout count.
        WaitingRunoutEvent waitingEvent = Assert.Single(
            state.Events
                .OfType<WaitingRunoutEvent>());

        // Turn and river must not be dealt before the decision.
        Assert.Single(state.Boards);
        Assert.Equal(3, state.Boards[0].Count);

        Assert.DoesNotContain(
            state.Events.OfType<BoardEvent>(),
            e => e.Round == RoundType.Turn);

        Assert.DoesNotContain(
            state.Events.OfType<BoardEvent>(),
            e => e.Round == RoundType.River);

        int boardEventCountBeforeSelection = state.Events
            .OfType<BoardEvent>()
            .Count();


        state.SelectRunoutCount(2);

        // Two boards must now exist.
        Assert.Equal(2, state.Boards.Count);

        // Both boards inherit the same flop and receive
        // separate turn and river cards.
        Assert.All(
            state.Boards,
            board => Assert.Equal(5, board.Count));

        Assert.Equal(
            state.Boards[0].Take(3),
            state.Boards[1].Take(3));

        Assert.NotEqual(
            state.Boards[0][3],
            state.Boards[1][3]);

        Assert.NotEqual(
            state.Boards[0][4],
            state.Boards[1][4]);

        RunoutCountSelectedEvent runoutEvent = Assert.Single(
            state.Events
                .OfType<RunoutCountSelectedEvent>());

        Assert.Equal(2, runoutEvent.Count);

        BoardEvent[] boardEvents = state.Events
            .OfType<BoardEvent>()
            .ToArray();

        // One flop + two turns + two rivers.
        Assert.Equal(
            boardEventCountBeforeSelection + 4,
            boardEvents.Length);

        Assert.Equal(
            2,
            boardEvents.Count(e =>
                e.Round == RoundType.Turn));

        Assert.Equal(
            2,
            boardEvents.Count(e =>
                e.Round == RoundType.River));

        Assert.Contains(
            boardEvents,
            e =>
                e.BoardIndex == 0 &&
                e.Round == RoundType.Turn);

        Assert.Contains(
            boardEvents,
            e =>
                e.BoardIndex == 0 &&
                e.Round == RoundType.River);

        Assert.Contains(
            boardEvents,
            e =>
                e.BoardIndex == 1 &&
                e.Round == RoundType.Turn);

        Assert.Contains(
            boardEvents,
            e =>
                e.BoardIndex == 1 &&
                e.Round == RoundType.River);



        Assert.Equal(
    RoundType.Showdown,
    state.Round);

        HandEvaluatedEvent[] evaluated = state.Events
            .OfType<HandEvaluatedEvent>()
            .ToArray();

        // 2 players × 2 boards
        Assert.Equal(4, evaluated.Length);

        Assert.Equal(
            2,
            evaluated.Count(e => e.SeatId == 0));

        Assert.Equal(
            2,
            evaluated.Count(e => e.SeatId == 3));

        PotAwardedEvent[] awards = state.Events
            .OfType<PotAwardedEvent>()
            .ToArray();

        Assert.NotEmpty(awards);

        Assert.All(
            awards,
            award => Assert.Contains(
                award.SeatId,
                [0, 3]));

        Assert.Single(
            state.Events.OfType<EndHandEvent>());

        Assert.Equal(
            HandState.Completed,
            state.State);
    }


}