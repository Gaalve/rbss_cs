using DAL1.RBSS_CS.Bifunctors;
using DAL1.RBSS_CS.Databse;
using DAL1.RBSS_CS.Hashfunction;
using Models.RBSS_CS;

namespace DAL1.RBSS_CS.Datastructures
{
    public class SortedSetPersistence: IPersistenceLayer
    {
        private readonly SortedSet<SimpleObjectWrapper> _set;
        private IDatabase _db;
        private IBifunctor _bifunctor;
        private IHashFunction _hashFunction;
        private int _branching;

        public SortedSetPersistence()
        {
            _set = new SortedSet<SimpleObjectWrapper>();
            _db = new DatabaseStub();
            _bifunctor = new XorBifunctor();
            _hashFunction = new StableHash();
            _branching = 2;
        }
        public string GetFingerprint(string idFrom, string idTo)
        {
            if (_set.Count == 0) return "AA==";
            if (idFrom == idTo) return GetFingerprint(_set);
            var lower = new SimpleObjectWrapper(idFrom);
            var lastExceeded = string.Compare(idFrom, idTo, StringComparison.Ordinal) > 0;
            var upper = lastExceeded ? _set.Last() : new SimpleObjectWrapper(idTo);
            RangeSet[] ranges = new RangeSet[2];
            var list = _set.GetViewBetween(lower, upper).Where(s => s.Data.Id != idTo).ToList();
            if (lastExceeded) list.AddRange(_set.GetViewBetween(new SimpleObjectWrapper(""), new SimpleObjectWrapper(idTo)).Where(s => s.Data.Id != idTo));
            return GetFingerprint(list);
        }

        private string GetFingerprint(IEnumerable<SimpleObjectWrapper> list)
        {
            var pc = _bifunctor.GetNewEmpty();
            foreach (var v in list)
            {
                pc.Apply(v.Hash);
            }

            return Convert.ToBase64String(pc.Hash);
        }

        public bool Insert(SimpleDataObject data)
        {
            if (!_set.Add(new SimpleObjectWrapper(data, _hashFunction))) return false;
            _db.Insert(data);
            return true;
        }


        public SimpleDataObject[] GetDataObjects()
        {
            return _set.Select(sel => sel.Data).ToArray();
        }


        public RangeSet[] SplitRange(string idFrom, string idTo)
        {
            if (_set.Count == 0) return new RangeSet[2];
            // if (idFrom == idTo) return SplitRange(idFrom);
            var lower = new SimpleObjectWrapper(idFrom);
            var lastExceeded = string.Compare(idFrom, idTo, StringComparison.Ordinal) > 0;
            var upper = lastExceeded ? _set.Last() : new SimpleObjectWrapper(idTo);
            RangeSet[] ranges = new RangeSet[2];
            var list = idFrom == idTo ? _set.ToList() : _set.GetViewBetween(lower, upper).Where(s => s.Data.Id != idTo).ToList();
            if (lastExceeded) list.AddRange(_set.GetViewBetween(new SimpleObjectWrapper(""), new SimpleObjectWrapper(idTo)).Where(s => s.Data.Id != idTo));
            
            var count = list.Count;
            if (count == 0) return ranges;
            var idx = 0;
            string curId = idFrom;
            // Split ranges into approximately equal parts
            for (int i = 0; i < _branching; i++)
            {
                var rem = _branching - i;
                var nc = (count - idx + rem - 1) / rem; // integer ceiling of remaining items in list
                var listRange = list.GetRange(idx, nc);
                idx += nc;
                if (idx < count)
                {
                    string nextId = list[idx].Data.Id;
                    ranges[i] = new RangeSet(curId, nextId, GetFingerprint(listRange),
                        listRange.Select(s => s.Data).ToArray());
                    curId = nextId;
                }
                else
                {
                    ranges[i] = new RangeSet(curId, idTo, GetFingerprint(listRange),
                        listRange.Select(s => s.Data).ToArray());
                    break;
                }
            }

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

        public void SetHashFunction(IHashFunction hashFunction)
        {
            _hashFunction = hashFunction;
        }

        public void SetBifunctor(IBifunctor bifunctor)
        {
            _bifunctor = bifunctor;
        }

        public void SetBranchingFactor(int branchingFactor)
        {
            if (branchingFactor < 2) throw new Exception("Branching Factor is not supported: " + branchingFactor);
            _branching = branchingFactor;
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
