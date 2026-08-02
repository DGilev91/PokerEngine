using PokerEngine.Enums;

namespace PokerEngine.Rules;

public sealed class PokerRules
{
    /// <summary>
    /// Defines which parts of the hand are performed automatically.
    /// <see cref="Automation.None"/> enables fully manual mode.
    /// <see cref="Automation.All"/> enables all available automation.
    /// </summary>
    public required Automation Automation { get; init; } =
        Automation.None;

    /// <summary>
    /// Gets the poker game type, such as Texas Hold'em or Omaha.
    /// </summary>
    public required GameType GameType { get; init; }

    /// <summary>
    /// Gets the betting structure, such as fixed limit,
    /// pot limit, or no limit.
    /// </summary>
    public required GameLimit GameLimit { get; init; }

    /// <summary>
    /// Gets the ordered sequence of betting rounds,
    /// such as preflop, flop, turn, and river.
    /// </summary>
    public required IReadOnlyList<RoundRules> Rounds { get; init; }

    /// <summary>
    /// Gets the required small blind amount.
    /// The value must be greater than zero and lower than the big blind.
    /// </summary>
    public required long SmallBlind { get; init; }

    /// <summary>
    /// Gets the required big blind amount.
    /// It commonly also defines the minimum full bet size
    /// in games that use the same minimum bet on every round.
    /// </summary>
    public required long BigBlind { get; init; }

    /// <summary>
    /// Gets the straddle configuration.
    /// A value of <see langword="null"/> means that straddles are disabled.
    /// </summary>
    public StraddleRules? Straddle { get; init; }

    /// <summary>
    /// Gets the ante configuration.
    /// A value of <see langword="null"/> means that antes are disabled.
    /// </summary>
    public AnteRules? Ante { get; init; }

    /// <summary>
    /// Gets the number of independent boards created at the start of the hand.
    /// </summary>
    /// <remarks>
    /// The usual value is <c>1</c>.
    /// Double-board formats, such as a double-board bomb pot,
    /// may use a value of <c>2</c>.
    ///
    /// This value defines how many boards exist from the beginning
    /// of the hand.
    /// </remarks>
    public int InitialBoardCount { get; init; } = 1;

    /// <summary>
    /// Gets the maximum number of runouts allowed for each initial board.
    /// </summary>
    /// <remarks>
    /// A value of <c>1</c> disables multiple runouts.
    /// A value of <c>2</c> allows running the board twice.
    /// A value of <c>3</c> allows running the board three times.
    ///
    /// The actual runout count is selected separately for a specific hand
    /// after betting has been closed by an all-in.
    /// </remarks>
    public int MaxRunoutCount { get; init; } = 1;
}