using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using Org.OpenAPIToolsServer.Controllers;

namespace RBSS_CS.Controllers
{
    public class PeerNetworkApi : PeerNetworkApiController
    {
        public override IActionResult ExitPost(Successor successor)
        {
            throw new NotImplementedException();
        }

        public override IActionResult JoinPost(Joining joining)
        {
            throw new NotImplementedException();
        }

        public override IActionResult NotifyPost(Predecessor predecessor)
        {
            throw new NotImplementedException();
        }
    }
}
