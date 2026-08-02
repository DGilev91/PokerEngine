using PokerEngine.Enums;
using PokerEngine.Rules;
using PokerEngine.States;

namespace PokerEngine.Games;

/// <summary>
/// Represents a No-Limit Texas Hold'em game definition.
/// </summary>
public sealed class NoLimitTexasHoldem : PokerGame
{
    private readonly PokerRules _rules;

    /// <summary>
    /// Initializes a new instance of the <see cref="NoLimitTexasHoldem"/> class.
    /// </summary>
    /// <param name="automation">
    /// Specifies which game operations are performed automatically.
    /// </param>
    /// <param name="smallBlind">
    /// The small blind amount.
    /// </param>
    /// <param name="bigBlind">
    /// The big blind amount.
    /// </param>
    /// <param name="ante">
    /// Optional ante configuration.
    /// </param>
    /// <param name="straddle">
    /// Optional straddle configuration.
    /// </param>
    /// <param name="initialBoardCount">
    /// The initial number of boards.
    /// </param>
    /// <param name="maxRunoutCount">
    /// The maximum number of runouts allowed.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when blind, ante, board, or runout values are invalid.
    /// </exception>
    public NoLimitTexasHoldem(
        Automation automation,
        long smallBlind,
        long bigBlind,
        AnteRules? ante = null,
        StraddleRules? straddle = null,
        int initialBoardCount = 1,
        int maxRunoutCount = 1)
    {
        if (smallBlind <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(smallBlind),
                smallBlind,
                "Small blind must be greater than zero.");
        }

        if (bigBlind <= smallBlind)
        {
            throw new ArgumentOutOfRangeException(
                nameof(bigBlind),
                bigBlind,
                "Big blind must be greater than the small blind.");
        }

        if (ante is not null && ante.Amount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(ante),
                ante.Amount,
                "Ante amount cannot be negative.");
        }

        if (initialBoardCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(initialBoardCount),
                initialBoardCount,
                "Initial board count must be greater than zero.");
        }

        if (maxRunoutCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxRunoutCount),
                maxRunoutCount,
                "Maximum runout count must be greater than zero.");
        }

        _rules = new PokerRules
        {
            Automation = automation,
            GameType = GameType.TexasHoldem,
            GameLimit = GameLimit.NoLimit,

            Rounds =
            [
                new RoundRules
                {
                    Type = RoundType.Preflop,
                    BoardCardCount = 0,
                    BetSize = bigBlind,
                    MaxRaises = null
                },
                new RoundRules
                {
                    Type = RoundType.Flop,
                    BoardCardCount = 3,
                    BetSize = bigBlind,
                    MaxRaises = null
                },
                new RoundRules
                {
                    Type = RoundType.Turn,
                    BoardCardCount = 1,
                    BetSize = bigBlind,
                    MaxRaises = null
                },
                new RoundRules
                {
                    Type = RoundType.River,
                    BoardCardCount = 1,
                    BetSize = bigBlind,
                    MaxRaises = null
                }
            ],

            Ante = ante,
            SmallBlind = smallBlind,
            BigBlind = bigBlind,
            Straddle = straddle,

            InitialBoardCount = initialBoardCount,
            MaxRunoutCount = maxRunoutCount
        };
    }

    /// <inheritdoc />
    public override IPokerState CreateState()
    {
        return new PokerState(_rules);
    }
}