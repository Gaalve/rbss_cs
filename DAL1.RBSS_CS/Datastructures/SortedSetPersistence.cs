using DAL1.RBSS_CS.Bifunctors;
using DAL1.RBSS_CS.Databse;
using Models.RBSS_CS;

namespace DAL1.RBSS_CS.Datastructures
{
    public class SortedSetPersistence: IPersistenceLayer
    {
        private readonly SortedSet<SimpleObjectWrapper> _set;
        private IDatabase _db;
        private IBifunctor _bifunctor;

        public SortedSetPersistence()
        {
            _set = new SortedSet<SimpleObjectWrapper>();
            _db = new DatabaseStub();
            _bifunctor = new XorBifunctor();
        }
        public string GetFingerprint(string lower, string upper)
        {
            if (_set.Count == 0) return "AA==";
            //if (string.Compare(lower, upper, StringComparison.Ordinal) > 0) return 0;
            IBifunctor hash = _bifunctor.GetNewEmpty();
            if (string.Compare(lower, upper, StringComparison.Ordinal) == 0)
            {
                foreach (var v in _set)
                {
                    hash.Apply(v.Hash);
                }
                
                return Convert.ToBase64String(hash.Hash);
            }

            var upperData = (string.Compare(lower, upper, StringComparison.Ordinal) > 0) ? _set.Last() : new SimpleObjectWrapper(upper);
            var subset = _set.GetViewBetween(new SimpleObjectWrapper(lower), upperData);
            Console.Write("Fp[");
            foreach (var v in subset)
            {
                if (v.Data.Id != upper)
                {
                    Console.Write(v.Data.Id + "(" + v.Hash + "),");
                    hash.Apply(v.Hash);
                }
                
            }
            Console.Write(") = "+hash.Hash + "\n");
            return Convert.ToBase64String(hash.Hash);
        }

        public bool Insert(SimpleDataObject data)
        {
            if (!_set.Add(new SimpleObjectWrapper(data))) return false;
            _db.Insert(data);
            return true;
        }


        public SimpleDataObject[] GetDataObjects()
        {
            return _set.Select(sel => sel.Data).ToArray();
        }


        private RangeSet[] SplitRange(string id)
        {
            RangeSet[] ranges = new RangeSet[2];
            if (_set.Count == 0) return ranges;
            var lower = new SimpleObjectWrapper(id);
            var upper = _set.Last();
            var midId = _set.Count == 1 ? upper.Data.Id : _set.Select(s => s.Data.Id).ToArray()[(1 + _set.Count) / 2];
            var mid = new SimpleObjectWrapper(midId);
            var range1 = _set.GetViewBetween(lower, mid);
            var range2 = _set.GetViewBetween(mid, upper);
            
            ranges[0] = new RangeSet(id, midId, GetFingerprint(id, midId), range1.Select(s => s.Data).Where(s => s.Id != midId).ToArray());
            ranges[1] = new RangeSet(midId, id, GetFingerprint(midId, id), range2.Select(s => s.Data).ToArray());
            return ranges;
        }


        public RangeSet[] SplitRange(string idFrom, string idTo)
        {
            if (_set.Count == 0) return new RangeSet[2];
            if (idFrom == idTo) return SplitRange(idFrom);
            var lower = new SimpleObjectWrapper(idFrom);
            var lastExceeded = string.Compare(idFrom, idTo, StringComparison.Ordinal) > 0;
            var upper = lastExceeded ? _set.Last() : new SimpleObjectWrapper(idTo);
            RangeSet[] ranges = new RangeSet[2];
            var subset = _set.GetViewBetween(lower, upper).Where(s => s.Data.Id != idTo).ToList();
            if (lastExceeded) subset.AddRange(_set.GetViewBetween(new SimpleObjectWrapper(""), new SimpleObjectWrapper(idTo)).Where(s => s.Data.Id != idTo));
            
            if (subset.Count == 0) return ranges;
            if (subset.Count == 1)
            {
                var tmidId = subset.First().Data.Id;
                ranges[0] = new RangeSet(idFrom, tmidId, "AA==", Array.Empty<SimpleDataObject>()); 
                //ranges[0] will be ignored
                ranges[1] = new RangeSet(idFrom, idTo, GetFingerprint(idFrom, idTo), new[]{subset.First().Data});
                return ranges;
            }
            var midCount = (subset.Count + 1) / 2;
            var midId = subset[midCount].Data.Id;

            var range1 = subset.GetRange(0, midCount);
            var range2 = subset.GetRange(midCount, subset.Count - midCount);

            ranges[0] = new RangeSet(idFrom, midId, GetFingerprint(idFrom, midId), range1.Select(s => s.Data).Where(s => s.Id != midId).ToArray());
            ranges[1] = new RangeSet(midId, idTo, GetFingerprint(midId, idTo), range2.Select(s => s.Data).Where(s => s.Id != idTo).ToArray());
            return ranges;
        }

        public RangeSet CreateRangeSet(string idFrom, string idTo)
        {
            if (_set.Count == 0) return new RangeSet(idFrom, idTo, "AA==");
            if (idFrom == idTo)
                return new RangeSet(idFrom, idTo, "null",
                    _set.Select(s => s.Data).ToArray());
            return new RangeSet(idFrom, idTo, "null", _set.GetViewBetween(new SimpleObjectWrapper(idFrom),
                string.Compare(idFrom, idTo, StringComparison.Ordinal) > 0 ? _set.Last() : 
                    new SimpleObjectWrapper(idTo)).Select(s => s.Data).Where(s => s.Id != idTo).ToArray());
        }

        public RangeSet CreateRangeSet(string idFrom, string idTo, ICollection<SimpleDataObject> exclude)
        {
            if (_set.Count == 0) return new RangeSet(idFrom, idTo, "AA==");
            if (idFrom == idTo)
                return new RangeSet(idFrom, idTo, "null",
                    _set.Select(s => s.Data).Where(
                        s => !exclude.Contains(s)).ToArray());
            return new RangeSet(idFrom, idTo, "null", _set.GetViewBetween(new SimpleObjectWrapper(idFrom),
                string.Compare(idFrom, idTo, StringComparison.Ordinal) > 0 ? _set.Last() : 
                    new SimpleObjectWrapper(idTo)).Select(s => s.Data).Where(
                s => s.Id != idTo && !exclude.Contains(s)).ToArray());
        }


        public RangeSet CreateRangeSet()
        {
            if (_set.Count == 0) return new RangeSet("", "", "AA==");
            var data = _set.First();

            return new RangeSet(data.Data.Id, data.Data.Id, GetFingerprint(data.Data.Id, data.Data.Id));
        }

        public SimpleDataObject? Search(string key)
        {
            var sow = new SimpleObjectWrapper(key);
            return _set.TryGetValue(sow, out sow) ? sow.Data : null;
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

        public void SetBifunctor(IBifunctor bifunctor)
        {
            _bifunctor = bifunctor;
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
