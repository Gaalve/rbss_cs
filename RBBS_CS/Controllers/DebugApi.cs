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
        public async Task<IActionResult> DebugConnect(string inetAddress)
        {
            ClientMap.Instance.PeerClient = new Client(inetAddress);
            var range = PersistenceLayer.Instance.CreateRangeSet();
            var remoteResult = await ClientMap.Instance.PeerClient.SyncApi.SyncPostAsync(
                new ValidateStep(range.IdFrom, range.IdTo, range.Fingerprint));

            while (remoteResult.Steps is { Count: > 0 })
            {
                Client local = new Client("http://localhost:80/");
                var localResult = await local.SyncApi.SyncPutAsync(remoteResult);
                if (localResult.Steps.Count == 0) break;
                remoteResult = await ClientMap.Instance.PeerClient.SyncApi.SyncPutAsync(localResult);
            }

            return Ok();
        }

        [HttpPost]
        [Route("/debugSet")]
        [Consumes("application/json")]
        [ValidateModelState]
        public async Task<IActionResult> DebugCreateSet(bool isX0)
        {

            Client local = new Client("http://localhost:80/");
            if (isX0)
            {
                local.ModifyApi.InsertPost(new SimpleDataObject("bee", "string"));
                local.ModifyApi.InsertPost(new SimpleDataObject("cat", "string"));
                local.ModifyApi.InsertPost(new SimpleDataObject("doe", "string"));
                local.ModifyApi.InsertPost(new SimpleDataObject("eel", "string"));
                local.ModifyApi.InsertPost(new SimpleDataObject("fox", "string"));
                local.ModifyApi.InsertPost(new SimpleDataObject("hog", "string"));
            }
            else
            {
                local.ModifyApi.InsertPost(new SimpleDataObject("ape", "string"));
                local.ModifyApi.InsertPost(new SimpleDataObject("eel", "string"));
                local.ModifyApi.InsertPost(new SimpleDataObject("fox", "string"));
                local.ModifyApi.InsertPost(new SimpleDataObject("gnu", "string"));
            }
            

            return Ok();
        }
    }
}
