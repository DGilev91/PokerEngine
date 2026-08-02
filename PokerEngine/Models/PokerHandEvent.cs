using PokerEngine.Enums;

namespace PokerEngine.Models;

/// <summary>
/// Represents an event emitted during a poker hand.
/// </summary>
public abstract record PokerHandEvent;

/// <summary>
/// Indicates that a new hand has been created.
/// </summary>
public sealed record NewHandEvent : PokerHandEvent;

/// <summary>
/// Contains the initial stack for each seat.
/// </summary>
/// <param name="stacks">
/// The initial stacks ordered by seat identifier.
/// </param>
public sealed record SeatsEvent(
    IReadOnlyList<long> stacks) : PokerHandEvent;

/// <summary>
/// Indicates that a player posted an ante, blind, or straddle.
/// </summary>
/// <param name="seatId">
/// The seat that made the post.
/// </param>
/// <param name="postType">
/// The type of posted contribution.
/// </param>
/// <param name="amount">
/// The amount actually posted.
/// </param>
/// <param name="isAllIn">
/// Indicates whether the post consumed the player's entire stack.
/// </param>
public sealed record PlayerPostedEvent(
    int seatId,
    PostType postType,
    long amount,
    bool isAllIn) : PokerHandEvent;

/// <summary>
/// Indicates that the hand has started.
/// </summary>
public sealed record HandStartedEvent : PokerHandEvent;

/// <summary>
/// Indicates that hole cards were dealt to a seat.
/// </summary>
/// <param name="seatId">
/// The seat that received the cards.
/// </param>
/// <param name="cards">
/// The dealt hole cards.
/// </param>
public sealed record HoleCardsEvent(
    int seatId,
    IReadOnlyList<string> cards) : PokerHandEvent;

/// <summary>
/// Indicates that the engine is waiting for a runout-count decision.
/// </summary>
/// <param name="count">
/// The maximum number of runouts that may be selected.
/// </param>
public sealed record WaitingRunoutEvent(
    int count) : PokerHandEvent;

/// <summary>
/// Indicates that the number of runouts has been selected.
/// </summary>
/// <param name="count">
/// The selected number of runouts.
/// </param>
public sealed record RunoutCountEvent(
    int count) : PokerHandEvent;

/// <summary>
/// Indicates that board cards were dealt for a betting round.
/// </summary>
/// <param name="round">
/// The round for which the cards were dealt.
/// </param>
/// <param name="boardIndex">
/// The zero-based board index.
/// </param>
/// <param name="cards">
/// The newly dealt board cards.
/// </param>
public sealed record BoardEvent(
    RoundType round,
    int boardIndex,
    IReadOnlyList<string> cards) : PokerHandEvent;

/// <summary>
/// Indicates that a player is expected to act.
/// </summary>
/// <param name="seatId">
/// The seat that must act.
/// </param>
/// <param name="actions">
/// The actions currently available to the player.
/// </param>
/// <param name="callAmount">
/// The amount required to call.
/// </param>
/// <param name="minBet">
/// The minimum permitted bet amount.
/// </param>
/// <param name="maxBet">
/// The maximum permitted bet amount.
/// </param>
/// <param name="minRaiseTo">
/// The minimum permitted total raise-to amount.
/// </param>
/// <param name="maxRaiseTo">
/// The maximum permitted total raise-to amount.
/// </param>
public sealed record PlayerTurnEvent(
    int seatId,
    IReadOnlyList<ActionType> actions,
    long callAmount,
    long minBet,
    long maxBet,
    long minRaiseTo,
    long maxRaiseTo) : PokerHandEvent;

/// <summary>
/// Indicates that a player performed a betting action.
/// </summary>
/// <param name="seatId">
/// The seat that performed the action.
/// </param>
/// <param name="actionType">
/// The performed action.
/// </param>
/// <param name="amount">
/// The number of chips committed by this action.
/// </param>
/// <param name="amountTo">
/// The player's total wager for the current betting round after the action.
/// </param>
/// <param name="isAllIn">
/// Indicates whether the action consumed the player's entire remaining stack.
/// </param>
public sealed record PlayerActionEvent(
    int seatId,
    ActionType actionType,
    long amount,
    long amountTo,
    bool isAllIn) : PokerHandEvent;

/// <summary>
/// Indicates that an unmatched portion of a wager was returned.
/// </summary>
/// <param name="seatId">
/// The seat receiving the returned chips.
/// </param>
/// <param name="amount">
/// The returned amount.
/// </param>
public sealed record UncalledBetReturnedEvent(
    int seatId,
    long amount) : PokerHandEvent;

/// <summary>
/// Indicates that a player revealed their hole cards.
/// </summary>
/// <param name="seatId">
/// The seat that revealed the cards.
/// </param>
/// <param name="cards">
/// The revealed hole cards.
/// </param>
public sealed record ShowCardsEvent(
    int seatId,
    IReadOnlyList<string> cards) : PokerHandEvent;

/// <summary>
/// Contains the evaluated hand for a seat on a specific board.
/// </summary>
/// <param name="seatId">
/// The evaluated seat.
/// </param>
/// <param name="boardIndex">
/// The zero-based board index.
/// </param>
/// <param name="category">
/// The evaluated hand category.
/// </param>
/// <param name="bestCards">
/// The best five-card combination.
/// </param>
public sealed record HandEvaluatedEvent(
    int seatId,
    int boardIndex,
    HandCategory category,
    IReadOnlyList<string> bestCards) : PokerHandEvent;

/// <summary>
/// Indicates that chips from a pot were awarded to a player.
/// </summary>
/// <param name="potIndex">
/// The zero-based pot or side-pot index.
/// </param>
/// <param name="boardIndex">
/// The board associated with the award.
/// </param>
/// <param name="seatId">
/// The seat receiving the chips.
/// </param>
/// <param name="amount">
/// The awarded amount.
/// </param>
public sealed record PotAwardedEvent(
    int potIndex,
    int boardIndex,
    int seatId,
    long amount) : PokerHandEvent;

/// <summary>
/// Indicates that the hand has completed.
/// </summary>
public sealed record EndHandEvent : PokerHandEvent;