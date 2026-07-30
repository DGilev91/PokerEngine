using PokerEngine.Interfaces;
using PokerEngine.Models;
using PokerEngine.States;

namespace PokerEngine.Games;

public abstract class PokerGame : IPokerGame
{
   public abstract IPokerState CreateState();
}