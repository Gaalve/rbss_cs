using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.RBSS_CS;

namespace DAL1.RBSS_CS
{
    public class RedBlackTreePersistence : IPersistenceLayer
    {
        private readonly RedBlackTree<SimpleObjectWrapper> _set;

        public RedBlackTreePersistence()
        {
            _set = new RedBlackTree<SimpleObjectWrapper>();
        }

        public int GetFingerprint(string lower, string upper)
        {
            var lowerWrapper = new SimpleObjectWrapper(lower);
            var upperWrapper = new SimpleObjectWrapper(upper);

            return _set.GetFingerprint(lowerWrapper, upperWrapper);
        }

        public bool Insert(SimpleDataObject data)
        {
            return _set.Insert(new SimpleObjectWrapper(data));
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
                ranges[0] = new RangeSet(idFrom, tmidId, GetFingerprint(idFrom, tmidId).ToString(), Array.Empty<SimpleDataObject>());
                ranges[1] = new RangeSet(tmidId, idTo, GetFingerprint(tmidId, idTo).ToString(), new[]{list[0].Data});
                return ranges;
            }
            var midCount = (list.Count + 1) / 2;
            var range1 = list.GetRange(0, midCount);
            var range2 = list.GetRange(midCount, list.Count - midCount);
            var midId = range2[0].Data.Id;
            ranges[0] = new RangeSet(idFrom, midId, GetFingerprint(idFrom, midId).ToString(), 
                range1.Select(s => s.Data).ToArray());
            ranges[1] = new RangeSet(midId, idTo, GetFingerprint(midId, idTo).ToString(), 
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
            if (list.Count == 0) return new RangeSet("", "", "0");
            var data = list[0];
            return new RangeSet(data.Data.Id, data.Data.Id, GetFingerprint(data.Data.Id, data.Data.Id).ToString());
        }

        public void Clear()
        {
            _set.Clear();
        }
    }
}
