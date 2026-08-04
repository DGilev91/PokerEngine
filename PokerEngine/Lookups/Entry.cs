namespace PokerEngine.Lookups;

public readonly record struct Entry(int Index, Label Label) : IComparable<Entry>
{
    public int CompareTo(Entry other)
    {
        return Index.CompareTo(other.Index);
    }
}
