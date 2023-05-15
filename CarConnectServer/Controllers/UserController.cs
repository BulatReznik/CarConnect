using CarConnectContracts.BindingModels;
using CarConnectContracts.BusinessLogicsContracts;
using CarConnectContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CarConnectServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserLogic _userLogic;
        public UserController(IUserLogic userLogic) {
            _userLogic = userLogic;
        }

        [HttpGet]
        public UserViewModel Login(string Email, string Password)
        {
            var list = _userLogic.Read(new UserBindingModel
            {
                Email = Email,
                Password = Password
            });
            return (list != null && list.Count > 0) ? list[0] : null;
        }

        [HttpPost]
        public void Register(UserBindingModel model) => _userLogic.CreateOrUpdate(model);
        
        [HttpPost]
        public void UpdateData(UserBindingModel model) => _userLogic.CreateOrUpdate(model);

    }
}
