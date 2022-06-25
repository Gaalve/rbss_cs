using Models.RBSS_CS;

namespace DAL1.RBSS_CS;

public class SimpleObjectWrapper : PrecalculatedHash, IComparable<SimpleObjectWrapper>
{
    public SimpleDataObject Data { get; private set; }

    public int UnixTimestamp { get; private set; }

    public SimpleObjectWrapper(SimpleDataObject sdo) : base(BitConverter.GetBytes(sdo.GetHashCode()))
    {
        Data = sdo;
    }

    public SimpleObjectWrapper(string id) : base(new byte[]{0})
    {
        Data = new SimpleDataObject(id, 0, "");
    }

    public int CompareTo(SimpleObjectWrapper? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(Data.Id, other.Data.Id, StringComparison.Ordinal);
    }

}