using DAL1.RBSS_CS.Bifunctors;
using DAL1.RBSS_CS.Databse;
using Models.RBSS_CS;

namespace DAL1.RBSS_CS
{
    public class PersistenceLayer<T> : IPersistenceLayerSingleton where T : IPersistenceLayer, new()
    {

        private readonly IPersistenceLayer _auxillaryDs;


        public PersistenceLayer(IDatabase database, IBifunctor bifunctor)
        {
            _auxillaryDs = new T();
            _auxillaryDs.SetDb(database);
            _auxillaryDs.SetBifunctor(bifunctor);
        }


        public string GetFingerprint(string lower, string upper)
        {
            return _auxillaryDs.GetFingerprint(lower, upper);
        }

        public bool Insert(SimpleDataObject data)
        {
            return _auxillaryDs.Insert(data);
        }

        public SimpleDataObject[] GetDataObjects()
        {
            return _auxillaryDs.GetDataObjects();
        }

        public RangeSet[] SplitRange(string idFrom, string idTo)
        {
            return _auxillaryDs.SplitRange(idFrom, idTo);
        }

        public RangeSet CreateRangeSet(string idFrom, string idTo)
        {
            return _auxillaryDs.CreateRangeSet(idFrom, idTo);
        }

        public RangeSet CreateRangeSet(string idFrom, string idTo, ICollection<SimpleDataObject> exclude)
        {
            return _auxillaryDs.CreateRangeSet(idFrom, idTo, exclude);
        }

        public RangeSet CreateRangeSet()
        {
            return _auxillaryDs.CreateRangeSet();
        }

        public SimpleDataObject? Search(string key)
        {
            return _auxillaryDs.Search(key);
        }

        public void Clear()
        {
            _auxillaryDs.Clear();
        }

        public void Initialize()
        {
            _auxillaryDs.Initialize();
        }
    }

    
}