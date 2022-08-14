using DAL1.RBSS_CS;
using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using Org.OpenAPIToolsServer.Attributes;

namespace RBSS_CS.Controllers
{
    /// <summary>
    /// The DebugApi controls certain functionality to allow developers easy access to data and methods
    /// required to perform unit tests or integration tests.
    /// RESTful-API paths are only callable if the server is set to TestingMode.
    /// </summary>
    [ApiController]
    public class DebugApi : ControllerBase
    {
        private readonly ServerSettings _settings;
        private readonly ModifyApi _modifyApi;
        private readonly SyncApi _syncApi;
        private readonly IPersistenceLayerSingleton _persistenceLayer;

        /// <summary>
        /// The controller receives a settings object that is treated as immutable singleton object.
        /// The settings control configuration parameters of the rbss protocol.
        /// The modifyApi controller object is used to modify the data set.
        /// The syncApi controller object is used to start synchronization processes.
        /// The persistenceLayer object specifies the type of persistence used. See <see cref="ServerSettings"/> for more information.
        ///
        /// </summary>
        /// <param name="modifyApi"></param>
        /// <param name="syncApi"></param>
        /// <param name="persistence"></param>
        /// <param name="settings"></param>
        public DebugApi(ModifyApi modifyApi, SyncApi syncApi, IPersistenceLayerSingleton persistence, ServerSettings settings)
        {
            _modifyApi = modifyApi;
            _syncApi = syncApi;
            _persistenceLayer = persistence;
            _settings = settings;
        }


        /// <summary>
        /// Returns the data set as an array
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/getset")]
        [Consumes("application/json")]
        [ValidateModelState]
        [ProducesResponseType(statusCode: 200, type: typeof(SimpleDataObject[]))]
        public IActionResult DebugGetSet()
        {
            if (!_settings.TestingMode) return Forbid();
            return Ok(_persistenceLayer.GetDataObjects());
        }

        /// <summary>
        /// Allows for multiple data objects to be inserted.
        /// Is faster than calling ModifyApi.Insert multiple times.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/batchInsert")]
        [Consumes("application/json")]
        [ValidateModelState]
        public IActionResult DebugBatchInsert(DebugInsert data)
        {
            if (!_settings.TestingMode) return Forbid();
            foreach (var d in data.DataSet)
            {
                _persistenceLayer.Insert(d);
            }
            return Ok();
        }

        /// <summary>
        /// Sets the successor to a specific internet address
        /// </summary>
        /// <param name="inetAddress"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/connect")]
        [Consumes("application/json")]
        [ValidateModelState]
        public async Task<IActionResult> DebugConnect(string inetAddress)
        {
            if (!_settings.TestingMode) return Forbid();
            Console.WriteLine("Debug Start");
            var remoteClient = new Client(inetAddress);
            var range = _persistenceLayer.CreateRangeSet();
            var remoteResult = (await remoteClient.SyncApi.SyncPostAsync(
                new ValidateStep(range.IdFrom, range.IdTo, range.Fingerprint))).Syncstate;

            while (remoteResult.Steps is { Count: > 0 })
            {
                var localAction = _syncApi.SyncPut(new InlineResponse(remoteResult));

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

                remoteResult = (await remoteClient.SyncApi.SyncPutAsync(new InlineResponse(localSyncState))).Syncstate;
            }
            Console.WriteLine("Debug Finished");
            return Ok();
        }


        /// <summary>
        /// Clears the data set of this peer.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/debugReset")]
        [Consumes("application/json")]
        [ValidateModelState]
        public IActionResult DebugReset()
        {
            if (!_settings.TestingMode) return Forbid();
            _persistenceLayer.Clear();
            return Ok();
        }

        /// <summary>
        /// Searches for a specific ID and returns the data object.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/debugSearch")]
        [Consumes("application/json")]
        [ValidateModelState]
        [ProducesResponseType(statusCode: 200, type: typeof(SimpleDataObject))]
        public async Task<IActionResult> DebugSearch(string key)
        {
            if (!_settings.TestingMode) return Forbid();
            var item = _persistenceLayer.Search(key);
            if (item == null) return BadRequest();
            return Ok(item);
        }
    }
}
