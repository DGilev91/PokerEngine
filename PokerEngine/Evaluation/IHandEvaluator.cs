namespace PokerEngine.Evaluation;

public interface IHandEvaluator
{
    HandRank Evaluate(IReadOnlyList<string> holeCards, IReadOnlyList<string> boardCards);
}