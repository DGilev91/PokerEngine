using PokerEngine.Enums;
using PokerEngine.States.Events;
using PokerEngine.States.Pots;
using PokerEngine.States.Seats;

namespace PokerEngine.States;

/// <summary>
/// Represents the mutable state of a single poker hand.
/// </summary>
/// <remarks>
/// <para>
/// Seat identifiers are zero-based and must remain contiguous:
/// <c>0, 1, 2, ...</c>.
/// </para>
///
/// <para>
/// The engine uses the following fixed seat mapping:
/// </para>
///
/// <list type="bullet">
/// <item>
/// <description>
/// Seat <c>0</c> is always the small blind.
/// </description>
/// </item>
/// <item>
/// <description>
/// Seat <c>1</c> is always the big blind.
/// </description>
/// </item>
/// <item>
/// <description>
/// In games with three or more players, the last seat is the button.
/// </description>
/// </item>
/// <item>
/// <description>
/// In heads-up games, seat <c>0</c> is both the button and the small blind,
/// while seat <c>1</c> is the big blind.
/// </description>
/// </item>
/// </list>
///
/// <para>
/// All chip amounts are represented by <see cref="long"/> values.
/// The caller is responsible for choosing the monetary unit, such as cents
/// for cash games or whole chips for tournaments.
/// </para>
/// </remarks>
public interface IPokerState
{
    /// <summary>
    /// Gets all events emitted during the hand in chronological order.
    /// </summary>
    IReadOnlyList<PokerHandEvent> Events { get; }

    /// <summary>
    /// Gets all seats participating in the hand.
    /// </summary>
    /// <remarks>
    /// The index in this collection is expected to match
    /// <see cref="Seat.SeatId"/>.
    /// </remarks>
    IReadOnlyList<Seat> Seats { get; }

    /// <summary>
    /// Gets the current pot and side-pot state.
    /// </summary>
    PotState PotState { get; }

    /// <summary>
    /// Gets all boards currently used by the hand.
    /// </summary>
    /// <remarks>
    /// Board index <c>0</c> is the primary board.
    /// Additional boards may be created for multiple initial boards
    /// or multiple runouts.
    /// </remarks>
    IReadOnlyList<IReadOnlyList<string>> Boards { get; }

    /// <summary>
    /// Gets the current lifecycle state of the hand.
    /// </summary>
    HandState State { get; }

    /// <summary>
    /// Gets the current betting round or showdown stage.
    /// </summary>
    RoundType Round { get; }

    /// <summary>
    /// Initializes a new hand with the supplied player stacks.
    /// </summary>
    /// <param name="stacks">
    /// The initial stack for each seat.
    /// The index in this collection becomes the seat identifier.
    /// </param>
    /// <remarks>
    /// At least two positive stacks are required.
    /// Forced posts may be performed automatically after initialization,
    /// depending on the configured automation flags.
    /// </remarks>
    void Initialize(IReadOnlyList<long> stacks);

    /// <summary>
    /// Posts an ante, blind, extra blind, dead blind, or straddle
    /// for the specified seat.
    /// </summary>
    /// <param name="seatId">
    /// The zero-based seat identifier.
    /// </param>
    /// <param name="postType">
    /// The type of forced contribution.
    /// </param>
    /// <param name="amount">
    /// The requested contribution amount.
    /// If the seat has fewer chips, the entire remaining stack is posted.
    /// </param>
    /// <remarks>
    /// This method must be called before <see cref="Start"/>.
    /// Small blind posts are expected from seat <c>0</c>,
    /// and big blind posts are expected from seat <c>1</c>.
    /// </remarks>
    void PlayerPost(
        int seatId,
        PostType postType,
        long amount);

    /// <summary>
    /// Starts the hand and begins the first betting round.
    /// </summary>
    /// <remarks>
    /// All required manual forced posts should be made before this method
    /// is called.
    /// </remarks>
    void Start();

    /// <summary>
    /// Deals hole cards to the specified seat.
    /// </summary>
    /// <param name="seatId">
    /// The zero-based seat identifier.
    /// </param>
    /// <param name="cards">
    /// The hole cards to assign.
    /// Pass <see langword="null"/> when automatic hole-card dealing is enabled.
    /// </param>
    /// <remarks>
    /// The value <c>"xx"</c> may be used for an unknown hole card.
    /// Unknown cards may later be replaced by calling <see cref="ShowCards"/>.
    /// </remarks>
    void DealHole(
        int seatId,
        IReadOnlyList<string>? cards = null);

    /// <summary>
    /// Performs a betting action for the specified seat.
    /// </summary>
    /// <param name="seatId">
    /// The seat currently expected to act.
    /// </param>
    /// <param name="actionType">
    /// The action to perform.
    /// </param>
    /// <param name="amount">
    /// The action amount.
    /// This value is ignored for fold, check, and call.
    /// For bet, it is the target total wager for the current round.
    /// For raise-to, it is the final total wager for the current round,
    /// not the additional number of chips to add.
    /// </param>
    void PlayerAction(
        int seatId,
        ActionType actionType,
        long amount = 0);

    /// <summary>
    /// Selects the number of runouts after betting has been closed by an all-in.
    /// </summary>
    /// <param name="count">
    /// The number of runouts to use.
    /// </param>
    /// <remarks>
    /// A value greater than one is only valid while the engine is waiting
    /// for a runout decision.
    /// </remarks>
    void SetRunoutCount(int count);

    /// <summary>
    /// Deals the cards required for the next street on the specified board.
    /// </summary>
    /// <param name="boardIndex">
    /// The zero-based board index.
    /// </param>
    /// <param name="cards">
    /// The cards for the next street.
    /// Pass <see langword="null"/> when automatic board dealing is enabled.
    /// </param>
    /// <remarks>
    /// For Texas Hold'em, the expected card counts are normally:
    /// three cards for the flop, one card for the turn,
    /// and one card for the river.
    /// </remarks>
    void DealBoard(
        int boardIndex = 0,
        IReadOnlyList<string>? cards = null);

    /// <summary>
    /// Reveals or replaces the hole cards of the specified seat.
    /// </summary>
    /// <param name="seatId">
    /// The zero-based seat identifier.
    /// </param>
    /// <param name="cards">
    /// The complete known hole-card set for the seat.
    /// </param>
    /// <remarks>
    /// This method may replace cards previously represented by <c>"xx"</c>.
    /// When all showdown contenders have known cards, the engine can evaluate
    /// hands, award pots, and complete the hand.
    /// </remarks>
    void ShowCards(
        int seatId,
        IReadOnlyList<string> cards);
}