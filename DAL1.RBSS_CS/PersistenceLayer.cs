using Models.RBSS_CS;

namespace DAL1.RBSS_CS
{
    public sealed class PersistenceLayer
    {
        // private static readonly Lazy<PersistenceLayer> Lazy = new(() => new PersistenceLayer());
        // public static PersistenceLayer Instance => Lazy.Value;
        // private readonly SortedSet<SimpleObjectWrapper> _set;
        //
        // private PersistenceLayer()
        // {
        //     _set = new SortedSet<SimpleObjectWrapper>();
        // }

        private static volatile PersistenceLayer? _instance;
        private static readonly object SyncRoot = new object();

        private readonly SortedSet<SimpleObjectWrapper> _set;

        private PersistenceLayer()
        {
            _set = new SortedSet<SimpleObjectWrapper>();
        }

        public static PersistenceLayer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new PersistenceLayer();
                        }
                    }
                }
                return _instance;
            }
        }


        public int GetFingerprint(string lower, string upper)
        {
            //if (string.Compare(lower, upper, StringComparison.Ordinal) > 0) return 0;
            int hash = 0;
            if (string.Compare(lower, upper, StringComparison.Ordinal) == 0)
            {
                foreach (var v in _set)
                {
                    hash ^= v.Hash;
                }

                return hash;
            }

            var upperData = (string.Compare(lower, upper, StringComparison.Ordinal) > 0) ? _set.Last() : new SimpleObjectWrapper(upper);
            var subset = _set.GetViewBetween(new SimpleObjectWrapper(lower), upperData);
            Console.Write("Fp[");
            foreach (var v in subset)
            {
                if (v.Data.Id != upper)
                {
                    Console.Write(v.Data.Id + "(" + v.Hash.ToString() + "),");
                    hash ^= v.Hash;
                }
                
            }
            Console.Write(") = "+hash.ToString() + "\n");
            return hash;
        }

        public bool Insert(SimpleDataObject data)
        {
            return _set.Add(new SimpleObjectWrapper(data));
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
            
            ranges[0] = new RangeSet(id, midId, GetFingerprint(id, midId).ToString(), range1.Select(s => s.Data).Where(s => s.Id != midId).ToArray());
            ranges[1] = new RangeSet(midId, id, GetFingerprint(midId, id).ToString(), range2.Select(s => s.Data).ToArray());
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
            var subset = _set.GetViewBetween(lower, upper);
            if (subset.Count == 0) return ranges;
            var offset = subset.Last().Data.Id.Equals(upper.Data.Id) && !lastExceeded? 0 : 1;
            var midId = subset.Count == 1 ? subset.First().Data.Id : subset.Select(s => s.Data.Id).ToArray()[(subset.Count + offset) / 2];
            var mid = new SimpleObjectWrapper(midId);

            var range1 = subset.GetViewBetween(lower, mid);
            var range2 = subset.GetViewBetween(mid, upper);

            ranges[0] = new RangeSet(idFrom, midId, GetFingerprint(idFrom, midId).ToString(), range1.Select(s => s.Data).Where(s => s.Id != midId).ToArray());
            ranges[1] = new RangeSet(midId, idTo, GetFingerprint(midId, idTo).ToString(), range2.Select(s => s.Data).Where(s => s.Id != idTo).ToArray());
            return ranges;
        }

        public RangeSet CreateRangeSet(string idFrom, string idTo)
        {
            return new RangeSet(idFrom, idTo, "null", _set.GetViewBetween(new SimpleObjectWrapper(idFrom),
                string.Compare(idFrom, idTo, StringComparison.Ordinal) > 0 ? _set.Last() : new SimpleObjectWrapper(idTo)).Select(s => s.Data).Where(s => s.Id != idTo).ToArray());
        }

        public RangeSet CreateRangeSet(string idFrom, string idTo, ICollection<SimpleDataObject> exclude)
        {
            return new RangeSet(idFrom, idTo, "null", _set.GetViewBetween(new SimpleObjectWrapper(idFrom),
                string.Compare(idFrom, idTo, StringComparison.Ordinal) > 0 ? _set.Last() : 
                    new SimpleObjectWrapper(idTo)).Select(s => s.Data).Where(s => s.Id != idTo && !exclude.Contains(s)).ToArray());
        }


        public RangeSet CreateRangeSet()
        {
            if (_set.Count == 0) return new RangeSet("", "", "0");
            var data = _set.First();

            return new RangeSet(data.Data.Id, data.Data.Id, GetFingerprint(data.Data.Id, data.Data.Id).ToString());
        }

        public void Clear()
        {
            _set.Clear();
        }

    }

    
}