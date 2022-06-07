using Models.RBB_CS;

namespace DAL1.RBB_CS;

public class RangeSet
{
    public string IdFrom;
    public string IdTo;
    public string Fingerprint;
    public SimpleDataObject[]? Data;

    public RangeSet(string idFrom, string idTo, string fingerprint, SimpleDataObject[]? data = null)
    {
        this.IdFrom = idFrom;
        this.IdTo = idTo;
        this.Fingerprint = fingerprint;
        this.Data = data;
    }
}