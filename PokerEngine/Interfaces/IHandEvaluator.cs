using PokerEngine.Models;

namespace PokerEngine.Interfaces;

public interface IHandEvaluator
{
    HandRank Evaluate(IReadOnlyList<string> holeCards, IReadOnlyList<string> boardCards);
}