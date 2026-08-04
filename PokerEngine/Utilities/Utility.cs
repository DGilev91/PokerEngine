using PokerKit.States;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PokerKit.Utilities;

public static class Utility
{
    public static Regex UnmatchablePattern { get; } = new("(?!)", RegexOptions.Compiled);

    public static IEnumerable<T> FilterNotNull<T>(IEnumerable<T?> values) where T : class
    {
        ArgumentNullException.ThrowIfNull(values);

        return values.Where(value => value is not null)!;
    }

    public static T? MinOrNull<T>(IEnumerable<T?> values) where T : struct, IComparable<T>
    {
        ArgumentNullException.ThrowIfNull(values);

        var items = values.Where(value => value.HasValue).Select(value => value!.Value).ToArray();

        return items.Length == 0 ? null : items.Min();
    }

    public static T? MaxOrNull<T>(IEnumerable<T?> values) where T : struct, IComparable<T>
    {
        ArgumentNullException.ThrowIfNull(values);

        var items = values.Where(value => value.HasValue).Select(value => value!.Value).ToArray();

        return items.Length == 0 ? null : items.Max();
    }

    public static long[] CleanValues(long value, int count)
    {
        ValidateCount(count);

        return Enumerable.Repeat(value, count).ToArray();
    }

    public static long[] CleanValues(IEnumerable<long> values, int count)
    {
        ArgumentNullException.ThrowIfNull(values);
        ValidateCount(count);

        var result = values.Take(count).ToList();

        while (result.Count < count)
        {
            result.Add(0);
        }

        return result.ToArray();
    }

    public static long[] CleanValues(IReadOnlyDictionary<int, long> values, int count)
    {
        ArgumentNullException.ThrowIfNull(values);
        ValidateCount(count);

        var result = new long[count];

        foreach (var pair in values)
        {
            var index = pair.Key < 0 ? count + pair.Key : pair.Key;

            if (index < 0 || index >= count)
            {
                throw new ArgumentOutOfRangeException(nameof(values), $"Index {pair.Key} is outside a collection of length {count}.");
            }

            result[index] += pair.Value;
        }

        return result;
    }

    public static List<T> Shuffled<T>(IEnumerable<T> values, Random? random = null)
    {
        ArgumentNullException.ThrowIfNull(values);

        random ??= Random.Shared;

        var result = values.ToList();

        for (var index = result.Count - 1; index > 0; index--)
        {
            var other = random.Next(index + 1);
            (result[index], result[other]) = (result[other], result[index]);
        }

        return result;
    }

    public static Queue<T> Rotated<T>(IEnumerable<T> values, int count)
    {
        ArgumentNullException.ThrowIfNull(values);

        var items = values.ToList();

        if (items.Count == 0)
        {
            return new Queue<T>();
        }

        // Python deque.rotate(n): positive rotates right, negative rotates left.
        var normalized = ((count % items.Count) + items.Count) % items.Count;

        if (normalized != 0)
        {
            items = items[^normalized..].Concat(items[..^normalized]).ToList();
        }

        return new Queue<T>(items);
    }

    public static (long Quotient, long Remainder) DivMod(long dividend, long divisor)
    {
        if (divisor == 0)
        {
            throw new DivideByZeroException();
        }

        return (Math.DivRem(dividend, divisor, out var remainder), remainder);
    }

    public static (decimal Quotient, decimal Remainder) DivMod(decimal dividend, decimal divisor)
    {
        if (divisor == 0)
        {
            throw new DivideByZeroException();
        }

        var quotient = dividend / divisor;

        return (quotient, dividend - quotient * divisor);
    }

    public static (long RakedAmount, long UnrakedAmount) Rake(long amount, State? state = null, decimal percentage = 0, long? cap = null, bool noFlopNoDrop = false)
    {
        if (percentage < 0 || percentage > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(percentage), "The rake percentage must be between 0 and 1.");
        }

        if (noFlopNoDrop)
        {
            if (state is null)
            {
                throw new ArgumentNullException(nameof(state), "If no-flop-no-drop is enabled, the state is required.");
            }

            if (!state.BoardCards.Any(cards => cards.Count > 0))
            {
                return (0, amount);
            }
        }

        var rakedAmount = decimal.Round(amount * percentage, 0, MidpointRounding.ToEven);
        var effectiveCap = cap ?? long.MaxValue;
        var limited = Math.Min(rakedAmount, effectiveCap);
        var integral = checked((long)limited);

        return (integral, amount - integral);
    }

    public static (decimal RakedAmount, decimal UnrakedAmount) Rake(decimal amount, State? state = null, decimal percentage = 0, decimal? cap = null, bool noFlopNoDrop = false)
    {
        if (percentage < 0 || percentage > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(percentage), "The rake percentage must be between 0 and 1.");
        }

        if (noFlopNoDrop)
        {
            if (state is null)
            {
                throw new ArgumentNullException(nameof(state), "If no-flop-no-drop is enabled, the state is required.");
            }

            if (!state.BoardCards.Any(cards => cards.Count > 0))
            {
                return (0, amount);
            }
        }

        var rakedAmount = amount * percentage;
        var limited = Math.Min(rakedAmount, cap ?? decimal.MaxValue);

        return (limited, amount - limited);
    }

    public static object ParseValue(string rawValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawValue);

        rawValue = rawValue.Replace(",", string.Empty);

        if (long.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integral))
        {
            return integral;
        }

        return decimal.Parse(rawValue, NumberStyles.Number, CultureInfo.InvariantCulture);
    }

    public static TimeOnly ParseTime(string rawTime)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawTime);

        return TimeOnly.ParseExact(rawTime, "HH:mm:ss", CultureInfo.InvariantCulture);
    }

    public static int ParseMonth(string rawMonth)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawMonth);

        return DateTime.ParseExact(rawMonth, "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces).Month;
    }

    public static int Sign(long value)
    {
        return Math.Sign(value);
    }

    private static void ValidateCount(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
    }
}
