using DAL1.RBSS_CS.Bifunctors;
using DAL1.RBSS_CS.Databse;
using DAL1.RBSS_CS.Hashfunction;
using Models.RBSS_CS;

namespace DAL1.RBSS_CS
{
    public class PersistenceLayer<T> : IPersistenceLayerSingleton where T : IPersistenceLayer, new()
    {

        private readonly IPersistenceLayer _auxillaryDs;


        public PersistenceLayer(IDatabase database, IBifunctor bifunctor, IHashFunction hashFunction, int branchingFactor)
        {
            _auxillaryDs = new T();
            _auxillaryDs.SetDb(database);
            _auxillaryDs.SetBifunctor(bifunctor);
            _auxillaryDs.SetHashFunction(hashFunction);
            _auxillaryDs.SetBranchingFactor(branchingFactor);
        }

        /// <summary>
        /// Returns the fingerprint over the specified bound
        /// </summary>
        /// <param name="lower">lower bound, included</param>
        /// <param name="upper">upper bound, excluded</param>
        /// <returns></returns>
        public string GetFingerprint(string lower, string upper)
        {
            return _auxillaryDs.GetFingerprint(lower, upper);
        }

        /// <summary>
        /// Inserts or updates the specified element
        /// </summary>
        /// <param name="data">the object to be inserted or updated </param>
        /// <returns></returns>
        public bool Insert(SimpleDataObject data)
        {
            return _auxillaryDs.Insert(data);
        }

        /// <summary>
        /// Returns the full data set in ascending order
        /// </summary>
        /// <returns></returns>
        public SimpleDataObject[] GetDataObjects()
        {
            return _auxillaryDs.GetDataObjects();
        }

        /// <summary>
        /// Splits the specified range according to the specifed branching factor
        /// </summary>
        /// <param name="idFrom">lower bound, included</param>
        /// <param name="idTo">upper bound, excluded</param>
        /// <returns></returns>
        public RangeSet[] SplitRange(string idFrom, string idTo)
        {
            return _auxillaryDs.SplitRange(idFrom, idTo);
        }

        /// <summary>
        /// Creates a range set according to the specified bound
        /// </summary>
        /// <param name="idFrom">lower bound, included</param>
        /// <param name="idTo">upper bound, excluded</param>
        /// <returns></returns>
        public RangeSet CreateRangeSet(string idFrom, string idTo)
        {
            return _auxillaryDs.CreateRangeSet(idFrom, idTo);
        }

        /// <summary>
        /// Creates a range set according to the specified bound, but excludes the given objects
        /// </summary>
        /// <param name="idFrom">lower bound, included</param>
        /// <param name="idTo">upper bound, excluded</param>
        /// <param name="exclude">objects to be excluded from the range set</param>
        /// <returns></returns>
        public RangeSet CreateRangeSet(string idFrom, string idTo, ICollection<SimpleDataObject> exclude)
        {
            return _auxillaryDs.CreateRangeSet(idFrom, idTo, exclude);
        }

        /// <summary>
        /// Creates a range set over all elements
        /// </summary>
        /// <returns></returns>
        public RangeSet CreateRangeSet()
        {
            return _auxillaryDs.CreateRangeSet();
        }

        /// <summary>
        /// Searches for specific object and returns it
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SimpleDataObject? Search(string key)
        {
            return _auxillaryDs.Search(key);
        }

        /// <summary>
        /// Clears the auxiliary data structure
        /// </summary>
        public void Clear()
        {
            _auxillaryDs.Clear();
        }

        /// <summary>
        /// Initializes the auxiliary data structure
        /// </summary>
        public void Initialize()
        {
            _auxillaryDs.Initialize();
        }
    }

    
}