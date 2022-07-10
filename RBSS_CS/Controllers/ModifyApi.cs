using DAL1.RBSS_CS;
using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using Org.OpenAPIToolsServer.Controllers;

namespace RBSS_CS.Controllers
{
    public class ModifyApi : ModifyApiController
    {
        private readonly ServerSettings _settings;
        private readonly IPersistenceLayerSingleton _persistenceLayer;
        private readonly SyncApi _syncApi;
        public ModifyApi(ServerSettings settings, IPersistenceLayerSingleton persistence, SyncApi syncApi)
        {
            _settings = settings;
            _persistenceLayer = persistence;
            _syncApi = syncApi;
        }

        public override IActionResult DeletePost(SimpleDataObject simpleDataObject)
        {
            if (_settings.TestingMode)
            {
                _persistenceLayer.Clear();
                return Ok();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override IActionResult InsertPost(SimpleDataObject simpleDataObject)
        {
            if (_persistenceLayer.Insert(simpleDataObject))
            {
                if(!_settings.UseIntervalForSync)ClientMap.Instance.SuccessorClient?.Synchronize(_syncApi, _persistenceLayer);
                return Ok();
            }
            return Conflict();
        }

        public override IActionResult UpdatePost(SimpleDataObject simpleDataObject)
        {
            throw new NotImplementedException();
        }
    }
}
