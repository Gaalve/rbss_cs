/*
 * Range-Based Set Synchronization Framework
 *
 * This is a simple Framework to synchronize range-based sets.
 *
 * OpenAPI spec version: 0.1.0
 * Contact: u.kuehn@tu-berlin.de
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using IO.Swagger.Server.Attributes;
using IO.Swagger.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace IO.Swagger.Server.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class SyncApiController : ControllerBase
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Checks if the fingerprint of own data matches the given fingerprint and optionally starts an asynchronous process to handle subset syncronization, if not</remarks>
        /// <param name="body"></param>
        /// <response code="200">Returns information neighter an sync process is started or sync is done</response>
        [HttpPost]
        [Route("/sync")]
        [ValidateModelState]
        [SwaggerOperation("SyncPost")]
        [SwaggerResponse(statusCode: 200, type: typeof(InlineResponse200), description: "Returns information neighter an sync process is started or sync is done")]
        public virtual IActionResult SyncPost([FromBody]SyncBody1 body)
        { 
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(InlineResponse200));
            string exampleJson = null;
            exampleJson = "{\n  \"syncstate\" : \"\"\n}";
            
                        var example = exampleJson != null
                        ? JsonConvert.DeserializeObject<InlineResponse200>(exampleJson)
                        : default(InlineResponse200);            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Checks required actions for given list of sync steps</remarks>
        /// <param name="body"></param>
        /// <response code="200">Returns information neighter an sync process is started or sync is done</response>
        [HttpPut]
        [Route("/sync")]
        [ValidateModelState]
        [SwaggerOperation("SyncPut")]
        [SwaggerResponse(statusCode: 200, type: typeof(InlineResponse200), description: "Returns information neighter an sync process is started or sync is done")]
        public virtual IActionResult SyncPut([FromBody]SyncBody body)
        { 
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(InlineResponse200));
            string exampleJson = null;
            exampleJson = "{\n  \"syncstate\" : \"\"\n}";
            
                        var example = exampleJson != null
                        ? JsonConvert.DeserializeObject<InlineResponse200>(exampleJson)
                        : default(InlineResponse200);            //TODO: Change the data returned
            return new ObjectResult(example);
        }
    }
}
