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
using Models.RBSS_CS;
using Org.OpenAPIToolsServer.Attributes;

namespace Org.OpenAPIToolsServer.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public abstract class SyncApiController : ControllerBase
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Checks if the fingerprint of own data matches the given fingerprint and optionally starts an asynchronous process to handle subset syncronization, if not</remarks>
        /// <param name="validateStep"></param>
        /// <response code="200">Returns information neighter an sync process is started or sync is done</response>
        [HttpPost]
        [Route("/sync")]
        [Consumes("application/json")]
        [ValidateModelState]
        [ProducesResponseType(statusCode: 200, type: typeof(SyncState))]
        public abstract IActionResult SyncPost([FromBody]ValidateStep validateStep);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Checks required actions for given list of sync steps</remarks>
        /// <param name="syncState"></param>
        /// <response code="200">Returns information neighter an sync process is started or sync is done</response>
        [HttpPut]
        [Route("/sync")]
        [Consumes("application/json")]
        [ValidateModelState]
        [ProducesResponseType(statusCode: 200, type: typeof(SyncState))]
        public abstract IActionResult SyncPut([FromBody]SyncState syncState);
    }
}
