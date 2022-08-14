using DAL1.RBSS_CS;
using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPIToolsServer.Controllers;
using Xunit.Sdk;

namespace RBSS_CS;

/// <summary>
/// A client is used to connect to other peers.
/// Creates Client-RESTful-API-controllers to allow for easy calling of controller routines.
/// </summary>
public class Client
{
    /// <summary>
    /// The configuration of the client. Contains the internet address.
    /// </summary>
    public Configuration Configuration { get;  }

    /// <summary>
    /// The ModifyApi to modify objects on the connected peer. <see cref="Controllers.ModifyApi"/>
    /// </summary>
    public ModifyApi ModifyApi { get; }
    /// <summary>
    /// The SyncApi to synchronize objects with the connected peer. <see cref="Controllers.SyncApi"/>
    /// </summary>
    public SyncApi SyncApi { get; }

    /// <summary>
    /// The PeerNetworkApi to connect and disconnect from connected peer. <see cref="Controllers.PeerNetworkApi"/>
    /// </summary>
    public PeerNetworkApi PeerNetworkApi { get; }

    /// <summary>
    /// A client is created by providing a internet address.
    /// </summary>
    /// <param name="inetAddress"></param>
    public Client(string inetAddress)
    {
        try
        {
            var uri = new Uri(inetAddress);
            if (uri.Scheme is not ("http" or "https")) inetAddress = "http://" + inetAddress;
        }
        catch (System.UriFormatException ex)
        {
            inetAddress = "http://" + inetAddress;
        }
        
        Configuration = new Configuration
        {
            BasePath = inetAddress
        };
        ModifyApi = new ModifyApi(Configuration);
        SyncApi = new SyncApi(Configuration);
        PeerNetworkApi = new PeerNetworkApi(Configuration);
    }

    private SyncState InitiateSync(IPersistenceLayerSingleton localPersistence)
    {
        var range = localPersistence.CreateRangeSet();
        return SyncApi.SyncPost(new ValidateStep(range.IdFrom, range.IdTo, range.Fingerprint)).Syncstate;
    }


    private static SyncState? GetLocalResult(SyncApiController localSyncApi, SyncState remoteResult)
    {
        var localAction = localSyncApi.SyncPut(new InlineResponse(remoteResult));

        if (localAction.GetType() != typeof(OkObjectResult))
        {
            throw new Exception("Wrong Result Type returned.");
        }

        var localResult = ((OkObjectResult)localAction).Value;

        if (localResult == null)
        {
            throw new Exception("No object attached.");
        }

        if (localResult.GetType() != typeof(SyncState))
        {
            throw new Exception("Wrong object attached.");
        }

        return (SyncState)localResult;
    }
    /// <summary>
    /// Starts an asynchronous process to synchronize the set with the connected client.
    /// </summary>
    /// <param name="localSyncApi">Reference to the local syncApi</param>
    /// <param name="localPersistence">Reference to the local persistence layer</param>
    public void Synchronize(Controllers.SyncApi  localSyncApi, IPersistenceLayerSingleton localPersistence)
    {
        Task.Run(() =>
        {
            var remoteResult = InitiateSync(localPersistence);
            while (remoteResult.Steps is { Count: > 0 })
            {
                var localSyncState = GetLocalResult(localSyncApi, remoteResult);
                if (localSyncState!.Steps.Count == 0) break;
                remoteResult = SyncApi.SyncPut(new InlineResponse(localSyncState)).Syncstate;
            }
        });
        
    }
    
}