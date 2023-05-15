using CarConnectContracts.BindingModels;
using CarConnectContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConnectContracts.BusinessLogicsContracts
{
    public interface IUserLogic
    {
        List<UserViewModel> Read(UserBindingModel model);
        void CreateOrUpdate(UserBindingModel model);
        void Delete(UserBindingModel model);
    }
}
