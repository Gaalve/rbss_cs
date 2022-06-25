using System.ComponentModel.DataAnnotations;

namespace RBSS_CS
{
    public class ServerSettings
    {
        /// <summary>
        /// Denotes if the server is running in testing mode or not.
        /// In testing mode the server will clear the whole set if a valid modify/deletePost request is sent.
        /// </summary>
        public bool TestingMode { get; set; } = false;

        /// <summary>
        /// Denotes if the server is running as the initiator in testing mode for integration tests.
        /// The server will exit when the integration tests are finished.
        /// </summary>
        public bool TestingModeInitiator { get; set; } = false;

        /// <summary>
        /// The assembly name of the auxillary data structure used in the persistence layer.
        /// The naming follows the convention of TopNamespace.SubNameSpace.ContainingClass+NestedClass (see https://docs.microsoft.com/en-us/dotnet/api/system.type.assemblyqualifiedname?view=net-6.0)
        /// The auxillary datastructure has to be loaded within the current AppDomain (see https://docs.microsoft.com/en-us/dotnet/api/system.appdomain.currentdomain?view=net-6.0)
        /// The datastructure has to implement the interface DAL1.RBSS_CS.IPersistenceLayer
        /// 
        /// </summary>
        public string AuxillaryDS { get; set; } = typeof(DAL1.RBSS_CS.RedBlackTreePersistence).FullName!;

        /// <summary>
        /// The minimum required elements to send an InsertStep. Has to bigger than 0.
        /// Default is 1.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int ItemSize { get; set; } = 1;

        /// <summary>
        /// The maximum allowed branches regarding an answer to the ValidationStep. Has to be bigger than 1.
        /// Default is 2.
        /// </summary>
        [Range(2, int.MaxValue)]
        public int BranchingFactor { get; set; } = 2;

        /// <summary>
        /// The assembly name of the hash function used to calculate the hash of Models.RBSS_CS.SimpleDataObjectt.
        /// The naming follows the convention of TopNamespace.SubNameSpace.ContainingClass+NestedClass (see https://docs.microsoft.com/en-us/dotnet/api/system.type.assemblyqualifiedname?view=net-6.0)
        /// The hash function has to be loaded within the current AppDomain (see https://docs.microsoft.com/en-us/dotnet/api/system.appdomain.currentdomain?view=net-6.0)
        /// 
        /// </summary>
        public string HashFunc { get; set; } = "";

        /// <summary>
        /// The assembly name of the bifunctor structure used for fingerprint calculations.
        /// The naming follows the convention of TopNamespace.SubNameSpace.ContainingClass+NestedClass (see https://docs.microsoft.com/en-us/dotnet/api/system.type.assemblyqualifiedname?view=net-6.0)
        /// The bifunctor has to be loaded within the current AppDomain (see https://docs.microsoft.com/en-us/dotnet/api/system.appdomain.currentdomain?view=net-6.0)
        /// 
        /// </summary>
        public string Bifunctor { get; set; } = "";

        /// <summary>
        /// Denotes if the peer should sync after a set interval or if the peer should sync when new items are added to set.
        /// </summary>
        public bool UseIntervalForSync { get; set; } = false;

        /// <summary>
        /// The number of milliseconds after which a peer should sync items with connected peers.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int SyncIntervalMS { get; set; } = 1000;

        /// <summary>
        /// The assembly name of the database used for data persistence.
        /// The naming follows the convention of TopNamespace.SubNameSpace.ContainingClass+NestedClass (see https://docs.microsoft.com/en-us/dotnet/api/system.type.assemblyqualifiedname?view=net-6.0)
        /// The database has to be loaded within the current AppDomain (see https://docs.microsoft.com/en-us/dotnet/api/system.appdomain.currentdomain?view=net-6.0)
        ///
        /// The value "none" disables data persistence.
        /// Default is "none".
        /// </summary>
        public string DBKind { get; set; } = "none";

        /// <summary>
        /// The config path of database. The config structure is defined by the databse implementation
        ///
        /// This value is ignored if DBKind is set to "none"
        /// </summary>
        public string DBConfigPath { get; set; } = "";

        /// <summary>
        /// Denotes the P2PStructure.
        ///
        /// Only "Ring" is valid
        /// </summary>
        public string P2PStructure { get; set; } = "Ring";


    }
}
