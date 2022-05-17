using DAL1.RBB_CS;
using Microsoft.AspNetCore.Mvc;
using Models.RBB_CS;
using Org.OpenAPIToolsServer.Attributes;

namespace RBBS_CS.Controllers
{
    [ApiController]
    public class DebugApi : ControllerBase
    {
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
        public IActionResult DebugConnect(string inetAddress)
        {
            ClientMap.Instance.PeerClient = new Client(inetAddress);
            return Ok();
        }
    }
}
