﻿using DAL1.RBSS_CS;
using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using Org.OpenAPIToolsServer.Attributes;

namespace RBSS_CS.Controllers
{
    [ApiController]
    public class DebugApi : ControllerBase
    {
        private readonly ServerSettings _settings;
        private readonly ModifyApi _modifyApi;
        private readonly SyncApi _syncApi;
        private readonly IPersistenceLayerSingleton _persistenceLayer;

        public DebugApi(ModifyApi modifyApi, SyncApi syncApi, IPersistenceLayerSingleton persistence, ServerSettings settings)
        {
            _modifyApi = modifyApi;
            _syncApi = syncApi;
            _persistenceLayer = persistence;
            _settings = settings;
        }


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
            if (!_settings.TestingMode) return Forbid();
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

        [HttpPost]
        [Route("/debugClear")]
        [Consumes("application/json")]
        [ValidateModelState]
        public async Task<IActionResult> DebugClearSet()
        {
            if (!_settings.TestingMode) return Forbid();
            _persistenceLayer.Clear();
            return Ok();
        }

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
