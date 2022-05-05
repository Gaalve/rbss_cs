using Microsoft.AspNetCore.Mvc;
using Org.OpenAPIToolsServer.Attributes;
using Org.OpenAPIToolsServer.Controllers;
using Org.OpenAPIToolsServer.Models;

namespace RBBS_CS.Controllers
{
    [ApiController]
    public class SyncApi : SyncApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Checks if the fingerprint of own data matches the given fingerprint and optionally starts an asynchronous process to handle subset syncronization, if not</remarks>
        /// <param name="inlineObject1"></param>
        /// <response code="200">Returns information neighter an sync process is started or sync is done</response>
        [HttpPost]
        [Route("/sync")]
        [Consumes("application/json")]
        [ValidateModelState]
        [ProducesResponseType(statusCode: 200, type: typeof(InlineResponse200))]
        public override IActionResult SyncPost(InlineObject1 inlineObject1)
        {

            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Checks required actions for given list of sync steps</remarks>
        /// <param name="inlineObject"></param>
        /// <response code="200">Returns information neighter an sync process is started or sync is done</response>
        [HttpPut]
        [Route("/sync")]
        [Consumes("application/json")]
        [ValidateModelState]
        [ProducesResponseType(statusCode: 200, type: typeof(InlineResponse200))]
        public override IActionResult SyncPut(InlineObject inlineObject)
        {

            Console.WriteLine(inlineObject.ToString());

            throw new NotImplementedException();
        }
    }
}
