using Microsoft.Extensions.Options;

namespace RBBS_CS
{
    public class ServerSettings
    {
        public const string Key = "Key";
        public bool TestingMode { get; set; } = false;
        public bool TestingModeInitiator { get; set; } = false;
    }
}
