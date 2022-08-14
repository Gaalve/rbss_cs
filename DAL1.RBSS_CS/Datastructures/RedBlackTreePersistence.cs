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

        /// <summary>
        /// Converts the byte array from the host byte order to the network byte order
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        private static byte[] HostToNetworkBytes(IEnumerable<byte> arr)
        {
            var na = arr.ToArray();
            if (BitConverter.IsLittleEndian) Array.Reverse(na);
            return na;
        }
        public RedBlackTreePersistence()
        {
            _set = new RedBlackTree<SimpleObjectWrapper>();
            _db = new DatabaseStub();
            _bifunctor = new XorBifunctor();
            _hashFunction = new StableHash();
            _branching = 2;
        }

        /// <summary>
        /// Compares two byte arrays by converting them to their base64 representation
        /// </summary>
        /// <param name="arr1"></param>
        /// <param name="arr2"></param>
        /// <returns></returns>
        private static int ByteArrayComparator(IEnumerable<byte> arr1, IEnumerable<byte> arr2)
        {
            var s1 = Convert.ToBase64String(HostToNetworkBytes(arr1));
            var s2 = Convert.ToBase64String(HostToNetworkBytes(arr2));
            return string.Compare(s1, s2, StringComparison.Ordinal);
        }

        /// <summary>
        /// Return the incremental fingerprint over all elements within the specified bound
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public string GetFingerprint(string lower, string upper)
        {
            var lowerWrapper = new SimpleObjectWrapper(lower);
            var upperWrapper = new SimpleObjectWrapper(upper);
            var fp = GetFingerprint(_set.GetSortedListBetween(lowerWrapper, upperWrapper));
            return fp;
        }

        /// <summary>
        /// Returns the incremental fingerprint over all items contained in the list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string GetFingerprint(List<SimpleObjectWrapper> list)
        {
            var pc = _bifunctor.GetNewEmpty();
            foreach (var v in list)
            {
                pc.Apply(v.Hash);
            }
            return Convert.ToBase64String(HostToNetworkBytes(pc.Hash));
        }

        /// <summary>
        /// Inserts or updates an element
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns all items contained in the set in ascending order
        /// </summary>
        /// <returns></returns>
        public SimpleDataObject[] GetDataObjects()
        {
            return _set.GetSortedList().Select(s => s.Data).ToArray();
        }

        /// <summary>
        /// Splits the range specified by the bounds according to the set branching factor
        /// </summary>
        /// <param name="idFrom">lower bound</param>
        /// <param name="idTo">upper bound</param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a range set over the specified bound
        /// </summary>
        /// <param name="idFrom"></param>
        /// <param name="idTo"></param>
        /// <returns></returns>
        public RangeSet CreateRangeSet(string idFrom, string idTo)
        {
            var lowerWrapper = new SimpleObjectWrapper(idFrom);
            var upperWrapper = new SimpleObjectWrapper(idTo);
            var list = _set.GetSortedListBetween(lowerWrapper, upperWrapper);
            return new RangeSet(idFrom, idTo, "null", list.Select(s => s.Data).ToArray());
        }

        /// <summary>
        /// Creates a range set over the specified bound and excluding certain elements
        /// </summary>
        /// <param name="idFrom"></param>
        /// <param name="idTo"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public RangeSet CreateRangeSet(string idFrom, string idTo, ICollection<SimpleDataObject> exclude)
        {
            var lowerWrapper = new SimpleObjectWrapper(idFrom);
            var upperWrapper = new SimpleObjectWrapper(idTo);
            var list = _set.GetSortedListBetween(lowerWrapper, upperWrapper);
            return new RangeSet(idFrom, idTo, "null", list.Select(s => s.Data)
                .Where(s => !exclude.Contains(s)).ToArray());
        }

        /// <summary>
        /// Creates a range set over all elements
        /// </summary>
        /// <returns></returns>
        public RangeSet CreateRangeSet()
        {
            var list = _set.GetSortedList();
            if (list.Count == 0) return new RangeSet("", "", "AAAAAA==");
            var data = list[0];
            return new RangeSet(data.Data.Id, data.Data.Id, GetFingerprint(list));
        }

        /// <summary>
        /// Searches for an element by the ID and returns it or null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SimpleDataObject? Search(string key)
        {
            return _set.Search(new SimpleObjectWrapper(key))?.Data;
        }

        /// <summary>
        /// Deletes all items in the data structure, does not affect the database
        /// </summary>
        public void Clear()
        {
            _set.Clear();
        }

        /// <summary>
        /// Sets the database implementation
        /// </summary>
        /// <param name="db"></param>
        public void SetDb(IDatabase db)
        {
            _db = db;
        }

        /// <summary>
        /// Sets the hash function implementation used to calcualte hashes of data objects
        /// </summary>
        /// <param name="hashFunction"></param>
        public void SetHashFunction(IHashFunction hashFunction)
        {
            _hashFunction = hashFunction;
        }

        /// <summary>
        /// Sets the bifunctor implementation
        /// </summary>
        /// <param name="bifunctor"></param>
        public void SetBifunctor(IBifunctor bifunctor)
        {
            _bifunctor = bifunctor;
        }

        /// <summary>
        /// Sets the branching factor of the rbss protocol
        /// </summary>
        /// <param name="branchingFactor"></param>
        /// <exception cref="Exception"></exception>
        public void SetBranchingFactor(int branchingFactor)
        {
            if (branchingFactor < 2) throw new Exception("Branching Factor is not supported: " + branchingFactor);
            _branching = branchingFactor;
        }
        /// <summary>
        /// Initializes the datastructure by adding all objects contained within the databse
        /// </summary>
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
