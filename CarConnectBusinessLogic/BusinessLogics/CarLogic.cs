using CarConnectContracts.BindingModels;
using CarConnectContracts.BusinessLogicsContracts;
using CarConnectContracts.StorageContracts;
using CarConnectContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConnectBusinessLogic.BusinessLogics
{
    public class CarLogic : ICarLogic
    {
        private readonly ICarStorage _carStorage;
        public CarLogic(ICarStorage carStorage)
        {
            _carStorage = carStorage;
        }
        public List<CarViewModel> Read(CarBindingModel model)
        {
            if (model == null)
            {
                return _carStorage.GetFullList();
            }
            if (model.Id.HasValue || !string.IsNullOrEmpty(model.LicensePlate))
            {
                return new List<CarViewModel> { _carStorage.GetCar(model) };
            }
            return _carStorage.GetFiltredList(model);
        }

        public void CreateOrUpdate(CarBindingModel model)
        {
            var car = _carStorage.GetCar(new CarBindingModel
            {
                LicensePlate = model.LicensePlate
            });
            if(car!=null && car.Id != model.Id)
            {
                throw new Exception("Автомобиль с таким номером уже есть");
            }
            if (model.Id.HasValue)
            {
                _carStorage.Update(model);
            }
            else
            {
                _carStorage.Insert(model);
            }
        }
      

        public void Delete(CarBindingModel model)
        {
            var car = _carStorage.GetCar(new CarBindingModel 
            {
                Id= model.Id 
            });
            if(car==null)
            {
                throw new Exception("Машина не найдена");
            }
            _carStorage.Delete(model);
        }

       
    }
}
