using CarConnectContracts.BindingModels;
using CarConnectContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConnectContracts.StorageContracts
{
    public interface IReviewStorage
    {
        List<ReviewViewModel> GetFullList();
        List<ReviewViewModel> GetFiltredList(ReviewBindingModel model);
        ReviewViewModel GetReview(ReviewBindingModel model);
        void Insert(ReviewBindingModel model);
        void Update(ReviewBindingModel model);
        void Delete(ReviewBindingModel model);
    }
}
