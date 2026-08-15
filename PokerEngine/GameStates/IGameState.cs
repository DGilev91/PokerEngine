using PokerEngine.Enums;
using PokerEngine.Models;

namespace PokerEngine.GameStates;

public interface IGameState
{
    GameSetup Setup { get; }
    
    IReadOnlyList<GameEvent> Events { get; }

    IReadOnlyList<Player> Players { get; }

    IReadOnlyList<Pot> Pots { get; }

    IReadOnlyList<IReadOnlyList<Card>> Boards { get; }

    IReadOnlyList<PlayerHand> Hands { get; }

    IReadOnlyList<Award> Awards { get; }

    int? RoundIndex { get; }

    GameStep Step { get; }

    PlayerAction? Action { get; }

    void Initialize(GameSetup setup);

    void DealHole(int seatIndex, IReadOnlyList<string> cards);

    void Act(int seatIndex, ActionType type, long amount = 0);

    void ReturnUncalledBet();

    void CollectBets();

    void DealBoard(IReadOnlyList<string> cards, int boardIndex = 0);

    void ShowHole(int seatIndex, IReadOnlyList<string> cards);

    void Muck(int seatIndex);

    void EvaluateHands();

    void AwardPots();
}