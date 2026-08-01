using PokerEngine.Enums;
using PokerEngine.Models;
using PokerEngine.States;

namespace PokerEngine.Tests;

public sealed class PokerStateAutomationTests
{
    [Fact]
    public void DealHoleCards_StartDealsCardsToEveryPlayer()
    {
        PokerState state = PokerStateTestFactory.CreateState(automation: Automation.DealHoleCards);

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        Assert.All(state.Seats, seat => Assert.Equal(2, seat.HoleCards.Count));

        HoleCardsEvent[] events = state.Events.OfType<HoleCardsEvent>().ToArray();

        Assert.Equal(3, events.Length);
        Assert.Equal([0, 1, 2], events.Select(e => e.seatId).ToArray());

        string[] allCards = state.Seats.SelectMany(seat => seat.HoleCards).ToArray();

        Assert.Equal(6, allCards.Length);
        Assert.Equal(6, allCards.Distinct().Count());
    }

    [Fact]
    public void WithoutDealHoleCards_StartDoesNotDealCards()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        Assert.All(state.Seats, seat => Assert.Empty(seat.HoleCards));
        Assert.Empty(state.Events.OfType<HoleCardsEvent>());
    }

    [Fact]
    public void DealBoard_PreflopCompletionDealsFlop()
    {
        PokerState state = PokerStateTestFactory.CreateState(automation: Automation.DealBoard);

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        Assert.Equal(RoundType.Flop, state.Round);
        Assert.Equal(3, state.Boards[0].Count);

        BoardEvent boardEvent = Assert.Single(state.Events.OfType<BoardEvent>());

        Assert.Equal(RoundType.Flop, boardEvent.round);
        Assert.Equal(3, boardEvent.cards.Count);

        PlayerTurnEvent turn = Assert.IsType<PlayerTurnEvent>(state.Events.Last());

        Assert.Equal(0, turn.seatId);
    }

    [Fact]
    public void DealBoard_CheckingFlopDealsTurn()
    {
        PokerState state = PokerStateTestFactory.CreateState(automation: Automation.DealBoard);

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        PokerStateTestFactory.CheckAroundThreePlayers(state);

        Assert.Equal(RoundType.Turn, state.Round);
        Assert.Equal(4, state.Boards[0].Count);

        BoardEvent[] boardEvents = state.Events.OfType<BoardEvent>().ToArray();

        Assert.Equal(2, boardEvents.Length);
        Assert.Equal(RoundType.Flop, boardEvents[0].round);
        Assert.Equal(RoundType.Turn, boardEvents[1].round);
    }

    [Fact]
    public void DealBoard_DealsFlopTurnAndRiver()
    {
        PokerState state = PokerStateTestFactory.CreateState(automation: Automation.DealBoard);

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

        PokerStateTestFactory.CheckAroundThreePlayers(state);
        PokerStateTestFactory.CheckAroundThreePlayers(state);
        PokerStateTestFactory.CheckAroundThreePlayers(state);

        Assert.Equal(HandState.Completed, state.State);
        Assert.Equal(RoundType.Showdown, state.Round);
        Assert.Equal(5, state.Boards[0].Count);

        BoardEvent[] boardEvents = state.Events.OfType<BoardEvent>().ToArray();

        Assert.Equal(3, boardEvents.Length);
        Assert.Equal([RoundType.Flop, RoundType.Turn, RoundType.River], boardEvents.Select(e => e.round).ToArray());
        Assert.Equal([3, 1, 1], boardEvents.Select(e => e.cards.Count).ToArray());

        Assert.Equal(3_000, state.Seats.Sum(seat => seat.Stack));
    }

    [Fact]
    public void FullAutomation_DealsUniqueHoleAndBoardCards()
    {
        Automation automation = Automation.ShuffleDeck | Automation.DealHoleCards | Automation.DealBoard;
        PokerState state = PokerStateTestFactory.CreateState(automation: automation);

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        PokerStateTestFactory.CheckAroundThreePlayers(state);
        PokerStateTestFactory.CheckAroundThreePlayers(state);
        PokerStateTestFactory.CheckAroundThreePlayers(state);

        string[] holeCards = state.Seats.SelectMany(seat => seat.HoleCards).ToArray();
        string[] boardCards = state.Boards.SelectMany(board => board).ToArray();
        string[] allCards = holeCards.Concat(boardCards).ToArray();

        Assert.Equal(6, holeCards.Length);
        Assert.Equal(5, boardCards.Length);
        Assert.Equal(11, allCards.Length);
        Assert.Equal(11, allCards.Distinct().Count());

        Assert.Equal(3, state.Events.OfType<HoleCardsEvent>().Count());
        Assert.Equal(3, state.Events.OfType<BoardEvent>().Count());
        Assert.Equal(3, state.Events.OfType<HandEvaluatedEvent>().Count());

        Assert.Equal(HandState.Completed, state.State);
        Assert.IsType<EndHandEvent>(state.Events.Last());
    }

    [Fact]
    public void AllInWithRunoutOffer_WaitsForSelection()
    {
        PokerState state = PokerStateTestFactory.CreateState(maxRunoutCount: 2, automation: Automation.DealBoard);

        state.Initialize([500, 500]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.DealHole(0, ["As", "Ah"]);
        state.DealHole(1, ["Kc", "Kd"]);

        state.PlayerAction(0, ActionType.RaiseTo, 500);
        state.PlayerAction(1, ActionType.Call);

        Assert.Equal(HandState.Started, state.State);
        Assert.Equal(RoundType.Preflop, state.Round);
        Assert.Empty(state.Events.OfType<BoardEvent>());
        Assert.Empty(state.Boards[0]);
    }

    [Fact]
    public void OneRunout_DealsOneCompleteBoard()
    {
        PokerState state = PokerStateTestFactory.CreateState(maxRunoutCount: 2, automation: Automation.DealBoard);

        PrepareHeadsUpAllIn(state);

        state.SetRunoutCount(1);

        Assert.Equal(HandState.Completed, state.State);
        Assert.Single(state.Boards);
        Assert.Equal(5, state.Boards[0].Count);
        Assert.Equal(3, state.Events.OfType<BoardEvent>().Count());

        RunoutCountEvent runout = Assert.Single(state.Events.OfType<RunoutCountEvent>());

        Assert.Equal(1, runout.count);
    }

    [Fact]
    public void TwoRunouts_DealsTwoCompleteBoards()
    {
        PokerState state = PokerStateTestFactory.CreateState(maxRunoutCount: 2, automation: Automation.DealBoard);

        PrepareHeadsUpAllIn(state);

        state.SetRunoutCount(2);

        Assert.Equal(HandState.Completed, state.State);
        Assert.Equal(2, state.Boards.Count);
        Assert.All(state.Boards, board => Assert.Equal(5, board.Count));

        BoardEvent[] boardEvents = state.Events.OfType<BoardEvent>().ToArray();

        Assert.Equal(6, boardEvents.Length);
        Assert.Equal(3, boardEvents.Count(e => e.boardIndex == 0));
        Assert.Equal(3, boardEvents.Count(e => e.boardIndex == 1));

        string[] cards = state.Boards.SelectMany(board => board).ToArray();

        Assert.Equal(10, cards.Length);
        Assert.Equal(10, cards.Distinct().Count());

        long awarded = state.Events.OfType<PotAwardedEvent>().Sum(e => e.amount);

        Assert.Equal(1_000, awarded);
        Assert.Equal(1_000, state.Seats.Sum(seat => seat.Stack));
    }

    [Fact]
    public void RunoutSelectionCannotBeSetTwice()
    {
        PokerState state = PokerStateTestFactory.CreateState(maxRunoutCount: 2, automation: Automation.DealBoard);

        PrepareHeadsUpAllIn(state);

        state.SetRunoutCount(2);

        Assert.Throws<InvalidOperationException>(() => state.SetRunoutCount(1));
    }

    [Fact]
    public void TwoRunoutsFromFlop_CopyFlopAndDealSeparateTurnAndRiver()
    {
        PokerState state = PokerStateTestFactory.CreateState(maxRunoutCount: 2, automation: Automation.DealBoard);

        state.Initialize([1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.DealHole(0, ["As", "Ah"]);
        state.DealHole(1, ["Kc", "Kd"]);

        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        Assert.Equal(RoundType.Flop, state.Round);
        Assert.Equal(3, state.Boards[0].Count);

        string[] flop = state.Boards[0].ToArray();

        state.PlayerAction(1, ActionType.Bet, 900);
        state.PlayerAction(0, ActionType.Call);

        Assert.Equal(HandState.Started, state.State);
        Assert.Single(state.Boards);
        Assert.Equal(3, state.Boards[0].Count);

        state.SetRunoutCount(2);

        Assert.Equal(HandState.Completed, state.State);
        Assert.Equal(2, state.Boards.Count);
        Assert.All(state.Boards, board => Assert.Equal(5, board.Count));

        Assert.Equal(flop, state.Boards[0].Take(3));
        Assert.Equal(flop, state.Boards[1].Take(3));

        Assert.NotEqual(state.Boards[0][3], state.Boards[1][3]);
        Assert.NotEqual(state.Boards[0][4], state.Boards[1][4]);

        BoardEvent[] boardEvents = state.Events.OfType<BoardEvent>().ToArray();

        Assert.Equal(1, boardEvents.Count(e => e.round == RoundType.Flop));
        Assert.Equal(2, boardEvents.Count(e => e.round == RoundType.Turn));
        Assert.Equal(2, boardEvents.Count(e => e.round == RoundType.River));

        Assert.Equal(4, state.Events.OfType<HandEvaluatedEvent>().Count());
        Assert.Single(state.Events.OfType<EndHandEvent>());

        Assert.Equal(2_000, state.Events.OfType<PotAwardedEvent>().Sum(e => e.amount));
        Assert.Equal(2_000, state.Seats.Sum(seat => seat.Stack));
    }

    [Fact]
    public void TwoRunoutsFromTurn_CopySharedBoardAndDealTwoSeparateRivers()
    {
        PokerState state = PokerStateTestFactory.CreateState(maxRunoutCount: 2, automation: Automation.DealBoard);

        state.Initialize([1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.DealHole(0, ["As", "Ah"]);
        state.DealHole(1, ["Kc", "Kd"]);

        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        Assert.Equal(RoundType.Flop, state.Round);
        Assert.Equal(3, state.Boards[0].Count);

        PokerStateTestFactory.CheckAroundHeadsUp(state);

        Assert.Equal(RoundType.Turn, state.Round);
        Assert.Equal(4, state.Boards[0].Count);

        string[] sharedBoard = state.Boards[0].ToArray();

        state.PlayerAction(1, ActionType.Bet, 900);
        state.PlayerAction(0, ActionType.Call);

        Assert.Equal(HandState.Started, state.State);
        Assert.Single(state.Boards);
        Assert.Equal(4, state.Boards[0].Count);

        state.SetRunoutCount(2);

        Assert.Equal(HandState.Completed, state.State);
        Assert.Equal(2, state.Boards.Count);
        Assert.All(state.Boards, board => Assert.Equal(5, board.Count));

        Assert.Equal(sharedBoard, state.Boards[0].Take(4));
        Assert.Equal(sharedBoard, state.Boards[1].Take(4));

        Assert.NotEqual(state.Boards[0][4], state.Boards[1][4]);

        BoardEvent[] boardEvents = state.Events.OfType<BoardEvent>().ToArray();

        Assert.Single(boardEvents.Where(e => e.round == RoundType.Flop));
        Assert.Single(boardEvents.Where(e => e.round == RoundType.Turn));
        Assert.Equal(2, boardEvents.Count(e => e.round == RoundType.River));

        Assert.Contains(boardEvents, e => e.round == RoundType.River && e.boardIndex == 0);
        Assert.Contains(boardEvents, e => e.round == RoundType.River && e.boardIndex == 1);

        Assert.Equal(4, state.Events.OfType<HandEvaluatedEvent>().Count());
        Assert.Single(state.Events.OfType<EndHandEvent>());

        Assert.Equal(2_000, state.Events.OfType<PotAwardedEvent>().Sum(e => e.amount));
        Assert.Equal(2_000, state.Seats.Sum(seat => seat.Stack));
    }

    [Fact]
    public void WithoutDealBoard_DealWithoutCardsIsRejected()
    {
        PokerState state = PokerStateTestFactory.CreateState();

        state.Initialize([1_000, 1_000, 1_000]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.PlayerAction(2, ActionType.Call);
        state.PlayerAction(0, ActionType.Call);
        state.PlayerAction(1, ActionType.Check);

        Assert.Throws<InvalidOperationException>(() => state.DealBoard());
    }

    private static void PrepareHeadsUpAllIn(PokerState state)
    {
        state.Initialize([500, 500]);
        state.PlayerPost(0, PostType.SmallBlind, 50);
        state.PlayerPost(1, PostType.BigBlind, 100);
        state.Start();

        state.DealHole(0, ["As", "Ah"]);
        state.DealHole(1, ["Kc", "Kd"]);

        state.PlayerAction(0, ActionType.RaiseTo, 500);
        state.PlayerAction(1, ActionType.Call);
    }
}