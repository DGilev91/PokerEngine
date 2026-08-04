using PokerEngine.Enums;


namespace PokerEngine.Hand.Events;

/// <summary>
/// Represents an event emitted during a poker hand.
/// </summary>
public abstract record PokerHandEvent
{

}

/// <summary>
/// Indicates that a new hand has been created.
/// </summary>
public sealed record NewHandEvent : PokerHandEvent;

/// <summary>
/// Contains the initial stack for each seat.
/// </summary>
/// <param name="Stacks">
/// The initial stacks ordered by seat identifier.
/// </param>
public sealed record SeatsEvent(
    IReadOnlyList<long> Stacks) : PokerHandEvent
{

}

/// <summary>
/// Indicates that a player posted an ante, blind, or straddle.
/// </summary>
/// <param name="SeatId">
/// The seat that made the post.
/// </param>
/// <param name="PostType">
/// The type of posted contribution.
/// </param>
/// <param name="Amount">
/// The amount actually posted.
/// </param>
/// <param name="IsAllIn">
/// Indicates whether the post consumed the player's entire stack.
/// </param>
public sealed record PlayerPostedEvent(
    int SeatId,
    PostType PostType,
    long Amount,
    bool IsAllIn) : PokerHandEvent;

/// <summary>
/// Indicates that the hand has started.
/// </summary>
public sealed record HandStartedEvent : PokerHandEvent;

/// <summary>
/// Indicates that hole cards were dealt to a seat.
/// </summary>
/// <param name="SeatId">
/// The seat that received the cards.
/// </param>
/// <param name="Cards">
/// The dealt hole cards.
/// </param>
public sealed record HoleCardsEvent(
    int SeatId,
    IReadOnlyList<string> Cards) : PokerHandEvent
{
}

/// <summary>
/// Indicates that the engine is waiting for a runout-count decision.
/// </summary>
/// <param name="Count">
/// The maximum number of runouts that may be selected.
/// </param>
public sealed record WaitingRunoutEvent(
    int Count) : PokerHandEvent;

/// <summary>
/// Indicates that the number of runouts has been selected.
/// </summary>
/// <param name="Count">
/// The selected number of runouts.
/// </param>
public sealed record RunoutCountSelectedEvent(
    int Count) : PokerHandEvent;

/// <summary>
/// Indicates that board cards were dealt for a betting round.
/// </summary>
/// <param name="Round">
/// The round for which the cards were dealt.
/// </param>
/// <param name="BoardIndex">
/// The zero-based board index.
/// </param>
/// <param name="Cards">
/// The newly dealt board cards.
/// </param>
public sealed record BoardEvent(
    RoundType Round,
    int BoardIndex,
    IReadOnlyList<string> Cards) : PokerHandEvent
{
    /// <inheritdoc />
}

/// <summary>
/// Indicates that a player is expected to act.
/// </summary>
/// <param name="SeatId">
/// The seat that must act.
/// </param>
/// <param name="Actions">
/// The actions currently available to the player.
/// </param>
/// <param name="CallAmount">
/// The amount required to call.
/// </param>
/// <param name="MinBet">
/// The minimum permitted bet amount.
/// </param>
/// <param name="MaxBet">
/// The maximum permitted bet amount.
/// </param>
/// <param name="MinRaiseTo">
/// The minimum permitted total raise-to amount.
/// </param>
/// <param name="MaxRaiseTo">
/// The maximum permitted total raise-to amount.
/// </param>
public sealed record PlayerTurnEvent(
    int SeatId,
    IReadOnlyList<ActionType> Actions,
    long CallAmount,
    long MinBet,
    long MaxBet,
    long MinRaiseTo,
    long MaxRaiseTo) : PokerHandEvent
{

}

public sealed record FoldEvent(int SeatId) : PokerHandEvent;

public sealed record CheckEvent(int SeatId) : PokerHandEvent;

public sealed record CallEvent(int SeatId, long Amount, bool IsAllIn) : PokerHandEvent;

public sealed record BetEvent(int SeatId, long Amount, bool IsAllIn) : PokerHandEvent;

public sealed record RaiseToEvent(int SeatId, long RaiseAmount, long AmountTo, bool IsAllIn) : PokerHandEvent;

/// <summary>
/// Indicates that an unmatched portion of a wager was returned.
/// </summary>
/// <param name="SeatId">
/// The seat receiving the returned chips.
/// </param>
/// <param name="Amount">
/// The returned amount.
/// </param>
public sealed record UncalledBetReturnedEvent(
    int SeatId,
    long Amount) : PokerHandEvent;

/// <summary>
/// Indicates that a player revealed their hole cards.
/// </summary>
/// <param name="SeatId">
/// The seat that revealed the cards.
/// </param>
/// <param name="Cards">
/// The revealed hole cards.
/// </param>
public sealed record ShowCardsEvent(
    int SeatId,
    IReadOnlyList<string> Cards) : PokerHandEvent
{
    /// <inheritdoc />
}

/// <summary>
/// Contains the evaluated hand for a seat on a specific board.
/// </summary>
/// <param name="SeatId">
/// The evaluated seat.
/// </param>
/// <param name="BoardIndex">
/// The zero-based board index.
/// </param>
/// <param name="Category">
/// The evaluated hand category.
/// </param>
/// <param name="BestCards">
/// The best five-card combination.
/// </param>
public sealed record HandEvaluatedEvent(
    int SeatId,
    int BoardIndex,
    HandCategory Category,
    IReadOnlyList<string> BestCards) : PokerHandEvent
{
}

/// <summary>
/// Indicates that chips from a pot were awarded to a player.
/// </summary>
/// <param name="PotIndex">
/// The zero-based pot or side-pot index.
/// </param>
/// <param name="BoardIndex">
/// The board associated with the award.
/// </param>
/// <param name="SeatId">
/// The seat receiving the chips.
/// </param>
/// <param name="Amount">
/// The awarded amount.
/// </param>
public sealed record PotAwardedEvent(
    int PotIndex,
    int BoardIndex,
    int SeatId,
    long Amount) : PokerHandEvent;

/// <summary>
/// Indicates that the hand has completed.
/// </summary>
public sealed record EndHandEvent : PokerHandEvent;