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
        public ModifyApi(ServerSettings settings, IPersistenceLayerSingleton persistence)
        {
            _settings = settings;
            _persistenceLayer = persistence;
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
            if (_persistenceLayer.Insert(simpleDataObject)) return Ok();
            return Conflict();
        }

        public override IActionResult UpdatePost(SimpleDataObject simpleDataObject)
        {
            throw new NotImplementedException();
        }
    }
}
