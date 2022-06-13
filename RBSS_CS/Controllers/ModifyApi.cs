using DAL1.RBSS_CS;
using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using Org.OpenAPIToolsServer.Controllers;

namespace RBSS_CS.Controllers
{
    public class ModifyApi : ModifyApiController
    {
        private readonly ServerSettings _settings;
        public ModifyApi(ServerSettings settings)
        {
            _settings = settings;
        }

        public override IActionResult DeletePost(SimpleDataObject simpleDataObject)
        {
            if (_settings.TestingMode)
            {
                PersistenceLayer.Instance.Clear();
                return Ok();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override IActionResult InsertPost(SimpleDataObject simpleDataObject)
        {
            if (PersistenceLayer.Instance.Insert(simpleDataObject)) return Ok();
            return Conflict();
        }

        public override IActionResult UpdatePost(SimpleDataObject simpleDataObject)
        {
            throw new NotImplementedException();
        }
    }
}
