using PokerEngine.Utilities;

namespace PokerEngine.Tests.Utilities;

public sealed class UtilityTests
{
    [Fact]
    public void CleanScalarValues()
    {
        Assert.Equal([4L, 4L, 4L, 4L], Utility.CleanValues(4, 4));
    }

    [Fact]
    public void CleanEnumerableValuesPadsAndTruncates()
    {
        Assert.Equal([1L, 2L, 3L, 4L, 0L, 0L], Utility.CleanValues([1, 2, 3, 4], 6));
        Assert.Equal([1L, 2L], Utility.CleanValues([1, 2, 3], 2));
    }

    [Fact]
    public void CleanMappingSupportsNegativeIndices()
    {
        var values = new Dictionary<int, long>
        {
            [0] = 1,
            [-1] = 2
        };

        Assert.Equal([1L, 0L, 0L, 2L], Utility.CleanValues(values, 4));
    }

    [Fact]
    public void RotateMatchesPythonDeque()
    {
        Assert.Equal(["c", "d", "a", "b"], Utility.Rotated(["a", "b", "c", "d"], 2));
        Assert.Equal([3, 4, 0, 1, 2], Utility.Rotated(Enumerable.Range(0, 5), -3));
    }

    [Fact]
    public void DivModMatchesIntegralBehavior()
    {
        Assert.Equal((3L, 2L), Utility.DivMod(11, 3));
    }

    [Fact]
    public void DefaultRakeIsZero()
    {
        Assert.Equal((0L, 100L), Utility.Rake(100));
        Assert.Equal((100L, 900L), Utility.Rake(1000, percentage: 0.1m));
        Assert.Equal((1L, 9L), Utility.Rake(10, percentage: 0.11m));
    }

    [Fact]
    public void ParseHelpers()
    {
        Assert.Equal(3L, Utility.ParseValue("3"));
        Assert.Equal(3.5m, Utility.ParseValue("3.5"));
        Assert.Equal(new TimeOnly(12, 34, 56), Utility.ParseTime("12:34:56"));
        Assert.Equal(7, Utility.ParseMonth("July"));
        Assert.Equal(-1, Utility.Sign(-5));
        Assert.Equal(0, Utility.Sign(0));
        Assert.Equal(1, Utility.Sign(10));
    }
}
