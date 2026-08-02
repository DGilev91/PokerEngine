using PokerEngine.Enums;

namespace PokerEngine.Models;

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

/// <summary>
/// Defines the ante rules for a poker game.
/// </summary>
public sealed class AnteRules
{
    /// <summary>
    /// Gets the position or group of players responsible for posting the ante.
    /// </summary>
    public required AnteType Type { get; init; }

    /// <summary>
    /// Gets the required ante amount.
    /// </summary>
    public required long Amount { get; init; }
}

/// <summary>
/// Defines the straddle rules for a poker game.
/// </summary>
public sealed class StraddleRules
{
    /// <summary>
    /// Gets the rule used to determine which position may post
    /// the first straddle.
    /// </summary>
    public required StraddleType Type { get; init; }

    /// <summary>
    /// Gets the permitted amounts for consecutive straddles.
    /// </summary>
    /// <remarks>
    /// The first item is the initial straddle amount.
    /// The second item is the first restraddle amount,
    /// followed by any additional restraddles.
    ///
    /// An empty collection means that straddles are disabled.
    /// </remarks>
    public required IReadOnlyList<long> Amounts { get; init; }

    /// <summary>
    /// Gets a value indicating whether the first straddle is mandatory.
    /// </summary>
    public bool IsMandatory { get; init; }
}

/// <summary>
/// Defines the rules for a single betting round.
/// </summary>
public sealed class RoundRules
{
    /// <summary>
    /// Gets the betting round type.
    /// </summary>
    public required RoundType Type { get; init; }

    /// <summary>
    /// Gets the number of cards dealt to each board
    /// at the beginning of this round.
    /// </summary>
    public int BoardCardCount { get; init; }

    /// <summary>
    /// Gets the configured bet size for this round.
    /// </summary>
    /// <remarks>
    /// In fixed-limit games, this value defines the fixed bet
    /// and full raise size.
    ///
    /// In no-limit and pot-limit games, it commonly defines
    /// the minimum full bet size.
    /// </remarks>
    public required long BetSize { get; init; }

    /// <summary>
    /// Gets the maximum number of full raises allowed
    /// during this round.
    /// </summary>
    /// <remarks>
    /// A value of <see langword="null"/> means that there is no limit.
    /// This setting is commonly used only in fixed-limit games.
    /// </remarks>
    public int? MaxRaises { get; init; }
}