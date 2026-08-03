using PokerEngine.Enums;
using PokerEngine.Games;
using PokerEngine.States;
using PokerEngine.States.Events;
using PokerEngine.Tests.States;

namespace PokerEngine.Tests.Games
{
    public class NoLimitTexasHoldem_Test
    {
        [Fact]
        public void Test_1()
        {
            IPokerGame game = new NoLimitTexasHoldem(
                automation: Automation.None,
                smallBlind: 100,
                bigBlind: 200);

            IPokerState state = game.CreateState();

            state.Initialize([10_000, 10_000, 10_000]);

            state.PlayerPost(0, PostType.SmallBlind, 100);
            state.PlayerPost(1, PostType.BigBlind, 200);
            state.Start();

            state.DealHole(0, ["As", "Ad"]);
            state.DealHole(1, ["Kh", "Qh"]);
            state.DealHole(2, ["7c", "7d"]);

            state.PlayerAction(2, ActionType.RaiseTo, 600);
            state.PlayerAction(0, ActionType.Fold);
            state.PlayerAction(1, ActionType.Fold);

            Assert.Equal(HandState.Completed, state.State);

            Assert.IsType<EndHandEvent>(state.Events.Last());
        }

        [Fact]
        public void Test_2()
        {
            IPokerGame game = new NoLimitTexasHoldem(
                automation: Automation.None,
                smallBlind: 100,
                bigBlind: 200);

            IPokerState state = game.CreateState();

            state.Initialize([10_000, 10_000, 10_000]);

            state.PlayerPost(0, PostType.SmallBlind, 100);
            state.PlayerPost(1, PostType.BigBlind, 200);
            state.Start();

            state.DealHole(0, ["As", "Ad"]);
            state.DealHole(1, ["Kh", "Qh"]);
            state.DealHole(2, ["7c", "7d"]);

            state.PlayerAction(2, ActionType.RaiseTo, 600);
            state.PlayerAction(0, ActionType.Call);
            state.PlayerAction(1, ActionType.Fold);

            state.DealBoard(0, ["Qs", "Jd", "2c"]);

            state.PlayerAction(0, ActionType.Check);
            state.PlayerAction(2, ActionType.Check);

            state.DealBoard(0, ["Tc"]);

            state.PlayerAction(0, ActionType.Check);
            state.PlayerAction(2, ActionType.Check);

            state.DealBoard(0, ["8c"]);

            state.PlayerAction(0, ActionType.Check);
            state.PlayerAction(2, ActionType.Check);

            Assert.Equal(HandState.Completed, state.State);

            Assert.IsType<EndHandEvent>(state.Events.Last());
        }

        [Fact]
        public void Test_3()
        {
            IPokerGame game = new NoLimitTexasHoldem(
                automation: Automation.None,
                smallBlind: 100,
                bigBlind: 200);

            IPokerState state = game.CreateState();

            state.Initialize([10_000, 10_000, 10_000]);

            state.PlayerPost(0, PostType.SmallBlind, 100);
            state.PlayerPost(1, PostType.BigBlind, 200);
            state.Start();

            //state.DealHole(0, ["As", "Ad"]);
            state.DealHole(1, ["Kh", "Qh"]);
            //state.DealHole(2, ["7c", "7d"]);

            state.PlayerAction(2, ActionType.RaiseTo, 600);
            state.PlayerAction(0, ActionType.Call);
            state.PlayerAction(1, ActionType.Fold);

            state.DealBoard(0, ["Qs", "Jd", "2c"]);

            state.PlayerAction(0, ActionType.Check);
            state.PlayerAction(2, ActionType.Check);

            state.DealBoard(0, ["Tc"]);

            state.PlayerAction(0, ActionType.Check);
            state.PlayerAction(2, ActionType.Check);

            state.DealBoard(0, ["8c"]);

            state.PlayerAction(0, ActionType.Bet, 9_400);
            state.PlayerAction(2, ActionType.Call);

            state.ShowCards(0, ["As", "Ad"]);
            state.ShowCards(2, ["7c", "7d"]);

            Assert.Equal(HandState.Completed, state.State);

            Assert.IsType<EndHandEvent>(state.Events.Last());
        }
    }
}
