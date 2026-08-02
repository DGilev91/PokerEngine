using PokerEngine.Enums;
using System.Collections;
using System.Reflection;

namespace PokerEngine.States.Events;

/// <summary>
/// Represents an event emitted during a poker hand.
/// </summary>
public abstract record PokerHandEvent
{
    private protected static string FormatList<T>(IEnumerable<T> values)
    {
        return $"[{string.Join(", ", values)}]";
    }
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
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(SeatsEvent)} {{ " +
               $"Stacks = {FormatList(Stacks)} }}";
    }
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
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(HoleCardsEvent)} {{ " +
               $"SeatId = {SeatId}, " +
               $"Cards = {FormatList(Cards)} }}";
    }
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
    public override string ToString()
    {
        return $"{nameof(BoardEvent)} {{ " +
               $"Round = {Round}, " +
               $"BoardIndex = {BoardIndex}, " +
               $"Cards = {FormatList(Cards)} }}";
    }
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
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(PlayerTurnEvent)} {{ " +
               $"SeatId = {SeatId}, " +
               $"Actions = {FormatList(Actions)}, " +
               $"CallAmount = {CallAmount}, " +
               $"MinBet = {MinBet}, " +
               $"MaxBet = {MaxBet}, " +
               $"MinRaiseTo = {MinRaiseTo}, " +
               $"MaxRaiseTo = {MaxRaiseTo} }}";
    }
}

/// <summary>
/// Indicates that a player performed a betting action.
/// </summary>
/// <param name="SeatId">
/// The seat that performed the action.
/// </param>
/// <param name="ActionType">
/// The performed action.
/// </param>
/// <param name="Amount">
/// The number of chips committed by this action.
/// </param>
/// <param name="IsAllIn">
/// Indicates whether the action consumed the player's entire remaining stack.
/// </param>
public sealed record PlayerActionEvent(
    int SeatId,
    ActionType ActionType,
    long Amount,
    bool IsAllIn) : PokerHandEvent;

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
    public override string ToString()
    {
        return $"{nameof(ShowCardsEvent)} {{ " +
               $"SeatId = {SeatId}, " +
               $"Cards = {FormatList(Cards)} }}";
    }
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
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(HandEvaluatedEvent)} {{ " +
               $"SeatId = {SeatId}, " +
               $"BoardIndex = {BoardIndex}, " +
               $"Category = {Category}, " +
               $"BestCards = {FormatList(BestCards)} }}";
    }
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