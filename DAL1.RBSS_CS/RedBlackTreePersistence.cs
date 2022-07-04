using Models.RBSS_CS;

namespace DAL1.RBSS_CS
{
    public class RedBlackTreePersistence : IPersistenceLayer
    {
        private readonly RedBlackTree<SimpleObjectWrapper> _set;
        private IDatabase _db;

        public RedBlackTreePersistence()
        {
            _set = new RedBlackTree<SimpleObjectWrapper>();
            _db = new DatabaseStub();
        }

        public string GetFingerprint(string lower, string upper)
        {
            var lowerWrapper = new SimpleObjectWrapper(lower);
            var upperWrapper = new SimpleObjectWrapper(upper);
            return Convert.ToBase64String(_set.GetFingerprint(lowerWrapper, upperWrapper));
        }

        public string GetFingerprint(List<SimpleObjectWrapper> sow)
        {
            return Convert.ToBase64String(_set.GetFingerprint(sow));
        }

        public bool Insert(SimpleDataObject data)
        {
            if (!_set.Insert(new SimpleObjectWrapper(data))) return false;
            _db.Insert(data);
            return true;
        }

        public SimpleDataObject[] GetDataObjects()
        {
            return _set.GetSortedList().Select(s => s.Data).ToArray();
        }

        public RangeSet[] SplitRange(string idFrom, string idTo)
        {
            var lowerWrapper = new SimpleObjectWrapper(idFrom);
            var upperWrapper = new SimpleObjectWrapper(idTo);
            var list = _set.GetSortedListBetween(lowerWrapper, upperWrapper);
            RangeSet[] ranges = new RangeSet[2];
            if (list.Count == 0) return ranges;
            if (list.Count == 1)
            {
                var tmidId = list[0].Data.Id;
                ranges[0] = new RangeSet(idFrom, tmidId, "AA==", Array.Empty<SimpleDataObject>()); 
                //ranges[0] will be ignored
                ranges[1] = new RangeSet(idFrom, idTo, GetFingerprint(list), new[]{list[0].Data});
                return ranges;
            }
            var midCount = (list.Count + 1) / 2;
            var range1 = list.GetRange(0, midCount);
            var range2 = list.GetRange(midCount, list.Count - midCount);
            var midId = range2[0].Data.Id;
            ranges[0] = new RangeSet(idFrom, midId, GetFingerprint(range1), 
                range1.Select(s => s.Data).ToArray());
            ranges[1] = new RangeSet(midId, idTo, GetFingerprint(range2), 
                range2.Select(s => s.Data).ToArray());
            return ranges;

        }

        public RangeSet CreateRangeSet(string idFrom, string idTo)
        {
            var lowerWrapper = new SimpleObjectWrapper(idFrom);
            var upperWrapper = new SimpleObjectWrapper(idTo);
            var list = _set.GetSortedListBetween(lowerWrapper, upperWrapper);
            return new RangeSet(idFrom, idTo, "null", list.Select(s => s.Data).ToArray());
        }

        public RangeSet CreateRangeSet(string idFrom, string idTo, ICollection<SimpleDataObject> exclude)
        {
            var lowerWrapper = new SimpleObjectWrapper(idFrom);
            var upperWrapper = new SimpleObjectWrapper(idTo);
            var list = _set.GetSortedListBetween(lowerWrapper, upperWrapper);
            return new RangeSet(idFrom, idTo, "null", list.Select(s => s.Data)
                .Where(s => !exclude.Contains(s)).ToArray());
        }

        public RangeSet CreateRangeSet()
        {
            var list = _set.GetSortedList();
            if (list.Count == 0) return new RangeSet("", "", "AA==");
            var data = list[0];
            return new RangeSet(data.Data.Id, data.Data.Id, GetFingerprint(list));
        }

        public SimpleDataObject? Search(string key)
        {
            return _set.Search(new SimpleObjectWrapper(key))?.Data;
        }

        public void Clear()
        {
            _set.Clear();
        }

        public void SetDb(IDatabase db)
        {
            _db = db;
        }

        public void SetHashFunction()
        {
            throw new NotImplementedException();
        }

        public void SetBifunctor()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            var objs = _db.GetAllDataObjects();
            foreach (var o in objs)
            {
                Insert(o);
            }
        }
    }
}
