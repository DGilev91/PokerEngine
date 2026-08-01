namespace PokerEngine.Enums;

[Flags]
public enum Automation
{
    None = 0,

    ShuffleDeck = 1 << 0,
    PostAntes = 1 << 1,
    PostBlinds = 1 << 2,
    DealHoleCards = 1 << 3,
    BurnCards = 1 << 4,
    DealBoard = 1 << 5,

    All =
        ShuffleDeck |
        PostAntes |
        PostBlinds |
        DealHoleCards |
        BurnCards |
        DealBoard
}