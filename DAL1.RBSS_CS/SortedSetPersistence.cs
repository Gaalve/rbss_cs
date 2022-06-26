using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.RBSS_CS;

namespace DAL1.RBSS_CS
{
    public class SortedSetPersistence: IPersistenceLayer
    {
        private readonly SortedSet<SimpleObjectWrapper> _set;

        public SortedSetPersistence()
        {
            _set = new SortedSet<SimpleObjectWrapper>();
        }
        public string GetFingerprint(string lower, string upper)
        {
            //if (string.Compare(lower, upper, StringComparison.Ordinal) > 0) return 0;
            PrecalculatedHash hash = new PrecalculatedHash();
            if (string.Compare(lower, upper, StringComparison.Ordinal) == 0)
            {
                foreach (var v in _set)
                {
                    hash = hash.Bifunctor(v.Hash);
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
                    hash = hash.Bifunctor(v.Hash);
                }
                
            }
            Console.Write(") = "+hash.Hash + "\n");
            return Convert.ToBase64String(hash.Hash);
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
            var subset = _set.GetViewBetween(lower, upper);
            if (subset.Count == 0) return ranges;
            var offset = subset.Last().Data.Id.Equals(upper.Data.Id) && !lastExceeded? 0 : 1;
            var midId = subset.Count == 1 ? subset.First().Data.Id : subset.Select(s => s.Data.Id).ToArray()[(subset.Count + offset) / 2];
            var mid = new SimpleObjectWrapper(midId);

            var range1 = subset.GetViewBetween(lower, mid);
            var range2 = subset.GetViewBetween(mid, upper);

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
                        s => s.Id != idTo && !exclude.Contains(s)).ToArray());
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
    }
}
