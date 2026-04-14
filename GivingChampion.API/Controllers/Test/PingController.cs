using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers.Test.v1
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    public class PingController : ControllerBase
    {

        public PingController()
        {
            
        }

        [HttpGet]
        public string Get()
        {
            return "Pong";
        }
    }
}
