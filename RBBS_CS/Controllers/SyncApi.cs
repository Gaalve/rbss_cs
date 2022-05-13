using Microsoft.AspNetCore.Mvc;
using Org.OpenAPIToolsServer.Attributes;
using Org.OpenAPIToolsServer.Controllers;
using Models.RBB_CS;

namespace RBBS_CS.Controllers
{
    [ApiController]
    public class SyncApi : SyncApiController
    {
        public override IActionResult SyncPost(ValidateStep validateStep)
        {
            throw new NotImplementedException();
        }

        public override IActionResult SyncPut(SyncState syncState)
        {
            throw new NotImplementedException();
        }
    }
}
