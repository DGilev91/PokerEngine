using PokerEngine.States.Seats;

namespace PokerEngine.States.Pots;

/// <summary>
/// Represents the current collection of pots and any uncalled bet
/// associated with a poker hand.
/// </summary>
public sealed class PotState
{
    private readonly List<Pot> _pots = [];

    private int? _uncalledCandidateSeatId;

    /// <summary>
    /// Gets all main and side pots in index order.
    /// </summary>
    public IReadOnlyList<Pot> Pots => _pots;

    /// <summary>
    /// Gets the main pot, or <see langword="null"/>
    /// when no pot exists.
    /// </summary>
    public Pot? MainPot =>
        _pots.Count > 0
            ? _pots[0]
            : null;

    /// <summary>
    /// Gets all side pots.
    /// </summary>
    public IReadOnlyList<Pot> SidePots =>
        _pots.Count > 1
            ? _pots.Skip(1).ToArray()
            : [];

    /// <summary>
    /// Gets the current uncalled bet, or <see langword="null"/>
    /// when no uncalled amount exists.
    /// </summary>
    public UncalledBet? UncalledBet { get; private set; }

    /// <summary>
    /// Gets the total amount contained in all matched pots.
    /// </summary>
    public long PotAmount =>
        _pots.Sum(pot => pot.Amount);

    /// <summary>
    /// Gets the current uncalled amount.
    /// </summary>
    public long UncalledAmount =>
        UncalledBet?.Amount ?? 0;

    /// <summary>
    /// Gets the combined amount contained in the pots
    /// and the current uncalled bet.
    /// </summary>
    public long TotalAmount =>
        PotAmount + UncalledAmount;

    /// <summary>
    /// Clears all pot information.
    /// </summary>
    internal void Clear()
    {
        _pots.Clear();

        _uncalledCandidateSeatId = null;
        UncalledBet = null;
    }

    /// <summary>
    /// Marks a seat as the possible owner of an uncalled bet.
    /// </summary>
    internal void SetUncalledCandidate(
        int seatId,
        IReadOnlyList<Seat> seats)
    {
        ValidateSeatId(seatId, seats);

        _uncalledCandidateSeatId = seatId;

        RefreshUncalledBet(seats);
    }

    /// <summary>
    /// Recalculates the current uncalled amount.
    /// </summary>
    internal void RefreshUncalledBet(
        IReadOnlyList<Seat> seats)
    {
        ArgumentNullException.ThrowIfNull(seats);

        if (!_uncalledCandidateSeatId.HasValue)
        {
            UncalledBet = null;
            return;
        }

        int seatId =
            _uncalledCandidateSeatId.Value;

        ValidateSeatId(seatId, seats);

        Seat candidate = seats[seatId];

        if (candidate.IsFolded)
        {
            ClearUncalledBet();
            return;
        }

        long highestOtherRoundBet = seats
            .Where(seat =>
                seat.SeatId != candidate.SeatId)
            .Select(seat => seat.RoundBet)
            .DefaultIfEmpty(0)
            .Max();

        long amount = Math.Max(
            0,
            candidate.RoundBet -
            highestOtherRoundBet);

        if (amount == 0)
        {
            ClearUncalledBet();
            return;
        }

        UncalledBet = new UncalledBet(
            candidate.SeatId,
            amount);
    }

    /// <summary>
    /// Clears the current uncalled-bet candidate.
    /// </summary>
    internal void ClearUncalledBet()
    {
        _uncalledCandidateSeatId = null;
        UncalledBet = null;
    }

    /// <summary>
    /// Removes and returns the current uncalled bet.
    /// </summary>
    internal UncalledBet? TakeUncalledBet(
        IReadOnlyList<Seat> seats)
    {
        RefreshUncalledBet(seats);

        UncalledBet? uncalledBet =
            UncalledBet;

        ClearUncalledBet();

        return uncalledBet;
    }

    /// <summary>
    /// Rebuilds the main pot and all side pots
    /// from the current seat contributions.
    /// </summary>
    internal void Rebuild(
        IReadOnlyList<Seat> seats)
    {
        ArgumentNullException.ThrowIfNull(seats);

        _pots.Clear();

        RefreshUncalledBet(seats);

        Dictionary<int, long> matchedBets =
            seats.ToDictionary(
                seat => seat.SeatId,
                seat =>
                {
                    long uncalledAmount =
                        UncalledBet?.SeatId ==
                        seat.SeatId
                            ? UncalledBet.Amount
                            : 0;

                    return Math.Max(
                        0,
                        seat.TotalBet -
                        uncalledAmount);
                });

        long[] levels = matchedBets
            .Values
            .Where(amount => amount > 0)
            .Distinct()
            .Order()
            .ToArray();

        long previousLevel = 0;

        List<PotBuilder> builders = [];

        foreach (long level in levels)
        {
            Seat[] contributors = seats
                .Where(seat =>
                    matchedBets[seat.SeatId] >= level)
                .ToArray();

            long contributionPerSeat =
                level - previousLevel;

            if (contributionPerSeat <= 0 ||
                contributors.Length == 0)
            {
                previousLevel = level;
                continue;
            }

            long amount =
                contributionPerSeat *
                contributors.Length;

            int[] eligibleSeatIds = contributors
                .Where(seat => !seat.IsFolded)
                .Select(seat => seat.SeatId)
                .Order()
                .ToArray();

            Dictionary<int, long> contributions =
                contributors.ToDictionary(
                    seat => seat.SeatId,
                    _ => contributionPerSeat);

            PotBuilder? previousBuilder =
                builders.LastOrDefault();

            if (previousBuilder is not null &&
                previousBuilder.EligibleSeatIds
                    .SequenceEqual(eligibleSeatIds))
            {
                previousBuilder.Amount += amount;

                foreach ((
                    int seatId,
                    long contribution) in contributions)
                {
                    previousBuilder.Contributions[seatId] =
                        previousBuilder.Contributions
                            .GetValueOrDefault(seatId) +
                        contribution;
                }
            }
            else
            {
                builders.Add(new PotBuilder
                {
                    Amount = amount,
                    EligibleSeatIds =
                        eligibleSeatIds,
                    Contributions =
                        contributions
                });
            }

            previousLevel = level;
        }

        for (int potIndex = 0;
             potIndex < builders.Count;
             potIndex++)
        {
            PotBuilder builder =
                builders[potIndex];

            Pot pot = new(potIndex);

            foreach ((
                int seatId,
                long contribution)
                in builder.Contributions)
            {
                pot.AddContribution(
                    seatId,
                    contribution);
            }

            foreach (Seat foldedSeat in seats.Where(
                         seat => seat.IsFolded))
            {
                pot.RemoveEligibility(
                    foldedSeat.SeatId);
            }

            _pots.Add(pot);
        }
    }

    private static void ValidateSeatId(
        int seatId,
        IReadOnlyList<Seat> seats)
    {
        ArgumentNullException.ThrowIfNull(seats);

        if (seatId < 0 ||
            seatId >= seats.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(seatId),
                seatId,
                $"Seat identifier must be between 0 and {seats.Count - 1}.");
        }
    }

    private sealed class PotBuilder
    {
        public long Amount { get; set; }

        public required IReadOnlyList<int>
            EligibleSeatIds
        {
            get;
            init;
        }

        public required Dictionary<int, long>
            Contributions
        {
            get;
            init;
        }
    }
}