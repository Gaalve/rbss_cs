namespace RBSS_CS;

public sealed class ClientMap
{
    private static volatile ClientMap? _instance;
    private static readonly object SyncRoot = new();

    public Client? SelfClient;
    public Client? SuccessorClient;
    public Client? PredecessorClient;

    private ClientMap() {}

    public static ClientMap Instance
    {
        get 
        {
            if (_instance == null) 
            {
                lock (SyncRoot)
                {
                    if (_instance == null) 
                        _instance = new ClientMap();
                }
            }

            return _instance;
        }
    }
}