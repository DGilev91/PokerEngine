using PokerEngine.Interfaces;

namespace PokerEngine.Games;

public abstract class PokerGame : IPokerGame
{
   public abstract IPokerState CreateState();
}