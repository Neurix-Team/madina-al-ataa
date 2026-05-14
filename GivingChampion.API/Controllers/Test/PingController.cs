using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers.Test.v1
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    /// <summary>
    /// Handles HTTP requests for Ping.
    /// </summary>
    public class PingController : ControllerBase
    {

        /// <summary>
        /// Performs the PingController operation.
        /// </summary>
        /// <returns>The result produced by the operation.</returns>
        public PingController()
        {
            
        }

        [HttpGet]
        /// <summary>
        /// Performs the Get operation.
        /// </summary>
        /// <returns>The result produced by the operation.</returns>
        public string Get()
        {
            return "Pong";
        }
    }
}
