using DAL1.RBB_CS;
using Microsoft.AspNetCore.Mvc;
using Org.OpenAPIToolsServer.Controllers;
using Models.RBB_CS;

namespace RBBS_CS.Controllers
{
    public class ModifyApi : ModifyApiController
    {
        public override IActionResult DeletePost(SimpleDataObject simpleDataObject)
        {
            throw new NotImplementedException();
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
