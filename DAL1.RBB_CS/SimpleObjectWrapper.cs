using Models.RBB_CS;

namespace DAL1.RBB_CS;

public class SimpleObjectWrapper : IComparable<SimpleObjectWrapper>
{
    public SimpleDataObject Data { get; private set; }
    public int Hash { get; private set; }

    public SimpleObjectWrapper(SimpleDataObject sdo)
    {
        Data = sdo;
        Hash = sdo.GetHashCode();
    }

    public SimpleObjectWrapper(int hash)
    {
        Data = null;
        Hash = hash;
    }

    public int CompareTo(SimpleObjectWrapper? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Hash.CompareTo(other.Hash);
    }
}