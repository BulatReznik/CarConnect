using CarConnectContracts.BindingModels;
using CarConnectContracts.BusinessLogicsContracts;
using CarConnectContracts.StorageContracts;
using CarConnectContracts.ViewModels;
using CarConnectDataBase.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConnectBusinessLogic.BusinessLogics
{
    public class ReviewLogic: IReviewLogic
    {
        private readonly IReviewStorage _reviewStorage;

        public ReviewLogic(IReviewStorage reviewStorage)
        {
            _reviewStorage = reviewStorage;
        }

        public void CreateOrUpdate(ReviewBindingModel model)
        {
            var review = _reviewStorage.GetReview(new ReviewBindingModel { CarId = model.CarId, UserId = model.UserId });
            if (review != null && review.Id != review.Id)
            {
                throw new Exception("Уже есть такой отзыв");
            }
            if(model.Id.HasValue)
            {
                _reviewStorage.Update(model);
            }
            else
            {
                _reviewStorage.Insert(model);
            }
        }

        public void Delete(ReviewBindingModel model)
        {
            var review = _reviewStorage.GetReview(new ReviewBindingModel { Id = model.Id });
            if (review == null)
            {
                throw new Exception("Отзыв не найден");
            }
            _reviewStorage.Delete(model);
        }

        public List<ReviewViewModel> Read(ReviewBindingModel model)
        {
            if(model == null)
            {
                return _reviewStorage.GetFullList();
            }
            if(model.Id.HasValue)
            {
                return new List<ReviewViewModel> { _reviewStorage.GetReview(model) };
            }
            return _reviewStorage.GetFiltredList(model);
        }
    }
}
