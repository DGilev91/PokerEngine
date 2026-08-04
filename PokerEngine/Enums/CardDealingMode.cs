namespace PokerEngine.Enums;

[Flags]
public enum CardDealingMode
{
    Manual = 0,

    ShuffleDeck = 1 << 0,
    DealHoleCards = 1 << 1,
    BurnCards = 1 << 2,
    DealBoard = 1 << 3,

    Automatic = ShuffleDeck| DealHoleCards| BurnCards| DealBoard
}