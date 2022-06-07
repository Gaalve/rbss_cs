using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;

namespace RBSS_CS;

public class Client
{
    private readonly Configuration _configuration;

    public ModifyApi ModifyApi { get; }
    public SyncApi SyncApi { get; }


    public Client(string inetAddress)
    {
        _configuration = new Configuration
        {
            BasePath = inetAddress
        };
        ModifyApi = new ModifyApi(_configuration);
        SyncApi = new SyncApi(_configuration);
    }
}