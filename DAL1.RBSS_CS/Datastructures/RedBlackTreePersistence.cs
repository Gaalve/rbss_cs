using DAL1.RBSS_CS.Bifunctors;
using DAL1.RBSS_CS.Databse;
using DAL1.RBSS_CS.Hashfunction;
using Models.RBSS_CS;

namespace DAL1.RBSS_CS.Datastructures
{
    public class RedBlackTreePersistence : IPersistenceLayer
    {
        private readonly RedBlackTree<SimpleObjectWrapper> _set;
        private IDatabase _db;
        private IBifunctor _bifunctor;
        private IHashFunction _hashFunction;
        private int _branching;

        public RedBlackTreePersistence()
        {
            _set = new RedBlackTree<SimpleObjectWrapper>();
            _db = new DatabaseStub();
            _bifunctor = new XorBifunctor();
            _hashFunction = new StableHash();
            _branching = 2;
        }

        private static int ByteArrayComparator(byte[] arr1, byte[] arr2)
        {
            var s1 = Convert.ToBase64String(arr1);
            var s2 = Convert.ToBase64String(arr2);
            return string.Compare(s1, s2, StringComparison.Ordinal);
        }

        public string GetFingerprint(string lower, string upper)
        {
            var lowerWrapper = new SimpleObjectWrapper(lower);
            var upperWrapper = new SimpleObjectWrapper(upper);
            var fp = GetFingerprint(_set.GetSortedListBetween(lowerWrapper, upperWrapper));
            Console.WriteLine("Hash from " + lower+ " to " + upper + ": " + fp);
            return fp;
        }

        private string GetFingerprint(List<SimpleObjectWrapper> list)
        {
            var pc = _bifunctor.GetNewEmpty();
            foreach (var v in list)
            {
                Console.WriteLine("Hash from " + v.Data.Id + ": " + Convert.ToBase64String(v.Hash.Reverse().ToArray()));
                Console.WriteLine("Hash from " + v.Data.Id + ": " + "IntHash: " + BitConverter.ToInt32(v.Hash));
                Console.WriteLine("Hash from " + v.Data.Id + ": " +  "IntHashR: " + BitConverter.ToInt32(v.Hash.Reverse().ToArray()));
                pc.Apply(v.Hash);
            }
            //Console.WriteLine("IntHash: " + BitConverter.ToInt32(pc.Hash));
            //Console.WriteLine("IntHashR: " + BitConverter.ToInt32(pc.Hash.Reverse().ToArray()));
            Console.WriteLine("Base64: " + Convert.ToBase64String(pc.Hash));
            Console.WriteLine("Hash.Length: " + pc.Hash.Length);
            Console.WriteLine("HashR.Length: " + pc.Hash.Reverse().ToArray().Length);
            return Convert.ToBase64String(pc.Hash.Reverse().ToArray());
        }

        public bool Insert(SimpleDataObject data)
        {
            var curElement = _set.Search(new SimpleObjectWrapper(data.Id));
            if (curElement == null)
            {
                if (!_set.Insert(new SimpleObjectWrapper(data, _hashFunction))) return false;
                _db.Insert(data);
                return true;
            }
            if (curElement.Data.Timestamp > data.Timestamp) return false;
            var tempHash = _hashFunction.Hash(data);
            if (curElement.Data.Timestamp == data.Timestamp &&
                ByteArrayComparator(curElement.Hash, tempHash) >= 0) return false;
            curElement.Data = data;
            curElement.Hash = tempHash; // assign recalculated hash of object
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
            RangeSet[] ranges = new RangeSet[_branching];
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
