using Models.RBSS_CS;

namespace DAL1.RBSS_CS
{
    public sealed class PersistenceLayer<T> : IPersitenceLayerSingleton where T : IPersistenceLayer, new()
    {
        // private static readonly Lazy<PersistenceLayer> Lazy = new(() => new PersistenceLayer());
        // public static PersistenceLayer Instance => Lazy.Value;
        // private readonly SortedSet<SimpleObjectWrapper> _set;
        //
        // private PersistenceLayer()
        // {
        //     _set = new SortedSet<SimpleObjectWrapper>();
        // }

        private static volatile PersistenceLayer<T>? _instance;
        private static readonly object SyncRoot = new();
        private readonly IPersistenceLayer _auxillaryDs;
        

        private PersistenceLayer()
        {
            _auxillaryDs = new T();
        }

        public static PersistenceLayer<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new PersistenceLayer<T>();
                        }
                    }
                }
                return _instance;
            }
        }


        public int GetFingerprint(string lower, string upper)
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

        public void Clear()
        {
            _auxillaryDs.Clear();
        }
    }

    
}