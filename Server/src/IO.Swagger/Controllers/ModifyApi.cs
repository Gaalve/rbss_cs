/*
 * Range-Based Set Synchronization Framework
 *
 * This is a simple Framework to synchronize range-based sets.
 *
 * OpenAPI spec version: 0.1.0
 * Contact: u.kuehn@tu-berlin.de
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using IO.Swagger.Server.Attributes;
using IO.Swagger.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IO.Swagger.Server.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class ModifyApiController : ControllerBase
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>(Not to be implement yet) Deletes an existing data item into the data set and starts asynchronous synchronization with peer</remarks>
        /// <param name="body"></param>
        /// <response code="200">Returns information about success by deleting data</response>
        [HttpPost]
        [Route("/delete")]
        [ValidateModelState]
        [SwaggerOperation("DeletePost")]
        public virtual IActionResult DeletePost([FromBody]DeleteBody body)
        { 
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);

            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Inserts a new data item into the data set and starts asynchronous synchronization with peer</remarks>
        /// <param name="body"></param>
        /// <response code="200">Returns information about success by insterting data</response>
        [HttpPost]
        [Route("/insert")]
        [ValidateModelState]
        [SwaggerOperation("InsertPost")]
        public virtual IActionResult InsertPost([FromBody]InsertBody body)
        { 
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);

            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>(Not to be implement yet) Updates an existing data item into the data set and starts asynchronous synchronization with peer</remarks>
        /// <param name="body"></param>
        /// <response code="200">Returns information about success by updating data</response>
        [HttpPost]
        [Route("/update")]
        [ValidateModelState]
        [SwaggerOperation("UpdatePost")]
        public virtual IActionResult UpdatePost([FromBody]UpdateBody body)
        { 
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);

            throw new NotImplementedException();
        }
    }
}
