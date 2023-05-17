using CarConnectContracts.BindingModels;
using CarConnectContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConnectContracts.BusinessLogicsContracts
{
    public interface IReviewLogic
    {
        List<ReviewViewModel> Read(ReviewBindingModel model);
        void CreateOrUpdate(ReviewBindingModel model);
        void Delete(ReviewBindingModel model);
    }
}
