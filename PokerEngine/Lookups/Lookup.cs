using PokerEngine.Utilities;

namespace PokerEngine.Lookups;

public abstract class Lookup
{
    private static readonly int[] Primes = [2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41];

    private static readonly IReadOnlyDictionary<Rank, int> Multipliers =
        Enum.GetValues<Rank>()
            .Where(rank => rank != Rank.Unknown)
            .Zip(Primes)
            .ToDictionary(pair => pair.First, pair => pair.Second);

    private readonly Dictionary<(long Hash, bool Suited), Entry> _entries = [];
    private int _entryCount;

    protected abstract IReadOnlyList<Rank> RankOrder { get; }

    protected Lookup()
    {
        AddEntries();
        ResetRanks();
    }

    protected abstract void AddEntries();

    public bool HasEntry(IEnumerable<Card> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        try
        {
            return _entries.ContainsKey(GetKey(cards));
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public bool HasEntry(string cards)
    {
        return HasEntry(Card.Parse(cards));
    }

    public Entry GetEntry(IEnumerable<Card> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        var key = GetKey(cards);

        if (!_entries.TryGetValue(key, out var entry))
        {
            throw new ArgumentException($"The cards '{string.Concat(cards)}' form an invalid hand.", nameof(cards));
        }

        return entry;
    }

    public Entry GetEntry(string cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        var parsed = Card.Parse(cards).ToArray();

        if (!_entries.TryGetValue(GetKey(parsed), out var entry))
        {
            throw new ArgumentException($"The cards '{cards}' form an invalid hand.", nameof(cards));
        }

        return entry;
    }

    public Entry? GetEntryOrNull(IEnumerable<Card> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        return _entries.TryGetValue(GetKey(cards), out var entry) ? entry : null;
    }

    public Entry? GetEntryOrNull(string cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        return GetEntryOrNull(Card.Parse(cards));
    }

    protected virtual (long Hash, bool Suited) GetKey(IEnumerable<Card> cards)
    {
        var values = cards.ToArray();

        return (Hash(values.Select(card => card.Rank)), Card.AreSuited(values));
    }

    protected void AddMultisets(IReadOnlyDictionary<int, int> counter, IReadOnlyList<bool> suitednesses, Label label)
    {
        foreach (var hash in HashMultisets(RankOrder, new Dictionary<int, int>(counter)).Reverse())
        {
            AddEntry(hash, suitednesses, label);
        }
    }

    protected void AddStraights(int count, IReadOnlyList<bool> suitednesses, Label label)
    {
        var wheel = RankOrder.TakeLast(1).Concat(RankOrder.Take(count - 1));

        AddEntry(Hash(wheel), suitednesses, label);

        for (var index = 0; index < RankOrder.Count - count + 1; index++)
        {
            AddEntry(Hash(RankOrder.Skip(index).Take(count)), suitednesses, label);
        }
    }

    private static long Hash(IEnumerable<Rank> ranks)
    {
        long result = 1;

        foreach (var rank in ranks)
        {
            if (!Multipliers.TryGetValue(rank, out var multiplier))
            {
                throw new ArgumentException($"The rank {rank} cannot be hashed.");
            }

            result *= multiplier;
        }

        return result;
    }

    private static IReadOnlyList<long> HashMultisets(IReadOnlyList<Rank> ranks, Dictionary<int, int> counter)
    {
        if (counter.Count == 0)
        {
            return [Hash([])];
        }

        var multiplicity = counter.Keys.Max();
        var count = counter[multiplicity];
        counter.Remove(multiplicity);

        var hashes = new List<long>();

        foreach (var samples in Combinations(ranks.Reverse().ToArray(), count))
        {
            var sampleSet = samples.ToHashSet();
            var partialRanks = ranks.Where(rank => !sampleSet.Contains(rank)).ToArray();
            var hash = Pow(Hash(samples), multiplicity);

            foreach (var partialHash in HashMultisets(partialRanks, counter))
            {
                hashes.Add(hash * partialHash);
            }
        }

        counter[multiplicity] = count;

        return hashes;
    }

    private static IEnumerable<IReadOnlyList<T>> Combinations<T>(IReadOnlyList<T> values, int count)
    {
        if (count == 0)
        {
            yield return [];
            yield break;
        }

        for (var index = 0; index <= values.Count - count; index++)
        {
            foreach (var tail in Combinations(values.Skip(index + 1).ToArray(), count - 1))
            {
                yield return [values[index], .. tail];
            }
        }
    }

    private static long Pow(long value, int exponent)
    {
        long result = 1;

        for (var index = 0; index < exponent; index++)
        {
            result *= value;
        }

        return result;
    }

    private void AddEntry(long hash, IEnumerable<bool> suitednesses, Label label)
    {
        var entry = new Entry(_entryCount, label);
        _entryCount++;

        foreach (var suited in suitednesses)
        {
            _entries[(hash, suited)] = entry;
        }
    }

    private void ResetRanks()
    {
        var indices = _entries.Values.Select(entry => entry.Index).Distinct().Order().ToArray();
        var reset = indices.Select((index, replacement) => (index, replacement)).ToDictionary(item => item.index, item => item.replacement);

        foreach (var key in _entries.Keys.ToArray())
        {
            var entry = _entries[key];
            _entries[key] = entry with { Index = reset[entry.Index] };
        }
    }
}
