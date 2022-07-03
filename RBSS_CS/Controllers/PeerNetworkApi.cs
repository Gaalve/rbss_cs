using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using Org.OpenAPIToolsServer.Controllers;

namespace RBSS_CS.Controllers
{
    public class PeerNetworkApi : PeerNetworkApiController
    {
        public override IActionResult ExitPost(Successor successor)
        {
            var cm = ClientMap.Instance;
            if (cm.SuccessorClient == null && cm.PredecessorClient == null) return BadRequest();
            else if (cm.PredecessorClient != null && cm.SuccessorClient != null)
            {
                if (cm.SuccessorClient.Configuration.BasePath == successor.SuccessorIP)
                {
                    cm.SuccessorClient.PeerNetworkApi.NotifyPost(new Predecessor(cm.SelfClient!.Configuration.BasePath));
                    return Ok();
                }
                cm.SuccessorClient = new Client(successor.SuccessorIP);
                cm.SuccessorClient.PeerNetworkApi.NotifyPost(new Predecessor(cm.SelfClient!.Configuration.BasePath));
                return Ok();
            }
            return BadRequest();
        }

        public override IActionResult JoinPost(Joining joining)
        {
            var cm = ClientMap.Instance;

            if (cm.SuccessorClient == null && cm.PredecessorClient == null)
            {
                cm.PredecessorClient = new Client(joining.JoiningIP);
                cm.SuccessorClient = new Client(joining.JoiningIP);
                return Ok(new Successor(cm.SelfClient!.Configuration.BasePath));
            }
            else if (cm.PredecessorClient != null && cm.SuccessorClient != null)
            {
                if (cm.SuccessorClient.Configuration.BasePath == joining.JoiningIP)
                    return Ok(new Successor(joining.JoiningIP));

                var successor = new Successor(cm.SuccessorClient.Configuration.BasePath);
                cm.SuccessorClient = new Client(joining.JoiningIP);
                return Ok(successor);
            }
            return BadRequest();
        }

        public override IActionResult NotifyPost(Predecessor predecessor)
        {
            ClientMap.Instance.PredecessorClient = new Client(predecessor.PredecessorIP);
            return Ok();
        }
    }
}
