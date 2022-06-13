using Models.RBSS_CS;

namespace DAL1.RBSS_CS;

public class SimpleObjectWrapper : IComparable<SimpleObjectWrapper>
{
    public SimpleDataObject Data { get; private set; }
    public int Hash { get; private set; }

    public SimpleObjectWrapper(SimpleDataObject sdo)
    {
        Data = sdo;
        Hash = sdo.GetHashCode();
    }

    public SimpleObjectWrapper(string id)
    {
        Data = new SimpleDataObject(id);
        Hash = 0;
    }

    public int CompareTo(SimpleObjectWrapper? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(Data.Id, other.Data.Id, StringComparison.Ordinal);
    }
}