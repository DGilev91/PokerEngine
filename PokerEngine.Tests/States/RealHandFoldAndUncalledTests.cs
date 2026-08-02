using PokerEngine.Enums;
using PokerEngine.Models;
using PokerEngine.States;

namespace PokerEngine.Tests.RealHands;

public sealed class RealHandFoldAndUncalledTests
{
    // ClubGG Hand #1723263323
    // Physical seats rotated to state seats: 2->state0, 3->state1, 4->state2, 5->state3, 6->state4, 7->state5, 8->state6, 1->state7
    [Fact]
    public void Hand1723263323_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([15_888, 10_000, 64_971, 22_150, 45_219, 82_509, 12_374, 29_447]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.RaiseTo, 800);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(7, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Call);
        state.DealBoard(0, ["8s", "4c", "Kc"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(5, ActionType.Bet, 1_275);
        state.PlayerAction(1, ActionType.Call);
        state.DealBoard(0, ["Ks"]);
        state.PlayerAction(1, ActionType.Bet, 4_250);
        state.PlayerAction(5, ActionType.RaiseTo, 10_625);
        state.PlayerAction(1, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(5, uncalled[0].seatId);
        Assert.Equal(6_375, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([5], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 282_558);
    }

    // ClubGG Hand #1723264247
    // Physical seats rotated to state seats: 3->state0, 4->state1, 5->state2, 6->state3, 7->state4, 8->state5, 1->state6, 2->state7
    [Fact]
    public void Hand1723264247_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([3_675, 64_971, 22_150, 45_219, 88_089, 12_374, 29_447, 15_788]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.RaiseTo, 400);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(7, ActionType.Fold);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Call);
        state.DealBoard(0, ["Ah", "6c", "8d"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(2, ActionType.Check);
        state.PlayerAction(3, ActionType.Bet, 1_056);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(2, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(3, uncalled[0].seatId);
        Assert.Equal(1_056, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([3], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 281_713);
    }

    // ClubGG Hand #1723266523
    // Physical seats rotated to state seats: 5->state0, 6->state1, 7->state2, 8->state3, 1->state4, 2->state5, 4->state6
    [Fact]
    public void Hand1723266523_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([21_550, 49_407, 88_089, 12_374, 29_447, 15_788, 64_471]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.RaiseTo, 800);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(2, uncalled[0].seatId);
        Assert.Equal(600, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([2], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 281_126);
    }

    // ClubGG Hand #1723274151
    // Physical seats rotated to state seats: 2->state0, 3->state1, 6->state2, 7->state3, 1->state4
    [Fact]
    public void Hand1723274151_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([16_522, 10_000, 37_216, 118_491, 27_622]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.RaiseTo, 800);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Call);
        state.DealBoard(0, ["6d", "Th", "Js"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(3, ActionType.Bet, 1_200);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(3, uncalled[0].seatId);
        Assert.Equal(1_200, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([3], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 209_851);
    }

    // ClubGG Hand #1723276046
    // Physical seats rotated to state seats: 6->state0, 7->state1, 1->state2, 2->state3, 3->state4
    [Fact]
    public void Hand1723276046_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([37_016, 111_371, 27_622, 15_722, 26_052]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.Call);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.RaiseTo, 600);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.RaiseTo, 1_227);
        state.PlayerAction(1, ActionType.Call);
        state.PlayerAction(3, ActionType.Call);
        state.DealBoard(0, ["Js", "5d", "Qd"]);
        state.PlayerAction(1, ActionType.Bet, 1_890);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.Call);
        state.DealBoard(0, ["6d"]);
        state.PlayerAction(1, ActionType.Bet, 4_725);
        state.PlayerAction(3, ActionType.RaiseTo, 12_605);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(3, uncalled[0].seatId);
        Assert.Equal(7_880, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([3], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 217_783);
    }

    // ClubGG Hand #1723282882
    // Physical seats rotated to state seats: 4->state0, 5->state1, 6->state2, 7->state3, 1->state4
    [Fact]
    public void Hand1723282882_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([15_539, 18_800, 36_716, 135_344, 27_222]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.RaiseTo, 600);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(3, uncalled[0].seatId);
        Assert.Equal(400, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([3], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 233_621);
    }

    // ClubGG Hand #1723293859
    // Physical seats rotated to state seats: 8->state0, 1->state1, 3->state2, 4->state3, 5->state4, 6->state5, 7->state6
    [Fact]
    public void Hand1723293859_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([39_800, 23_997, 31_838, 8_369, 19_700, 48_996, 109_932]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.RaiseTo, 800);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(2, ActionType.Call);
        state.DealBoard(0, ["9s", "7s", "9d"]);
        state.PlayerAction(2, ActionType.Check);
        state.PlayerAction(5, ActionType.Bet, 1_900);
        state.PlayerAction(2, ActionType.Call);
        state.DealBoard(0, ["Qh"]);
        state.PlayerAction(2, ActionType.Check);
        state.PlayerAction(5, ActionType.Bet, 8_000);
        state.PlayerAction(2, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(5, uncalled[0].seatId);
        Assert.Equal(8_000, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([5], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 282_632);
    }

    // ClubGG Hand #1723297882
    // Physical seats rotated to state seats: 4->state0, 5->state1, 6->state2, 7->state3, 8->state4, 1->state5, 3->state6
    [Fact]
    public void Hand1723297882_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([11_889, 33_392, 51_297, 109_932, 39_700, 22_597, 9_161]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(6, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);
        state.DealBoard(0, ["Tc", "Js", "2d"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(1, ActionType.Bet, 400);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.DealBoard(0, ["Qd"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(6, ActionType.Check);
        state.DealBoard(0, ["6d"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(6, ActionType.Bet, 2_000);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(6, uncalled[0].seatId);
        Assert.Equal(2_000, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([6], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 277_968);
    }

    // ClubGG Hand #1723302241
    // Physical seats rotated to state seats: 7->state0, 8->state1, 1->state2, 3->state3, 4->state4, 5->state5, 6->state6
    [Fact]
    public void Hand1723302241_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([109_732, 39_700, 22_197, 11_508, 11_136, 32_592, 50_497]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.RaiseTo, 900);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.RaiseTo, 12_800);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(1, uncalled[0].seatId);
        Assert.Equal(11_900, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([1], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 277_362);
    }

    // ClubGG Hand #1723304568
    // Physical seats rotated to state seats: 1->state0, 4->state1, 5->state2, 6->state3, 7->state4, 8->state5
    [Fact]
    public void Hand1723304568_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([21_997, 10_236, 31_892, 61_309, 109_632, 40_800]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.RaiseTo, 400);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Fold);
        state.DealBoard(0, ["8h", "Ad", "3c"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(3, ActionType.Bet, 700);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(0, ActionType.Fold);
        state.DealBoard(0, ["Jd"]);
        state.PlayerAction(3, ActionType.Bet, 1_848);
        state.PlayerAction(5, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(3, uncalled[0].seatId);
        Assert.Equal(1_848, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([3], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 275_866);
    }

    // ClubGG Hand #1723305741
    // Physical seats rotated to state seats: 4->state0, 5->state1, 6->state2, 7->state3, 8->state4, 1->state5
    [Fact]
    public void Hand1723305741_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([10_036, 31_892, 62_761, 109_632, 39_700, 21_597]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Check);
        state.DealBoard(0, ["Js", "3s", "Qd"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(3, ActionType.Bet, 350);
        state.PlayerAction(5, ActionType.RaiseTo, 1_400);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(3, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(5, uncalled[0].seatId);
        Assert.Equal(1_050, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([5], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 275_618);
    }

    // ClubGG Hand #1723306504
    // Physical seats rotated to state seats: 5->state0, 6->state1, 7->state2, 8->state3, 1->state4, 4->state5
    [Fact]
    public void Hand1723306504_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([31_692, 62_761, 109_082, 39_700, 22_363, 9_936]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.RaiseTo, 700);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(5, uncalled[0].seatId);
        Assert.Equal(500, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([5], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 275_534);
    }

    // ClubGG Hand #1723308546
    // Physical seats rotated to state seats: 7->state0, 8->state1, 1->state2, 4->state3, 5->state4, 6->state5
    [Fact]
    public void Hand1723308546_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([108_912, 39_700, 22_363, 10_066, 31_592, 62_461]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(1, uncalled[0].seatId);
        Assert.Equal(200, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([1], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 275_094);
    }

    // ClubGG Hand #1723309012
    // Physical seats rotated to state seats: 8->state0, 1->state1, 4->state2, 5->state3, 6->state4, 7->state5
    [Fact]
    public void Hand1723309012_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([39_800, 22_363, 10_066, 31_592, 62_461, 108_812]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(0, ActionType.RaiseTo, 400);
        state.PlayerAction(1, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(0, uncalled[0].seatId);
        Assert.Equal(200, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([0], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 275_094);
    }

    // ClubGG Hand #1723309980
    // Physical seats rotated to state seats: 1->state0, 4->state1, 5->state2, 6->state3, 7->state4, 8->state5
    [Fact]
    public void Hand1723309980_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([22_163, 10_066, 31_592, 62_461, 108_812, 40_000]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.RaiseTo, 500);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.RaiseTo, 800);
        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(5, ActionType.Call);
        state.DealBoard(0, ["Qd", "5c", "7s"]);
        state.PlayerAction(1, ActionType.Bet, 200);
        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(5, ActionType.Call);
        state.DealBoard(0, ["6d"]);
        state.PlayerAction(1, ActionType.Bet, 9_066);
        state.PlayerAction(5, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(1, uncalled[0].seatId);
        Assert.Equal(9_066, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([1], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 275_094);
    }

    // ClubGG Hand #1723310959
    // Physical seats rotated to state seats: 4->state0, 5->state1, 6->state2, 7->state3, 8->state4, 1->state5
    [Fact]
    public void Hand1723310959_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([11_712, 30_792, 62_461, 108_812, 39_000, 22_063]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);
        state.DealBoard(0, ["Qd", "7c", "7h"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(3, ActionType.Check);
        state.PlayerAction(5, ActionType.Bet, 264);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(3, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(5, uncalled[0].seatId);
        Assert.Equal(264, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([5], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 274_840);
    }

    // ClubGG Hand #1723314780
    // Physical seats rotated to state seats: 8->state0, 1->state1, 2->state2, 3->state3, 4->state4, 5->state5, 6->state6, 7->state7
    [Fact]
    public void Hand1723314780_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([38_800, 22_615, 21_550, 43_670, 4_762, 30_292, 62_061, 112_981]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(7, ActionType.Fold);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);
        state.DealBoard(0, ["Td", "Qh", "6s"]);
        state.PlayerAction(0, ActionType.Bet, 450);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(2, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(0, uncalled[0].seatId);
        Assert.Equal(450, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([0], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 336_731);
    }

    // ClubGG Hand #1723318430
    // Physical seats rotated to state seats: 3->state0, 5->state1, 6->state2, 7->state3, 8->state4, 1->state5, 2->state6
    [Fact]
    public void Hand1723318430_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([18_115, 30_292, 61_661, 112_981, 39_164, 18_115, 52_288]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.RaiseTo, 700);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Call);
        state.DealBoard(0, ["9s", "Ac", "4h"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(6, ActionType.Check);
        state.DealBoard(0, ["Js"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(6, ActionType.Bet, 1_500);
        state.PlayerAction(1, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(6, uncalled[0].seatId);
        Assert.Equal(1_500, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([6], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 332_616);
    }

    // ClubGG Hand #1723320484
    // Physical seats rotated to state seats: 6->state0, 7->state1, 8->state2, 1->state3, 2->state4, 3->state5, 5->state6
    [Fact]
    public void Hand1723320484_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([56_561, 112_981, 39_164, 18_115, 52_098, 23_363, 29_492]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.RaiseTo, 400);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(4, uncalled[0].seatId);
        Assert.Equal(200, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([4], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 331_774);
    }

    // ClubGG Hand #1723320940
    // Physical seats rotated to state seats: 7->state0, 8->state1, 1->state2, 2->state3, 3->state4, 5->state5, 6->state6
    [Fact]
    public void Hand1723320940_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([112_781, 39_164, 18_115, 52_398, 23_363, 29_492, 56_461]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.RaiseTo, 900);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(3, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(4, uncalled[0].seatId);
        Assert.Equal(700, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([4], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 331_774);
    }

    // ClubGG Hand #1723322037
    // Physical seats rotated to state seats: 1->state0, 2->state1, 3->state2, 4->state3, 5->state4, 6->state5, 7->state6, 8->state7
    [Fact]
    public void Hand1723322037_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([17_915, 49_573, 23_863, 39_800, 29_492, 56_461, 115_381, 38_864]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.RaiseTo, 700);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(7, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Call);
        state.DealBoard(0, ["2d", "Jc", "6c"]);
        state.PlayerAction(1, ActionType.Bet, 495);
        state.PlayerAction(2, ActionType.Call);
        state.DealBoard(0, ["5c"]);
        state.PlayerAction(1, ActionType.Bet, 821);
        state.PlayerAction(2, ActionType.Call);
        state.DealBoard(0, ["Ks"]);
        state.PlayerAction(1, ActionType.Bet, 1_363);
        state.PlayerAction(2, ActionType.RaiseTo, 8_221);
        state.PlayerAction(1, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Equal(1, uncalled.Length);
        Assert.Equal(2, uncalled[0].seatId);
        Assert.Equal(6_858, uncalled[0].amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.seatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([2], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 371_349);
    }

}