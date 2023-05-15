using CarConnectContracts.BusinessLogicsContracts;
using Microsoft.AspNetCore.Mvc;

namespace CarConnectServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RecognationController
    {
        private readonly IRecognationLogic _recognationLogic;
        public RecognationController(IRecognationLogic recognationLogic) {
            _recognationLogic = recognationLogic;
        }

        [HttpGet]
        public List<string> GetRecognation() => _recognationLogic.GetRecognations(); 
    }
}
