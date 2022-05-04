/*
 * Range-Based Set Synchronization Framework
 *
 * This is a simple Framework to synchronize range-based sets.
 *
 * The version of the OpenAPI document: 0.1.0
 * Contact: u.kuehn@tu-berlin.de
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Org.OpenAPIToolsServer.Attributes;
using Org.OpenAPIToolsServer.Models;

namespace Org.OpenAPIToolsServer.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public abstract class ModifyApiController : ControllerBase
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>(Not to be implement yet) Deletes an existing data item into the data set and starts asynchronous synchronization with peer</remarks>
        /// <param name="inlineObject4"></param>
        /// <response code="200">Returns information about success by deleting data</response>
        [HttpPost]
        [Route("/delete")]
        [Consumes("application/json")]
        [ValidateModelState]
        public abstract IActionResult DeletePost([FromBody]InlineObject4 inlineObject4);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Inserts a new data item into the data set and starts asynchronous synchronization with peer</remarks>
        /// <param name="inlineObject2"></param>
        /// <response code="200">Returns information about success by insterting data</response>
        [HttpPost]
        [Route("/insert")]
        [Consumes("application/json")]
        [ValidateModelState]
        public abstract IActionResult InsertPost([FromBody]InlineObject2 inlineObject2);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>(Not to be implement yet) Updates an existing data item into the data set and starts asynchronous synchronization with peer</remarks>
        /// <param name="inlineObject3"></param>
        /// <response code="200">Returns information about success by updating data</response>
        [HttpPost]
        [Route("/update")]
        [Consumes("application/json")]
        [ValidateModelState]
        public abstract IActionResult UpdatePost([FromBody]InlineObject3 inlineObject3);
    }
}
