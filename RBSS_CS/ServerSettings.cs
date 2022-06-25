using System.ComponentModel.DataAnnotations;

namespace RBSS_CS
{
    public class ServerSettings
    {
        public bool TestingMode { get; set; } = false;
        public bool TestingModeInitiator { get; set; } = false;
        public string AuxillaryDS { get; set; } = typeof(DAL1.RBSS_CS.RedBlackTreePersistence).FullName!;
        [Range(1, int.MaxValue)]
        public int ItemSize { get; set; } = 1;
        [Range(2, int.MaxValue)]
        public int BranchingFactor { get; set; } = 2;
        public string HashFunc { get; set; } = "";
        public string Bifunctor { get; set; } = "";
        public bool UseIntervalForSync { get; set; } = false;
        [Range(1, int.MaxValue)]
        public int SyncIntervalMS { get; set; } = 1000;
        public string DBKind { get; set; } = "";
        public string DBConfigPath { get; set; } = "";
        public string P2PStructure { get; set; } = "";

    }
}
