using PokerEngine.Enums;

namespace PokerEngine.Models;

public sealed record Post(int SeatId, BlindType Type, long Amount);

public sealed record Ante(bool Uniform, IReadOnlyList<long> Amounts)
{
    public static Ante None => new(true, [0]);

    public static Ante EveryPlayer(long amount)
    {
        return new(true, [amount]);
    }

    public static Ante ByPosition(params long[] amounts)
    {
        return new(false, amounts);
    }
}