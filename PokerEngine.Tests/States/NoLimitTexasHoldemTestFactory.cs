using System.Reflection;
using PokerEngine.Enums;
using PokerEngine.Games;
using PokerEngine.Models;
using PokerEngine.States;

namespace PokerEngine.Tests.Games;

internal static class NoLimitTexasHoldemTestFactory
{
    public static NoLimitTexasHoldem CreateGame(
        Automation automation = Automation.None,
        long smallBlind = 50,
        long bigBlind = 100,
        AnteRules? ante = null,
        StraddleRules? straddle = null,
        int initialBoardCount = 1,
        int maxRunoutCount = 1)
    {
        return new NoLimitTexasHoldem(
            automation,
            smallBlind,
            bigBlind,
            ante,
            straddle,
            initialBoardCount,
            maxRunoutCount);
    }

    public static PokerState CreateState(
        Automation automation = Automation.None,
        long smallBlind = 50,
        long bigBlind = 100,
        AnteRules? ante = null,
        StraddleRules? straddle = null,
        int initialBoardCount = 1,
        int maxRunoutCount = 1)
    {
        return (PokerState)CreateGame(
            automation,
            smallBlind,
            bigBlind,
            ante,
            straddle,
            initialBoardCount,
            maxRunoutCount).CreateState();
    }

    public static PokerRules GetRules(NoLimitTexasHoldem game)
    {
        FieldInfo field = typeof(NoLimitTexasHoldem).GetField(
            "_rules",
            BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException(
                "В NoLimitTexasHoldem не найдено приватное поле _rules.");

        return (PokerRules)(field.GetValue(game)
            ?? throw new InvalidOperationException(
                "Поле _rules содержит null."));
    }

    public static AnteRules CreateAnte(
        long amount,
        AnteType type = AnteType.EveryPlayer)
    {
        return new AnteRules
        {
            Type = type,
            Amount = amount
        };
    }

    public static StraddleRules CreateStraddles(
        bool isMandatory,
        StraddleType type = StraddleType.Utg,
        params long[] amounts)
    {
        return new StraddleRules
        {
            Type = type,
            Amounts = amounts,
            IsMandatory = isMandatory
        };
    }
}
