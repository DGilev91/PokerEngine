using System.Security.Cryptography;
using System.Text;
using PokerEngine.Lookups;
using PokerEngine.Utilities;

namespace PokerEngine.Tests.Lookups;

public abstract class LookupTestBase
{
    protected static string SerializeCombinations(IEnumerable<IEnumerable<Card>> combinations)
    {
        return string.Join('\n', combinations.Select(SerializeCombination));
    }

    protected static string SerializeCombination(IEnumerable<Card> combination)
    {
        return string.Concat(combination.Select(card => card.ToString()));
    }

    protected static string Md5(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = MD5.HashData(bytes);

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    protected static IEnumerable<IReadOnlyList<T>> Combinations<T>(IReadOnlyList<T> values, int count)
    {
        if (count < 0 || count > values.Count)
        {
            yield break;
        }

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
}

public sealed class StandardLookupExhaustiveTests : LookupTestBase
{
    [Fact]
    public void GetEntry()
    {
        var lookup = new StandardLookup();

        var combinations = Combinations(Deck.Standard, 5)
            .OrderBy(cards => lookup.GetEntry(cards))
            .ToArray();

        var value = SerializeCombinations(combinations);

        Assert.Equal("488cdd27873395ba75205cd02fb9d6b2", Md5(value));
    }
}

public sealed class ShortDeckHoldemLookupExhaustiveTests : LookupTestBase
{
    [Fact]
    public void GetEntry()
    {
        var lookup = new ShortDeckHoldemLookup();

        var combinations = Combinations(Deck.ShortDeckHoldem, 5)
            .OrderBy(cards => lookup.GetEntry(cards))
            .ToArray();

        var value = SerializeCombinations(combinations);

        Assert.Equal("5b46b727ce4526a68d41b0e55939353c", Md5(value));
    }
}

public sealed class EightOrBetterLookupExhaustiveTests : LookupTestBase
{
    [Fact]
    public void GetEntry()
    {
        var lookup = new EightOrBetterLookup();

        var combinations = Combinations(Deck.Regular, 5)
            .Where(cards => lookup.HasEntry(cards))
            .OrderBy(cards => lookup.GetEntry(cards))
            .ToArray();

        var value = SerializeCombinations(combinations);

        Assert.Equal("ecf2b6b16031562a6761932b1ce1de91", Md5(value));
    }
}

public sealed class RegularLookupExhaustiveTests : LookupTestBase
{
    [Fact]
    public void GetEntry()
    {
        var lookup = new RegularLookup();

        var combinations = Combinations(Deck.Regular, 5)
            .OrderBy(cards => lookup.GetEntry(cards))
            .ToArray();

        var value = SerializeCombinations(combinations);

        Assert.Equal("28925b97b06a1e674eaabf241a441e15", Md5(value));
    }
}

public sealed class BadugiLookupExhaustiveTests : LookupTestBase
{
    [Fact]
    public void GetEntry()
    {
        var lookup = new BadugiLookup();
        var combinations = new List<IReadOnlyList<Card>>();

        for (var count = 1; count <= 4; count++)
        {
            foreach (var cards in Combinations(Deck.Regular, count))
            {
                var paired = Card.ArePaired(cards);
                var suited = Card.AreSuited(cards);
                var rainbow = Card.AreRainbow(cards);

                if (!paired && rainbow)
                {
                    combinations.Add(cards);
                }
                else if (paired || suited)
                {
                    Assert.Throws<ArgumentException>(() => lookup.GetEntry(cards));
                }
            }
        }

        var ordered = combinations
            .OrderBy(cards => lookup.GetEntry(cards))
            .ToArray();

        var value = SerializeCombinations(ordered);

        Assert.Equal("9d29ddbc3f76d815e166c6faa2af9021", Md5(value));
    }
}

public sealed class StandardBadugiLookupExhaustiveTests : LookupTestBase
{
    [Fact]
    public void GetEntry()
    {
        var lookup = new StandardBadugiLookup();
        var combinations = new List<IReadOnlyList<Card>>();

        for (var count = 1; count <= 4; count++)
        {
            foreach (var cards in Combinations(Deck.Standard, count))
            {
                var paired = Card.ArePaired(cards);
                var suited = Card.AreSuited(cards);
                var rainbow = Card.AreRainbow(cards);

                if (!paired && rainbow)
                {
                    combinations.Add(cards);
                }
                else if (paired || suited)
                {
                    Assert.Throws<ArgumentException>(() => lookup.GetEntry(cards));
                }
            }
        }

        var ordered = combinations
            .OrderBy(cards => lookup.GetEntry(cards))
            .ToArray();

        var value = SerializeCombinations(ordered);

        Assert.Equal("06886dd57a4c7b780953f5090c63bcfe", Md5(value));
    }
}

public sealed class KuhnPokerLookupExhaustiveTests : LookupTestBase
{
    [Fact]
    public void GetEntry()
    {
        var lookup = new KuhnPokerLookup();

        var combinations = Combinations(Deck.KuhnPoker, 1)
            .OrderBy(cards => lookup.GetEntry(cards))
            .ToArray();

        var value = SerializeCombinations(combinations);

        Assert.Equal("bb7d5e9f8fe4f404fd1551d8995ca1a2", Md5(value));
    }
}

public sealed class RhodeIslandHoldemLookupExhaustiveTests : LookupTestBase
{
    [Fact]
    public void GetEntry()
    {
        var lookup = new RhodeIslandHoldemLookup();

        var combinations = Combinations(Deck.Standard, 3)
            .OrderBy(cards => lookup.GetEntry(cards))
            .ToArray();

        var value = SerializeCombinations(combinations);

        Assert.Equal("fcf110834e2232b3303cb039dcdd024b", Md5(value));
    }
}
