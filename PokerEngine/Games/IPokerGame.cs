using PokerEngine.Hand;

namespace PokerEngine.Games;


public interface IPokerGame
{
    IPokerHand CreateHand();
}