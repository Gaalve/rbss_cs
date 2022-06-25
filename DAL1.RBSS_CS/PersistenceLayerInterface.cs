using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.RBSS_CS;

namespace DAL1.RBSS_CS
{
    public interface IPersistenceLayer
    {
        public string GetFingerprint(string lower, string upper);
        public bool Insert(SimpleDataObject data);
        public SimpleDataObject[] GetDataObjects();
        public RangeSet[] SplitRange(string idFrom, string idTo);
        public RangeSet CreateRangeSet(string idFrom, string idTo);
        public RangeSet CreateRangeSet(string idFrom, string idTo, ICollection<SimpleDataObject> exclude);
        public RangeSet CreateRangeSet();
        public void Clear();
    }

    public interface IPersistenceLayerSingleton: IPersistenceLayer
    {
        static IPersistenceLayerSingleton Instance { get; }
    }


}
