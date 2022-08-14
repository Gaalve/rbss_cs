using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using Org.OpenAPIToolsServer.Controllers;

namespace RBSS_CS.Controllers
{
    /// <summary>
    /// The Api to control joins and exits of other peers in a p2p ring structure 
    /// </summary>
    public class PeerNetworkApi : PeerNetworkApiController
    {
        /// <summary>
        /// Called when the successor peer gracefully leaves the p2p network.
        /// </summary>
        /// <param name="successor">the new successor of this peer</param>
        /// <response code="200"> successor was successfully added </response>
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

        /// <summary>
        /// Peer joins this node as a predecessor.
        /// </summary>
        /// <param name="joining"> IP of the other peer </param>
        /// <response code="200"> join operation was successful, successor is returned</response>
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

        /// <summary>
        /// notifies the successor of a successful join operation,
        /// this peer will add the received ip-address as a predecessor
        /// </summary>
        /// <param name="predecessor">the ip address of the predecessor </param>
        /// <returns></returns>
        public override IActionResult NotifyPost(Predecessor predecessor)
        {
            ClientMap.Instance.PredecessorClient = new Client(predecessor.PredecessorIP);
            return Ok();
        }
    }
}
