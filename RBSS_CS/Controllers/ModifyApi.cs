using DAL1.RBSS_CS;
using Microsoft.AspNetCore.Mvc;
using Models.RBSS_CS;
using Org.OpenAPIToolsServer.Controllers;

namespace RBSS_CS.Controllers
{
    /// <summary>
    /// The RESTful-Api to modify the data set of this node.
    /// </summary>
    public class ModifyApi : ModifyApiController
    {
        private readonly ServerSettings _settings;
        private readonly IPersistenceLayerSingleton _persistenceLayer;
        private readonly SyncApi _syncApi;
        /// <summary>
        /// The controller receives a settings object that is treated as immutable singleton object.
        /// The settings control configuration parameters of rbss protocol.
        /// The syncApi controller is used to start a synchronization process if new data is added and certain settings are matched.
        /// The persistenceLayer object specifies the type of persistence used. See <see cref="ServerSettings"/> for more information.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="persistence"></param>
        /// <param name="syncApi"></param>
        public ModifyApi(ServerSettings settings, IPersistenceLayerSingleton persistence, SyncApi syncApi)
        {
            _settings = settings;
            _persistenceLayer = persistence;
            _syncApi = syncApi;
        }

        /// <summary>
        /// The delete functionality is not implemented and clears the data set if called when the TestingMode is checked.
        /// </summary>
        /// <param name="simpleDataObject">the data to be deleted</param>
        /// <exception cref="NotImplementedException">throws if peer is not in testing mode</exception>
        public override IActionResult DeletePost(SimpleDataObject simpleDataObject)
        {
            if (_settings.TestingMode)
            {
                _persistenceLayer.Clear();
                return Ok();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Inserts or updates a data object
        /// </summary>
        /// <param name="simpleDataObject"></param>
        /// <returns> StatusCode = 200 if object is not within the set and was successfully added, else 409 </returns>
        public override IActionResult InsertPost(SimpleDataObject simpleDataObject)
        {
            simpleDataObject.Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            if (_persistenceLayer.Insert(simpleDataObject))
            {
                if(!_settings.UseIntervalForSync)ClientMap.Instance.SuccessorClient?.Synchronize(_syncApi, _persistenceLayer);
                return Ok();
            }
            return Conflict();
        }

        /// <summary>
        /// Updates a data object
        /// </summary>
        /// <param name="simpleDataObject">new data object</param>
        /// <returns>StatusCode = 200 if object is  within the set as was updated, else 409 </returns>
        public override IActionResult UpdatePost(SimpleDataObject simpleDataObject)
        {
            simpleDataObject.Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

            if (_persistenceLayer.GetDataObjects().Any(s => s.Id.Equals(simpleDataObject.Id)))
            {
                _persistenceLayer.Insert(simpleDataObject);
                if(!_settings.UseIntervalForSync)ClientMap.Instance.SuccessorClient?.Synchronize(_syncApi, _persistenceLayer);
                return Ok();
            }
            return Conflict();
        }
    }
}
