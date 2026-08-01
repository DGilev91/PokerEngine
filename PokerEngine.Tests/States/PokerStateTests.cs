using PokerEngine.Enums;
using PokerEngine.Models;
using PokerEngine.States;

namespace PokerEngine.Tests;

public sealed class PokerStateTests
{
    [Fact]
    public void EveryoneFoldsToBigBlind_BigBlindWinsEntirePotWithoutUncalled()
    {
        PokerState state = CreateState();

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
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.PlayerAction(2, ActionType.RaiseTo, 300);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);

        Assert.Equal(HandState.Completed, state.State);

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
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.PlayerPost(2, PostType.DeadBlind, 50);
        state.Start();

        state.PlayerAction(2, ActionType.Fold);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);

        Assert.Equal(HandState.Completed, state.State);
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
        PokerState state = CreateState();

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
        PokerState state = CreateState();

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
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.PlayerPost(2, PostType.ExtraBlind, 100);
        state.Start();

        state.PlayerAction(2, ActionType.Check);
        state.PlayerAction(3, ActionType.Fold);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);

        Assert.Equal(HandState.Completed, state.State);
        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());

        PotAwardedEvent awarded = Assert.Single(state.Events.OfType<PotAwardedEvent>());

        Assert.Equal(2, awarded.seatId);
        Assert.Equal(250, awarded.amount);

        Assert.Equal(950, state.Seats[0].Stack);
        Assert.Equal(900, state.Seats[1].Stack);
        Assert.Equal(1_150, state.Seats[2].Stack);
        Assert.Equal(1_000, state.Seats[3].Stack);
    }

    [Fact]
    public void DeadAndExtraBlind_AreHandledSeparately()
    {
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.PlayerPost(2, PostType.DeadBlind, 50);
        state.PlayerPost(2, PostType.ExtraBlind, 100);
        state.Start();

        Assert.Equal(150, state.Seats[2].TotalBet);
        Assert.Equal(100, state.Seats[2].RoundBet);

        PlayerTurnEvent turn = Assert.IsType<PlayerTurnEvent>(state.Events.Last());

        Assert.Equal(2, turn.seatId);
        Assert.Equal(0, turn.callAmount);

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
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        Assert.Equal(RoundType.Flop, state.Round);

        Assert.Equal(900, state.Seats[0].Stack);
        Assert.Equal(900, state.Seats[1].Stack);
        Assert.Equal(900, state.Seats[2].Stack);

        Assert.Equal(100, state.Seats[0].TotalBet);
        Assert.Equal(100, state.Seats[1].TotalBet);
        Assert.Equal(100, state.Seats[2].TotalBet);

        Assert.Equal(0, state.Seats[0].RoundBet);
        Assert.Equal(0, state.Seats[1].RoundBet);
        Assert.Equal(0, state.Seats[2].RoundBet);

        Assert.Empty(state.Events.OfType<UncalledBetReturnedEvent>());
    }

    [Fact]
    public void DealFlop_StartsPostflopActionFromSmallBlind()
    {
        PokerState state = CreateState();

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

        BoardEvent boardEvent = state.Events.OfType<BoardEvent>().Last();

        Assert.Equal(RoundType.Flop, boardEvent.round);
        Assert.Equal(0, boardEvent.boardIndex);
        Assert.Equal(["2s", "7h", "Jc"], boardEvent.cards);

        PlayerTurnEvent turn = Assert.IsType<PlayerTurnEvent>(state.Events.Last());

        Assert.Equal(0, turn.seatId);
        Assert.Equal(0, turn.callAmount);
        Assert.Contains(ActionType.Check, turn.actions);
        Assert.Contains(ActionType.Bet, turn.actions);
    }

    [Fact]
    public void PlayerCannotActOutOfTurn()
    {
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => state.PlayerAction(0, ActionType.Call));

        Assert.Contains("Сейчас ход игрока", exception.Message);
    }

    [Fact]
    public void PlayerCannotCheckWhenFacingBet()
    {
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        Assert.Throws<InvalidOperationException>(() => state.PlayerAction(2, ActionType.Check));
    }

    [Fact]
    public void PlayerCannotBetWhenBlindAlreadyExists()
    {
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        Assert.Throws<InvalidOperationException>(() => state.PlayerAction(2, ActionType.Bet, 200));
    }

    [Fact]
    public void RaiseBelowMinimumIsRejected()
    {
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        Assert.Throws<InvalidOperationException>(() => state.PlayerAction(2, ActionType.RaiseTo, 150));
    }

    [Fact]
    public void MinimumPreflopRaiseToIsTwoBigBlinds()
    {
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        PlayerTurnEvent turn = Assert.IsType<PlayerTurnEvent>(state.Events.Last());

        Assert.Equal(200, turn.minRaiseTo);
        Assert.Equal(1_000, turn.maxRaiseTo);
    }

    [Fact]
    public void FullHand_ShowdownAwardsPotToBestHand()
    {
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.DealHole(0, ["As", "Ah"]);
        state.DealHole(1, ["Kc", "Kd"]);
        state.DealHole(2, ["Qc", "Qd"]);

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        state.DealBoard(0, ["2c", "3d", "4h"]);

        CheckAroundThreePlayers(state);

        state.DealBoard(0, ["5s"]);

        CheckAroundThreePlayers(state);

        state.DealBoard(0, ["9c"]);

        CheckAroundThreePlayers(state);

        Assert.Equal(HandState.Completed, state.State);
        Assert.Equal(RoundType.Showdown, state.Round);

        Assert.Equal(1_200, state.Seats[0].Stack);
        Assert.Equal(900, state.Seats[1].Stack);
        Assert.Equal(900, state.Seats[2].Stack);

        PotAwardedEvent awarded = Assert.Single(state.Events.OfType<PotAwardedEvent>());

        Assert.Equal(0, awarded.seatId);
        Assert.Equal(300, awarded.amount);

        Assert.Equal(3, state.Events.OfType<HandEvaluatedEvent>().Count());
        Assert.IsType<EndHandEvent>(state.Events.Last());
    }

    [Fact]
    public void AllInWithDifferentStacks_CreatesMainAndSidePot()
    {
        PokerState state = CreateState();

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

        Assert.Equal(HandState.Completed, state.State);

        PotAwardedEvent[] awards = state.Events.OfType<PotAwardedEvent>().ToArray();

        Assert.Equal(2, awards.Length);

        PotAwardedEvent mainPot = Assert.Single(awards.Where(result => result.potIndex == 0));
        PotAwardedEvent sidePot = Assert.Single(awards.Where(result => result.potIndex == 1));

        Assert.Equal(0, mainPot.seatId);
        Assert.Equal(300, mainPot.amount);

        Assert.Equal(1, sidePot.seatId);
        Assert.Equal(400, sidePot.amount);

        Assert.Equal(300, state.Seats[0].Stack);
        Assert.Equal(400, state.Seats[1].Stack);
        Assert.Equal(200, state.Seats[2].Stack);

        Assert.Equal(900, state.Seats.Sum(seat => seat.Stack));
    }

    [Fact]
    public void TieOnBoard_SplitsPotEqually()
    {
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.DealHole(0, ["2c", "3d"]);
        state.DealHole(1, ["4c", "5d"]);

        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        state.DealBoard(0, ["As", "Ks", "Qs"]);

        CheckAroundHeadsUp(state);

        state.DealBoard(0, ["Js"]);

        CheckAroundHeadsUp(state);

        state.DealBoard(0, ["Ts"]);

        CheckAroundHeadsUp(state);

        Assert.Equal(HandState.Completed, state.State);

        PotAwardedEvent[] awards = state.Events.OfType<PotAwardedEvent>().ToArray();

        Assert.Equal(2, awards.Length);
        Assert.All(awards, award => Assert.Equal(100, award.amount));

        Assert.Contains(awards, award => award.seatId == 0);
        Assert.Contains(awards, award => award.seatId == 1);

        Assert.Equal(1_000, state.Seats[0].Stack);
        Assert.Equal(1_000, state.Seats[1].Stack);
    }

    [Fact]
    public void ShortBigBlind_PostsOnlyAvailableStack()
    {
        PokerState state = CreateState();

        state.Initialize([1_000, 30, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);

        PlayerPostedEvent bigBlind = state.Events.OfType<PlayerPostedEvent>().Last();

        Assert.Equal(1, bigBlind.seatId);
        Assert.Equal(PostType.BigBlind, bigBlind.postType);
        Assert.Equal(30, bigBlind.amount);
        Assert.True(bigBlind.isAllIn);

        Assert.Equal(0, state.Seats[1].Stack);
        Assert.Equal(30, state.Seats[1].RoundBet);
        Assert.Equal(30, state.Seats[1].TotalBet);
    }

    [Fact]
    public void SetRunoutCountOutsideAllInSituation_RejectsMultipleRunouts()
    {
        PokerState state = CreateState(maxRunoutCount: 2);

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        Assert.Throws<InvalidOperationException>(() => state.SetRunoutCount(2));
    }

    [Fact]
    public void SetRunoutCountAboveMaximumIsRejected()
    {
        PokerState state = CreateState(maxRunoutCount: 2);

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        Assert.Throws<ArgumentOutOfRangeException>(() => state.SetRunoutCount(3));
    }

    [Fact]
    public void DealBoardWithWrongCardCountIsRejected()
    {
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        Assert.Throws<ArgumentException>(() => state.DealBoard(0, ["As", "Ks"]));
    }

    [Fact]
    public void DealHoleWithWrongCardCountIsRejected()
    {
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        Assert.Throws<ArgumentException>(() => state.DealHole(0, ["As"]));
    }

    [Fact]
    public void PlayerCannotReceiveHoleCardsTwice()
    {
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.DealHole(0, ["As", "Kh"]);

        Assert.Throws<InvalidOperationException>(() => state.DealHole(0, ["Qc", "Qh"]));
    }

    [Fact]
    public void TotalChipsAreConservedAfterCompletedHand()
    {
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.PlayerAction(2, ActionType.RaiseTo, 300);
        state.PlayerAction(0, ActionType.Fold);
        state.PlayerAction(1, ActionType.Fold);

        Assert.Equal(3_000, state.Seats.Sum(seat => seat.Stack));
    }

    [Fact]
    public void EventsBeginWithNewHandAndSeats()
    {
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000, 1_000]);

        Assert.IsType<NewHandEvent>(state.Events[0]);

        SeatsEvent seatsEvent = Assert.IsType<SeatsEvent>(state.Events[1]);

        Assert.Equal([1_000, 1_000, 1_000], seatsEvent.stacks);
    }

    [Fact]
    public void StartCreatesFirstPlayerTurnForButtonInThreeHandedGame()
    {
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        PlayerTurnEvent turn = Assert.IsType<PlayerTurnEvent>(state.Events.Last());

        Assert.Equal(2, turn.seatId);
        Assert.Equal(100, turn.callAmount);
        Assert.Equal(200, turn.minRaiseTo);

        Assert.Contains(ActionType.Fold, turn.actions);
        Assert.Contains(ActionType.Call, turn.actions);
        Assert.Contains(ActionType.RaiseTo, turn.actions);
    }

    [Fact]
    public void HeadsUpSmallBlindActsFirstPreflop()
    {
        PokerState state = CreateState();

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
        PokerState state = CreateState();

        state.Initialize([1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        state.DealBoard(0, ["2s", "7h", "Jc"]);

        PlayerTurnEvent turn = Assert.IsType<PlayerTurnEvent>(state.Events.Last());

        Assert.Equal(1, turn.seatId);
        Assert.Equal(0, turn.callAmount);
    }

    private static void CheckAroundThreePlayers(PokerState state)
    {
        state.PlayerAction(0, ActionType.Check);
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(2, ActionType.Check);
    }

    private static void CheckAroundHeadsUp(PokerState state)
    {
        state.PlayerAction(1, ActionType.Check);
        state.PlayerAction(0, ActionType.Check);
    }

    private static PokerState CreateState(int maxRunoutCount = 1)
    {
        return new PokerState(CreateClassicRules(maxRunoutCount));
    }

    private static PokerRules CreateClassicRules(int maxRunoutCount = 1)
    {
        return new PokerRules
        {
            Automation = Automation.None,
            GameType = GameType.TexasHoldem,
            BettingType = BettingType.NoLimit,
            SmallBlind = 50,
            BigBlind = 100,
            Ante = null,
            Straddle = null,
            InitialBoardCount = 1,
            MaxRunoutCount = maxRunoutCount,
            Rounds =
            [
                new RoundRules
                {
                    Type = RoundType.Preflop,
                    BoardCardCount = 0,
                    BetSize = 100,
                    MaxRaises = null
                },
                new RoundRules
                {
                    Type = RoundType.Flop,
                    BoardCardCount = 3,
                    BetSize = 100,
                    MaxRaises = null
                },
                new RoundRules
                {
                    Type = RoundType.Turn,
                    BoardCardCount = 1,
                    BetSize = 100,
                    MaxRaises = null
                },
                new RoundRules
                {
                    Type = RoundType.River,
                    BoardCardCount = 1,
                    BetSize = 100,
                    MaxRaises = null
                }
            ]
        };
    }
}