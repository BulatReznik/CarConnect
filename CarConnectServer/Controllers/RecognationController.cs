using CarConnectContracts.BusinessLogicsContracts;
using Emgu.CV;
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

        [HttpPost]
        public void AddRecognition(UMat mat) => _recognationLogic.AddRecognations(mat);

        [HttpGet]
        public List<string> GetRecognation() => _recognationLogic.ReadRecognitions();
    }
}
