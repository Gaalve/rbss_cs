using DAL1.RBSS_CS;
using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPIToolsServer.Controllers;
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

    private SyncState InitiateSync(IPersistenceLayerSingleton localPersistence)
    {
        var range = localPersistence.CreateRangeSet();
        return SyncApi.SyncPost(new ValidateStep(range.IdFrom, range.IdTo, range.Fingerprint));
    }


    private static SyncState? GetLocalResult(SyncApiController localSyncApi, SyncState remoteResult)
    {
        var localAction = localSyncApi.SyncPut(remoteResult);

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
    public void Synchronize(Controllers.SyncApi  localSyncApi, IPersistenceLayerSingleton localPersistence)
    {
        Task.Run(() =>
        {
            var remoteResult = InitiateSync(localPersistence);
            while (remoteResult.Steps is { Count: > 0 })
            {
                var localSyncState = GetLocalResult(localSyncApi, remoteResult);
                if (localSyncState!.Steps.Count == 0) break;
                remoteResult = SyncApi.SyncPut(localSyncState);
            }
        });
        
    }
    
}