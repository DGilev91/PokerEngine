using PokerEngine.Enums;
using PokerEngine.Models;
using PokerEngine.States;

namespace PokerEngine.Tests;

public sealed class PokerStateManualTests
{
    [Fact]
    public void EveryoneFoldsToBigBlind_BigBlindWinsEntirePotWithoutUncalled()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);

        Assert.Equal(HandState.Completed, state.State);
        Assert.Equal(950, state.Seats[0].Stack);
        Assert.Equal(1_050, state.Seats[1].Stack);
        Assert.Equal(1_000, state.Seats[2].Stack);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        PotAwardedEvent awarded = Assert.Single(state.Events.OfType<PotAwardedEvent>());

        Assert.Equal(0, awarded.potIndex);
        Assert.Equal(0, awarded.boardIndex);
        Assert.Equal(1, awarded.seatId);
        Assert.Equal(150, awarded.amount);

        Assert.IsType<EndHandEvent>(state.Events.Last());
    }

    [Fact]
    public void EveryoneFoldsToRaise_ReturnsOnlyUnmatchedRaisePart()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.PlayerAction(2, ActionType.RaiseTo, 300);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);

        UncalledBetReturnedEvent uncalled = Assert.Single(state.Events.OfType<UncalledBetReturnedEvent>());

        Assert.Equal(2, uncalled.seatId);
        Assert.Equal(200, uncalled.amount);

        PotAwardedEvent awarded = Assert.Single(state.Events.OfType<PotAwardedEvent>());

        Assert.Equal(2, awarded.seatId);
        Assert.Equal(250, awarded.amount);

        Assert.Equal(950, state.Seats[0].Stack);
        Assert.Equal(900, state.Seats[1].Stack);
        Assert.Equal(1_150, state.Seats[2].Stack);
    }

    [Fact]
    public void DeadBlind_RemainsInPotAndDoesNotCreateUncalled()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.PlayerPost(2, PostType.DeadBlind, 50);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        PotAwardedEvent awarded = Assert.Single(state.Events.OfType<PotAwardedEvent>());

        Assert.Equal(1, awarded.seatId);
        Assert.Equal(200, awarded.amount);

        Assert.Equal(950, state.Seats[0].Stack);
        Assert.Equal(1_100, state.Seats[1].Stack);
        Assert.Equal(950, state.Seats[2].Stack);
        Assert.Equal(1_000, state.Seats[3].Stack);
    }

    [Fact]
    public void DeadBlind_DoesNotReduceCallAmount()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.PlayerPost(2, PostType.DeadBlind, 50);
        state.Start();

        PlayerTurnEvent turn = Assert.IsType<PlayerTurnEvent>(state.Events.Last());

        Assert.Equal(2, turn.seatId);
        Assert.Equal(100, turn.callAmount);
        Assert.Contains(ActionType.Fold, turn.actions);
        Assert.Contains(ActionType.Call, turn.actions);
        Assert.Contains(ActionType.RaiseTo, turn.actions);
    }

    [Fact]
    public void ExtraBlind_IsLiveAndDoesNotRequireCall()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.PlayerPost(2, PostType.ExtraBlind, 100);
        state.Start();

        PlayerTurnEvent turn = Assert.IsType<PlayerTurnEvent>(state.Events.Last());

        Assert.Equal(2, turn.seatId);
        Assert.Equal(0, turn.callAmount);
        Assert.Contains(ActionType.Check, turn.actions);
        Assert.Contains(ActionType.RaiseTo, turn.actions);
        Assert.DoesNotContain(ActionType.Call, turn.actions);
    }

    [Fact]
    public void EveryoneFoldsToExtraBlind_ExtraBlindWinsEntirePotWithoutUncalled()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.PlayerPost(2, PostType.ExtraBlind, 100);
        state.Start();

        state.PlayerAction(2, ActionType.Check);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        PotAwardedEvent awarded = Assert.Single(state.Events.OfType<PotAwardedEvent>());

        Assert.Equal(2, awarded.seatId);
        Assert.Equal(250, awarded.amount);
        Assert.Equal(1_150, state.Seats[2].Stack);
    }

    [Fact]
    public void DeadAndExtraBlind_AreHandledSeparately()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.PlayerPost(2, PostType.DeadBlind, 50);
        state.PlayerPost(2, PostType.ExtraBlind, 100);
        state.Start();

        Assert.Equal(150, state.Seats[2].TotalBet);
        Assert.Equal(100, state.Seats[2].RoundBet);

        state.PlayerAction(2, ActionType.Check);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        PotAwardedEvent awarded = Assert.Single(state.Events.OfType<PotAwardedEvent>());

        Assert.Equal(2, awarded.seatId);
        Assert.Equal(300, awarded.amount);
        Assert.Equal(1_150, state.Seats[2].Stack);
    }

    [Fact]
    public void CallsAndBigBlindCheck_CompletePreflop()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        Assert.Equal(RoundType.Flop, state.Round);

        Assert.All(state.Seats, seat => Assert.Equal(100, seat.TotalBet));
        Assert.All(state.Seats, seat => Assert.Equal(0, seat.RoundBet));
        Assert.All(state.Seats, seat => Assert.Equal(900, seat.Stack));

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());
    }

    [Fact]
    public void DealFlop_StartsPostflopActionFromSmallBlind()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        state.DealBoard(0, ["2s", "7h", "Jc"]);

        Assert.Equal(RoundType.Flop, state.Round);
        Assert.Equal(["2s", "7h", "Jc"], state.Boards[0]);

        PlayerTurnEvent turn = Assert.IsType<PlayerTurnEvent>(state.Events.Last());

        Assert.Equal(0, turn.seatId);
        Assert.Equal(0, turn.callAmount);
        Assert.Contains(ActionType.Check, turn.actions);
        Assert.Contains(ActionType.Bet, turn.actions);
    }

    [Fact]
    public void PlayerCannotActOutOfTurn()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        Assert.Throws<InvalidOperationException>(() => state.PlayerAction(0, ActionType.Call));
    }

    [Fact]
    public void PlayerCannotCheckWhenFacingBet()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        Assert.Throws<InvalidOperationException>(() => state.PlayerAction(2, ActionType.Check));
    }

    [Fact]
    public void RaiseBelowMinimumIsRejected()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        Assert.Throws<InvalidOperationException>(() => state.PlayerAction(2, ActionType.RaiseTo, 150));
    }

    [Fact]
    public void MinimumPreflopRaiseToIsTwoBigBlinds()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        PlayerTurnEvent turn = Assert.IsType<PlayerTurnEvent>(state.Events.Last());

        Assert.Equal(200, turn.minRaiseTo);
        Assert.Equal(1_000, turn.maxRaiseTo);
    }

    [Fact]
    public void AllInWithDifferentStacks_CreatesMainAndSidePot()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([100, 300, 500]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.DealHole(0, ["As", "5s"]);
        state.DealHole(1, ["Kc", "Kd"]);
        state.DealHole(2, ["Qc", "Qd"]);

        state.PlayerAction(2, ActionType.RaiseTo, 500);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Call);

        UncalledBetReturnedEvent uncalled = Assert.Single(state.Events.OfType<UncalledBetReturnedEvent>());

        Assert.Equal(2, uncalled.seatId);
        Assert.Equal(200, uncalled.amount);

        state.DealBoard(0, ["2h", "3d", "4c"]);
        state.DealBoard(0, ["9s"]);
        state.DealBoard(0, ["Jh"]);

        PotAwardedEvent[] awards = state.Events.OfType<PotAwardedEvent>().ToArray();

        PotAwardedEvent mainPot = Assert.Single(awards.Where(result => result.potIndex == 0));
        PotAwardedEvent sidePot = Assert.Single(awards.Where(result => result.potIndex == 1));

        Assert.Equal(0, mainPot.seatId);
        Assert.Equal(300, mainPot.amount);

        Assert.Equal(1, sidePot.seatId);
        Assert.Equal(400, sidePot.amount);

        Assert.Equal(900, state.Seats.Sum(seat => seat.Stack));
    }

    [Fact]
    public void TieOnBoard_SplitsPotEqually()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.DealHole(0, ["2c", "3d"]);
        state.DealHole(1, ["4c", "5d"]);

        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        state.DealBoard(0, ["As", "Ks", "Qs"]);

        PokerStateTestFactory.CheckAroundHeadsUp(state);

        state.DealBoard(0, ["Js"]);

        PokerStateTestFactory.CheckAroundHeadsUp(state);

        state.DealBoard(0, ["Ts"]);

        PokerStateTestFactory.CheckAroundHeadsUp(state);

        PotAwardedEvent[] awards = state.Events.OfType<PotAwardedEvent>().ToArray();

        Assert.Equal(2, awards.Length);
        Assert.All(awards, award => Assert.Equal(100, award.amount));

        Assert.Equal(1_000, state.Seats[0].Stack);
        Assert.Equal(1_000, state.Seats[1].Stack);
    }

    [Fact]
    public void HeadsUpSmallBlindActsFirstPreflop()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        PlayerTurnEvent turn = Assert.IsType<PlayerTurnEvent>(state.Events.Last());

        Assert.Equal(0, turn.seatId);
        Assert.Equal(50, turn.callAmount);
    }

    [Fact]
    public void HeadsUpBigBlindActsFirstPostflop()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        state.DealBoard(0, ["2s", "7h", "Jc"]);

        PlayerTurnEvent turn = Assert.IsType<PlayerTurnEvent>(state.Events.Last());

        Assert.Equal(1, turn.seatId);
    }
}