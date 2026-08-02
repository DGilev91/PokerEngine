using PokerEngine.Enums;
using PokerEngine.States;
using PokerEngine.States.Events;

namespace PokerEngine.Tests.RealHands;

public sealed class RealHandShowdownTests
{
    // Physical seats rotated to state seats: 8->state0, 1->state1, 2->state2, 4->state3, 5->state4, 6->state5, 7->state6
    [Fact]
    public void Hand1723260432_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([6_124, 40_000, 22_212, 65_171, 22_350, 46_019, 47_001]);
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

        state.PlayerAction(2, ActionType.RaiseTo, 800);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(0, ActionType.RaiseTo, 2_750);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(5, ActionType.Fold);
        state.DealBoard(0, ["9h", "Qh", "9s"]);
        state.PlayerAction(0, ActionType.Bet, 3_374);
        state.PlayerAction(2, ActionType.Call);
        state.DealBoard(0, ["3c"]);
        state.DealBoard(0, ["6h"]);

        state.ShowCards(0, ["Kd", "Kc"]);
        state.ShowCards(2, ["Ts", "Td"]);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([0], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 248_877);
    }

    // Physical seats rotated to state seats: 1->state0, 2->state1, 4->state2, 5->state3, 6->state4, 7->state5, 8->state6
    [Fact]
    public void Hand1723261659_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([39_800, 16_088, 65_171, 22_350, 45_219, 47_001, 12_374]);
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

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.RaiseTo, 800);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(0, ActionType.RaiseTo, 3_000);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(5, ActionType.Call);
        state.DealBoard(0, ["3d", "Th", "5c"]);
        state.PlayerAction(0, ActionType.Bet, 2_178);
        state.PlayerAction(5, ActionType.RaiseTo, 7_656);
        state.PlayerAction(0, ActionType.Call);
        state.DealBoard(0, ["7h"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(5, ActionType.Check);
        state.DealBoard(0, ["7c"]);
        state.PlayerAction(0, ActionType.Bet, 29_144);
        state.PlayerAction(5, ActionType.Call);

        state.ShowCards(0, ["As", "Ad"]);
        state.ShowCards(5, ["7s", "7d"]);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([5], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 248_003);
    }

    // Physical seats rotated to state seats: 1->state0, 2->state1, 3->state2, 4->state3, 5->state4, 6->state5, 7->state6, 8->state7
    [Fact]
    public void Hand1723272174_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([27_722, 16_722, 2_580, 64_071, 19_700, 49_007, 94_404, 11_791]);
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

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.RaiseTo, 800);
        state.PlayerAction(6, ActionType.Call);
        state.PlayerAction(7, ActionType.RaiseTo, 2_250);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(6, ActionType.Call);
        state.DealBoard(0, ["6d", "7h", "Kd"]);
        state.PlayerAction(2, ActionType.Bet, 330);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(6, ActionType.RaiseTo, 5_475);
        state.PlayerAction(7, ActionType.RaiseTo, 9_541);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(6, ActionType.Call);
        state.DealBoard(0, ["8c"]);
        state.PlayerAction(5, ActionType.Check);
        state.PlayerAction(6, ActionType.Check);
        state.DealBoard(0, ["7d"]);
        state.PlayerAction(5, ActionType.Check);
        state.PlayerAction(6, ActionType.Bet, 19_126);
        state.PlayerAction(5, ActionType.Fold);

        state.ShowCards(2, ["Qc", "7s"]);
        state.ShowCards(6, ["Ad", "4d"]);
        state.ShowCards(7, ["Ts", "Th"]);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Single(uncalled);
        Assert.Equal(6, uncalled[0].SeatId);
        Assert.Equal(19_126, uncalled[0].Amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([6], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 285_997);
    }

    // Physical seats rotated to state seats: 1->state0, 3->state1, 4->state2, 5->state3, 6->state4, 7->state5
    [Fact]
    public void Hand1723280111_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([27_222, 25_315, 19_187, 19_200, 36_916, 109_494]);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.DealHole(0, ["xx", "xx"]);
        state.DealHole(1, ["xx", "xx"]);
        state.DealHole(2, ["xx", "xx"]);
        state.DealHole(3, ["xx", "xx"]);
        state.DealHole(4, ["xx", "xx"]);
        state.DealHole(5, ["xx", "xx"]);

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.RaiseTo, 400);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Call);
        state.DealBoard(0, ["2h", "Qc", "Qs"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(3, ActionType.Check);
        state.PlayerAction(5, ActionType.Check);
        state.DealBoard(0, ["2d"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(3, ActionType.Check);
        state.PlayerAction(5, ActionType.Bet, 600);
        state.PlayerAction(1, ActionType.Call);
        state.PlayerAction(3, ActionType.Fold);
        state.DealBoard(0, ["7d"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(5, ActionType.Check);

        state.ShowCards(1, ["As", "4c"]);
        state.ShowCards(5, ["Kc", "Ts"]);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([1], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 237_334);
    }

    // Physical seats rotated to state seats: 3->state0, 4->state1, 5->state2, 6->state3, 7->state4, 1->state5
    [Fact]
    public void Hand1723281150_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([26_491, 19_187, 18_800, 36_916, 108_494, 27_222]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.DealHole(0, ["xx", "xx"]);
        state.DealHole(1, ["xx", "xx"]);
        state.DealHole(2, ["xx", "xx"]);
        state.DealHole(3, ["xx", "xx"]);
        state.DealHole(4, ["xx", "xx"]);
        state.DealHole(5, ["xx", "xx"]);

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.Call);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(0, ActionType.RaiseTo, 800);
        state.PlayerAction(1, ActionType.RaiseTo, 1_800);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Call);
        state.PlayerAction(0, ActionType.RaiseTo, 3_648);
        state.PlayerAction(1, ActionType.Call);
        state.PlayerAction(4, ActionType.Call);
        state.DealBoard(0, ["2h", "Js", "Kh"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(4, ActionType.Bet, 3_677);
        state.PlayerAction(0, ActionType.RaiseTo, 9_781);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(4, ActionType.Call);
        state.DealBoard(0, ["8h"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(4, ActionType.Bet, 15_353);
        state.PlayerAction(0, ActionType.Call);
        state.DealBoard(0, ["Jd"]);

        state.ShowCards(0, ["Ad", "Ah"]);
        state.ShowCards(4, ["Qs", "Jc"]);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Single(uncalled);
        Assert.Equal(4, uncalled[0].SeatId);
        Assert.Equal(2_291, uncalled[0].Amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([4], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 237_110);
    }

    // Physical seats rotated to state seats: 1->state0, 3->state1, 4->state2, 5->state3, 6->state4, 7->state5, 8->state6
    [Fact]
    public void Hand1723294914_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([23_797, 29_138, 8_369, 19_700, 51_574, 109_932, 39_700]);
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

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(3, ActionType.RaiseTo, 700);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Call);
        state.PlayerAction(2, ActionType.RaiseTo, 3_500);
        state.PlayerAction(3, ActionType.RaiseTo, 6_300);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Call);
        state.PlayerAction(2, ActionType.RaiseTo, 8_369);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(1, ActionType.Call);
        state.DealBoard(0, ["7c", "8d", "Qc"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(3, ActionType.Check);
        state.DealBoard(0, ["Jd"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(3, ActionType.Bet, 2_600);
        state.PlayerAction(1, ActionType.Call);
        state.DealBoard(0, ["6h"]);
        state.PlayerAction(1, ActionType.Bet, 10_232);
        state.PlayerAction(3, ActionType.Call);

        state.ShowCards(1, ["9s", "7s"]);
        state.ShowCards(2, ["Ah", "Jh"]);
        state.ShowCards(3, ["Ac", "Js"]);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Single(uncalled);
        Assert.Equal(1, uncalled[0].SeatId);
        Assert.Equal(1_501, uncalled[0].Amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([2, 3], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 282_210);
    }

    // Physical seats rotated to state seats: 5->state0, 6->state1, 7->state2, 8->state3, 1->state4, 3->state5, 4->state6
    [Fact]
    public void Hand1723299400_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([32_792, 51_297, 109_932, 39_700, 22_397, 10_361, 11_289]);
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
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Call);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(6, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);
        state.DealBoard(0, ["2c", "3d", "4d"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(4, ActionType.Check);
        state.PlayerAction(5, ActionType.Bet, 500);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Call);
        state.PlayerAction(4, ActionType.Fold);
        state.DealBoard(0, ["5c"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(5, ActionType.Check);
        state.DealBoard(0, ["Jd"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(5, ActionType.Check);

        state.ShowCards(1, ["Tc", "3c"]);
        state.ShowCards(5, ["Ks", "4c"]);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([5], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 277_768);
    }

    // Physical seats rotated to state seats: 6->state0, 7->state1, 8->state2, 1->state3, 3->state4, 4->state5, 5->state6
    [Fact]
    public void Hand1723300987_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([50_597, 109_932, 39_700, 22_197, 11_461, 11_089, 32_592]);
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
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Call);
        state.PlayerAction(5, ActionType.RaiseTo, 900);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(4, ActionType.Call);
        state.DealBoard(0, ["8h", "Kc", "5h"]);
        state.PlayerAction(4, ActionType.Check);
        state.PlayerAction(5, ActionType.Check);
        state.DealBoard(0, ["9h"]);
        state.PlayerAction(4, ActionType.Check);
        state.PlayerAction(5, ActionType.Check);
        state.DealBoard(0, ["Js"]);
        state.PlayerAction(4, ActionType.Check);
        state.PlayerAction(5, ActionType.Check);

        state.ShowCards(4, ["Qc", "7d"]);
        state.ShowCards(5, ["Qs", "3s"]);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([4, 5], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 277_568);
    }

    // Physical seats rotated to state seats: 6->state0, 7->state1, 8->state2, 1->state3, 4->state4, 5->state5
    [Fact]
    public void Hand1723307134_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([62_561, 109_082, 39_700, 22_363, 10_236, 31_592]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.DealHole(0, ["xx", "xx"]);
        state.DealHole(1, ["xx", "xx"]);
        state.DealHole(2, ["xx", "xx"]);
        state.DealHole(3, ["xx", "xx"]);
        state.DealHole(4, ["xx", "xx"]);
        state.DealHole(5, ["xx", "xx"]);

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.RaiseTo, 700);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Call);
        state.DealBoard(0, ["Qc", "5h", "Jc"]);
        state.PlayerAction(1, ActionType.Bet, 750);
        state.PlayerAction(4, ActionType.Call);
        state.DealBoard(0, ["Th"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(4, ActionType.Bet, 1_500);
        state.PlayerAction(1, ActionType.Call);
        state.DealBoard(0, ["8d"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(4, ActionType.Check);

        state.ShowCards(1, ["Ah", "7c"]);
        state.ShowCards(4, ["Ad", "3h"]);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([1, 4], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 275_534);
    }

    // Physical seats rotated to state seats: 6->state0, 7->state1, 8->state2, 1->state3, 3->state4, 4->state5, 5->state6
    [Fact]
    public void Hand1723312750_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([62_261, 109_706, 39_000, 22_615, 39_800, 10_712, 30_492]);
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
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Call);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(6, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);
        state.DealBoard(0, ["Qs", "4h", "7h"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(4, ActionType.Bet, 750);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);
        state.DealBoard(0, ["Tc"]);
        state.PlayerAction(4, ActionType.Bet, 1_875);
        state.PlayerAction(5, ActionType.Call);
        state.DealBoard(0, ["Ks"]);
        state.PlayerAction(4, ActionType.Check);
        state.PlayerAction(5, ActionType.Bet, 3_125);
        state.PlayerAction(4, ActionType.Call);

        state.ShowCards(4, ["Qc", "2d"]);
        state.ShowCards(5, ["8c", "6c"]);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([4], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 314_586);
    }

    // Physical seats rotated to state seats: 1->state0, 2->state1, 3->state2, 4->state3, 5->state4, 6->state5, 7->state6, 8->state7
    [Fact]
    public void Hand1723315678_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([22_415, 21_350, 43_670, 4_762, 30_292, 62_061, 112_981, 39_164]);
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
        state.PlayerAction(3, ActionType.RaiseTo, 4_762);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(7, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Call);
        state.DealBoard(0, ["4c", "Ts", "2s"]);
        state.DealBoard(0, ["9s"]);
        state.DealBoard(0, ["Ks"]);

        state.ShowCards(1, ["Kh", "5c"]);
        state.ShowCards(3, ["Ac", "9d"]);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([1], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 336_695);
    }

    // Physical seats rotated to state seats: 2->state0, 3->state1, 5->state2, 6->state3, 7->state4, 8->state5, 1->state6
    [Fact]
    public void Hand1723316839_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([25_555, 43_670, 30_292, 62_061, 112_981, 39_164, 22_315]);
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
        state.PlayerAction(3, ActionType.RaiseTo, 400);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Call);
        state.DealBoard(0, ["4s", "6s", "3h"]);
        state.PlayerAction(0, ActionType.Bet, 1_200);
        state.PlayerAction(1, ActionType.Call);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(6, ActionType.Call);
        state.DealBoard(0, ["Jh"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(6, ActionType.Bet, 2_600);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.RaiseTo, 12_350);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(0, ActionType.Call);
        state.DealBoard(0, ["Jc"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(1, ActionType.Bet, 29_720);
        state.PlayerAction(0, ActionType.Call);

        state.ShowCards(0, ["6c", "5h"]);
        state.ShowCards(1, ["Ah", "3d"]);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Single(uncalled);
        Assert.Equal(1, uncalled[0].SeatId);
        Assert.Equal(18_115, uncalled[0].Amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([0], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 336_038);
    }

    // Physical seats rotated to state seats: 5->state0, 6->state1, 7->state2, 8->state3, 1->state4, 2->state5, 3->state6
    [Fact]
    public void Hand1723319279_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([29_592, 61_661, 112_981, 39_164, 18_115, 52_998, 18_015]);
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
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(6, ActionType.RaiseTo, 900);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Call);
        state.PlayerAction(5, ActionType.Call);
        state.DealBoard(0, ["4s", "Ks", "6d"]);
        state.PlayerAction(1, ActionType.Bet, 1_400);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Call);
        state.DealBoard(0, ["8s"]);
        state.PlayerAction(1, ActionType.Bet, 2_800);
        state.PlayerAction(6, ActionType.Call);
        state.DealBoard(0, ["2d"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(6, ActionType.Check);

        state.ShowCards(1, ["7c", "6c"]);
        state.ShowCards(6, ["Ad", "6s"]);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([6], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 332_526);
    }


    [Fact]
    public void Hand1723666678_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(
            smallBlind: 50,
            bigBlind: 100,
            maxRunoutCount: 1);

        state.Initialize([
            15_050, // state 0: physical seat 5, pid42261396
        10_311, // state 1: physical seat 3, pid69676394
        8_794   // state 2: physical seat 4, pid97347399
        ]);

        state.PlayerPost(
            0,
            PostType.SmallBlind,
            50);

        state.PlayerPost(
            1,
            PostType.BigBlind,
            100);

        state.Start();

        state.DealHole(0, ["xx", "xx"]);
        state.DealHole(1, ["xx", "xx"]);
        state.DealHole(2, ["xx", "xx"]);

        // Preflop
        state.PlayerAction(
            2,
            ActionType.Fold);

        state.PlayerAction(
            0,
            ActionType.Call);

        state.PlayerAction(
            1,
            ActionType.Check);

        // Flop: [3s 8c 5s]
        state.DealBoard(
            0,
            ["3s", "8c", "5s"]);

        state.PlayerAction(
            0,
            ActionType.Bet,
            100);

        state.PlayerAction(
            1,
            ActionType.Call);

        // Turn: [8s]
        state.DealBoard(
            0,
            ["8s"]);

        state.PlayerAction(
            0,
            ActionType.Bet,
            132);

        state.PlayerAction(
            1,
            ActionType.Call);

        // River: [Th]
        state.DealBoard(
            0,
            ["Th"]);

        state.PlayerAction(
            0,
            ActionType.Bet,
            332);

        state.PlayerAction(
            1,
            ActionType.Call);

        // Showdown
        state.ShowCards(
            0,
            ["Td", "5d"]);

        state.ShowCards(
            1,
            ["Ts", "3h"]);

        Assert.Empty(
            state.Events
                .OfType<UncalledBetReturnedEvent>());

        PotAwardedEvent[] awards = state.Events
            .OfType<PotAwardedEvent>()
            .OrderBy(award => award.SeatId)
            .ToArray();

        Assert.Equal(
            2,
            awards.Length);

        Assert.Equal(
            0,
            awards[0].SeatId);

        Assert.Equal(
            664,
            awards[0].Amount);

        Assert.Equal(
            1,
            awards[1].SeatId);

        Assert.Equal(
            664,
            awards[1].Amount);

        Assert.Equal(
            HandState.Completed,
            state.State);

        RealHandTestFactory.AssertCompletedAndConserved(
            state,
            34_155);
    }

}