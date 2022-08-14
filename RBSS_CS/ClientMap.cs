namespace RBSS_CS;

/// <summary>
/// Store Predecessor, Successor and InternetAddress of this peer.
/// Used for the p2p ring structure.
/// </summary>
public sealed class ClientMap
{
    private static volatile ClientMap? _instance;
    private static readonly object SyncRoot = new();

    /// <summary>
    /// Used to store the local internet address.
    /// </summary>
    public Client? SelfClient;
    /// <summary>
    /// The successor of this peer
    /// </summary>
    public Client? SuccessorClient;
    /// <summary>
    /// The predecessor of this peer
    /// </summary>
    public Client? PredecessorClient;

    private ClientMap() {}

    /// <summary>
    /// Access to this class as a singleton. This peer only has one successor and predecessor.
    /// </summary>
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