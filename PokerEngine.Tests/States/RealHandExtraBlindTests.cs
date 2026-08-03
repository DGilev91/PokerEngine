using PokerEngine.Enums;
using PokerEngine.States;
using PokerEngine.States.Events;

namespace PokerEngine.Tests.RealHands;

public sealed class RealHandExtraBlindTests
{
    // Physical seats rotated to state seats: 6->state0, 7->state1, 8->state2, 1->state3, 2->state4, 3->state5, 4->state6, 5->state7
    [Fact]
    public void Hand1723267128_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([49_207, 88_389, 12_374, 29_447, 15_788, 10_000, 64_471, 21_450]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.PlayerPost(5, PostType.ExtraBlind, 200);
        state.Start();



        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Check);
        state.PlayerAction(6, ActionType.Call);
        state.PlayerAction(7, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);
        state.DealBoard(0, ["7h", "6h", "Qs"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(1, ActionType.Bet, 750);
        state.PlayerAction(5, ActionType.Call);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(7, ActionType.Call);
        state.PlayerAction(0, ActionType.Fold);
        state.DealBoard(0, ["8h"]);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(5, ActionType.Check);
        state.PlayerAction(7, ActionType.Check);
        state.DealBoard(0, ["4s"]);
        state.PlayerAction(1, ActionType.Bet, 1_625);
        state.PlayerAction(5, ActionType.RaiseTo, 4_875);
        state.PlayerAction(7, ActionType.Fold);
        state.PlayerAction(1, ActionType.Call);

        state.ShowCards(1, ["7d", "5h"]);
        state.ShowCards(5, ["8c", "4c"]);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([1], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 291_126);
    }

    // Physical seats rotated to state seats: 3->state0, 6->state1, 7->state2, 8->state3, 1->state4, 2->state5
    [Fact]
    public void Hand1723274807_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([9_200, 37_216, 119_867, 10_000, 27_622, 15_722]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.PlayerPost(3, PostType.ExtraBlind, 200);
        state.Start();


        state.PlayerAction(2, ActionType.RaiseTo, 800);
        state.PlayerAction(3, ActionType.RaiseTo, 2_700);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(2, ActionType.RaiseTo, 11_000);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.DealBoard(0, ["4h", "8h", "3d"]);
        state.DealBoard(0, ["7c"]);
        state.DealBoard(0, ["Ts"]);

        state.ShowCards(0, ["Qs", "Qc"]);
        state.ShowCards(2, ["As", "9s"]);
        state.ShowCards(3, ["Qh", "9c"]);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Single(uncalled);
        Assert.Equal(2, uncalled[0].SeatId);
        Assert.Equal(1_000, uncalled[0].Amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([0, 2], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 219_627);
    }

    // Physical seats rotated to state seats: 7->state0, 1->state1, 2->state2, 3->state3, 5->state4, 6->state5
    [Fact]
    public void Hand1723277130_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([103_529, 27_622, 25_567, 22_935, 19_700, 36_916]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.PlayerPost(4, PostType.DeadBlind, 100);
        state.PlayerPost(4, PostType.ExtraBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.Check);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);
        state.DealBoard(0, ["8s", "6c", "6h"]);
        state.PlayerAction(0, ActionType.Bet, 500);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.Fold);
        state.DealBoard(0, ["3s"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(3, ActionType.Check);
        state.DealBoard(0, ["Kh"]);
        state.PlayerAction(0, ActionType.Bet, 1_575);
        state.PlayerAction(3, ActionType.RaiseTo, 3_307);
        state.PlayerAction(0, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Single(uncalled);
        Assert.Equal(3, uncalled[0].SeatId);
        Assert.Equal(1_732, uncalled[0].Amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([3], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 236_269);
    }

    // Physical seats rotated to state seats: 1->state0, 2->state1, 3->state2, 4->state3, 5->state4, 6->state5, 7->state6
    [Fact]
    public void Hand1723278433_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([27_422, 25_367, 25_515, 27_787, 19_400, 36_916, 101_254]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.PlayerPost(3, PostType.ExtraBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(3, ActionType.Check);
        state.PlayerAction(4, ActionType.Call);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);
        state.DealBoard(0, ["Td", "4s", "2s"]);
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(2, ActionType.Check);
        state.PlayerAction(3, ActionType.Bet, 1_200);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(6, ActionType.RaiseTo, 3_900);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Call);
        state.DealBoard(0, ["9s"]);
        state.PlayerAction(3, ActionType.Check);
        state.PlayerAction(6, ActionType.Bet, 4_500);
        state.PlayerAction(3, ActionType.Call);
        state.DealBoard(0, ["2h"]);
        state.PlayerAction(3, ActionType.Check);
        state.PlayerAction(6, ActionType.Bet, 9_000);
        state.PlayerAction(3, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Single(uncalled);
        Assert.Equal(6, uncalled[0].SeatId);
        Assert.Equal(9_000, uncalled[0].Amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([6], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 263_661);
    }

    // Physical seats rotated to state seats: 5->state0, 6->state1, 7->state2, 1->state3, 3->state4, 4->state5
    [Fact]
    public void Hand1723283401_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([20_000, 36_716, 135_644, 27_222, 10_000, 15_439]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.PlayerPost(4, PostType.ExtraBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.RaiseTo, 600);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Single(uncalled);
        Assert.Equal(2, uncalled[0].SeatId);
        Assert.Equal(400, uncalled[0].Amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([2], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 245_021);
    }

    // Physical seats rotated to state seats: 5->state0, 6->state1, 7->state2, 8->state3, 1->state4, 3->state5, 4->state6
    [Fact]
    public void Hand1723312078_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([30_592, 62_461, 108_612, 39_000, 22_615, 40_000, 11_512]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.PlayerPost(5, PostType.ExtraBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Check);
        state.PlayerAction(6, ActionType.RaiseTo, 800);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(5, ActionType.Fold);
        state.DealBoard(0, ["Kc", "Ts", "4d"]);
        state.PlayerAction(2, ActionType.Bet, 1_050);
        state.PlayerAction(6, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Single(uncalled);
        Assert.Equal(2, uncalled[0].SeatId);
        Assert.Equal(1_050, uncalled[0].Amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([2], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 314_792);
    }

    // Physical seats rotated to state seats: 7->state0, 8->state1, 1->state2, 2->state3, 3->state4, 4->state5, 5->state6, 6->state7
    [Fact]
    public void Hand1723314030_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([109_506, 39_000, 22_615, 23_400, 45_520, 4_762, 30_292, 62_061]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.PlayerPost(3, PostType.ExtraBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.RaiseTo, 700);
        state.PlayerAction(4, ActionType.Call);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(7, ActionType.Fold);
        state.PlayerAction(0, ActionType.RaiseTo, 1_850);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(3, ActionType.Call);
        state.PlayerAction(4, ActionType.Call);
        state.DealBoard(0, ["8s", "Qd", "Ad"]);
        state.PlayerAction(0, ActionType.Bet, 2_875);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Single(uncalled);
        Assert.Equal(0, uncalled[0].SeatId);
        Assert.Equal(2_875, uncalled[0].Amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([0], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 337_156);
    }

    // Physical seats rotated to state seats: 8->state0, 1->state1, 2->state2, 3->state3, 4->state4, 5->state5, 6->state6, 7->state7
    [Fact]
    public void Hand1723321464_ReplaysExactPlayersAndActions()
    {
        PokerState state = RealHandTestFactory.CreateState(maxRunoutCount: 1);

        state.Initialize([38_964, 18_115, 52_198, 23_863, 40_000, 29_492, 56_461, 112_681]);
        state.PlayerPost(0, PostType.SmallBlind, 100);
        state.PlayerPost(1, PostType.BigBlind, 200);
        state.PlayerPost(4, PostType.ExtraBlind, 200);
        state.Start();

        state.PlayerAction(2, ActionType.RaiseTo, 900);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(4, ActionType.Fold);
        state.PlayerAction(5, ActionType.Fold);
        state.PlayerAction(6, ActionType.Fold);
        state.PlayerAction(7, ActionType.RaiseTo, 2_625);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);
        state.PlayerAction(2, ActionType.Call);
        state.DealBoard(0, ["4s", "8d", "9c"]);
        state.PlayerAction(2, ActionType.Check);
        state.PlayerAction(7, ActionType.Bet, 2_875);
        state.PlayerAction(2, ActionType.Fold);

        UncalledBetReturnedEvent[] uncalled = state.Events.OfType<UncalledBetReturnedEvent>().ToArray();
        Assert.Single(uncalled);
        Assert.Equal(7, uncalled[0].SeatId);
        Assert.Equal(2_875, uncalled[0].Amount);

        int[] awardedSeats = state.Events
            .OfType<PotAwardedEvent>()
            .Select(award => award.SeatId)
            .Distinct()
            .Order()
            .ToArray();
        Assert.Equal([7], awardedSeats);

        RealHandTestFactory.AssertCompletedAndConserved(state, 371_774);
    }

}