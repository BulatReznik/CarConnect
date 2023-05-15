using CarConnectContracts.BindingModels;
using CarConnectContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConnectContracts.StorageContracts
{
    public interface ICarStorage
    {
        List<CarViewModel> GetFullList();
        List<CarViewModel> GetFiltredList(CarBindingModel model);
        CarViewModel GetCar(CarBindingModel model);
        void Insert(CarBindingModel model);
        void Update(CarBindingModel model);
        void Delete(CarBindingModel model);

    }
}
