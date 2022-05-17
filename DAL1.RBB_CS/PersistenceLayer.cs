using Models.RBB_CS;

namespace DAL1.RBB_CS
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


        public int get_fingerprint(string lower, string upper)
        {
            if (string.Compare(lower, upper, StringComparison.Ordinal) > 0) return 0;
            int hash = 0;
            if (string.Compare(lower, upper, StringComparison.Ordinal) == 0)
            {
                foreach (var v in _set)
                {
                    hash ^= v.GetHashCode();
                }

                return hash;
            }

            var upperData = new SimpleObjectWrapper(upper);
            var subset = _set.GetViewBetween(new SimpleObjectWrapper(lower), upperData);

            foreach (var v in subset)
            {
                if (v.Hash != upperData.Hash)
                {
                    hash ^= v.GetHashCode();
                }
                
            }
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

    }

    
}