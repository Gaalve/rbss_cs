using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Xunit.Sdk;

namespace RBSS_CS;

public class Client
{
    public Configuration Configuration { get;  }

    public ModifyApi ModifyApi { get; }
    public SyncApi SyncApi { get; }

    public PeerNetworkApi PeerNetworkApi { get; }

    public Client(string inetAddress)
    {
        Configuration = new Configuration
        {
            BasePath = inetAddress
        };
        ModifyApi = new ModifyApi(Configuration);
        SyncApi = new SyncApi(Configuration);
        PeerNetworkApi = new PeerNetworkApi(Configuration);
    }

    
}