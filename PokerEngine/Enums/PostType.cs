namespace PokerEngine.Enums;

public enum PostType
{
    Ante,          // Анте (мертвые деньги)
    SmallBlind,     // Малый блайнд
    BigBlind,       // Большой блайнд
    DeadBlind,      // Мертвый блайнд (при возвращении за стол)
    ExtraBlind,     // Живой блайнд вне очереди
    Straddle        // Страдл (любого типа)
}
