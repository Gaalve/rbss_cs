using DAL1.RBB_CS;
using DAL1.RBSS_CS;
using Microsoft.AspNetCore.Mvc;
using Models.RBB_CS;
using Org.OpenAPIToolsServer.Attributes;
using RBSS_CS;

namespace RBBS_CS.Controllers
{
    [ApiController]
    public class DebugApi : ControllerBase
    {
        private readonly ModifyApi _modifyApi;
        private readonly SyncApi _syncApi;

        public DebugApi(ModifyApi modifyApi, SyncApi syncApi)
        {
            _modifyApi = modifyApi;
            _syncApi = syncApi;
        }


        [HttpGet]
        [Route("/getset")]
        [Consumes("application/json")]
        [ValidateModelState]
        [ProducesResponseType(statusCode: 200, type: typeof(SimpleDataObject[]))]
        public IActionResult DebugGetSet()
        {
            return Ok(PersistenceLayer.Instance.GetDataObjects());
        }

        [HttpPost]
        [Route("/connect")]
        [Consumes("application/json")]
        [ValidateModelState]
        public async Task<IActionResult> DebugConnect(string inetAddress)
        {
            Console.WriteLine("Debug Start");
            var remoteClient = new Client(inetAddress);
            var range = PersistenceLayer.Instance.CreateRangeSet();
            var remoteResult = await remoteClient.SyncApi.SyncPostAsync(
                new ValidateStep(range.IdFrom, range.IdTo, range.Fingerprint));

            while (remoteResult.Steps is { Count: > 0 })
            {
                var localAction = _syncApi.SyncPut(remoteResult);

                if (localAction.GetType() != typeof(OkObjectResult))
                {
                    Console.WriteLine("Wrong Result Type returned.");
                    break;
                }

                var localResult = ((OkObjectResult)localAction).Value;

                if (localResult == null)
                {
                    Console.WriteLine("No object attached.");
                    break;
                }

                if (localResult.GetType() != typeof(SyncState))
                {
                    Console.WriteLine("Wrong object attached.");
                    break;
                }

                var localSyncState = (SyncState)localResult;

                if (localSyncState.Steps.Count == 0)
                {
                    break; // Sync finished
                }

                remoteResult = await remoteClient.SyncApi.SyncPutAsync(localSyncState);
            }
            Console.WriteLine("Debug Finished");
            return Ok();
        }

        [HttpPost]
        [Route("/debugSet")]
        [Consumes("application/json")]
        [ValidateModelState]
        public async Task<IActionResult> DebugCreateSet(bool isX0)
        {
            if (isX0)
            {
                _modifyApi.InsertPost(new SimpleDataObject("bee", "string"));
                _modifyApi.InsertPost(new SimpleDataObject("cat", "string"));
                _modifyApi.InsertPost(new SimpleDataObject("doe", "string"));
                _modifyApi.InsertPost(new SimpleDataObject("eel", "string"));
                _modifyApi.InsertPost(new SimpleDataObject("fox", "string"));
                _modifyApi.InsertPost(new SimpleDataObject("hog", "string"));
            }
            else
            {
                _modifyApi.InsertPost(new SimpleDataObject("ape", "string"));
                _modifyApi.InsertPost(new SimpleDataObject("eel", "string"));
                _modifyApi.InsertPost(new SimpleDataObject("fox", "string"));
                _modifyApi.InsertPost(new SimpleDataObject("gnu", "string"));
            }
            

            return Ok();
        }
    }
}
