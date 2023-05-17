using CarConnectContracts.BindingModels;
using CarConnectContracts.BusinessLogicsContracts;
using CarConnectContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CarConnectServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CarController
    {
        private readonly ICarLogic _carLogic;
        
        public CarController(ICarLogic carLogic)
        {
            _carLogic = carLogic;
        }

        [HttpGet]
        public List<CarViewModel> GetCarList() => _carLogic.Read(null)?.ToList();

        [HttpGet]
        public CarViewModel GetCar(int carId) => _carLogic.Read(new CarBindingModel { Id = carId })?[0];

        [HttpGet]
        public CarViewModel GetCarByPlate(string LicensePlate) => _carLogic.Read(new CarBindingModel { LicensePlate = LicensePlate })?[0];

        [HttpPost]
        public void CreateOrUpdateCar(CarBindingModel model) => _carLogic.CreateOrUpdate(model);

        [HttpPost]
        public void DeleteCar(CarBindingModel model) => _carLogic.Delete(model);

        [HttpGet]
        public List<CarViewModel> GetUserCarlist(int userId) => _carLogic.Read(new CarBindingModel { UserId = userId });
    }
}
