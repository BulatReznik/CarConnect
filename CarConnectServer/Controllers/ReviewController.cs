using CarConnectBusinessLogic.BusinessLogics;
using CarConnectContracts.BindingModels;
using CarConnectContracts.BusinessLogicsContracts;
using CarConnectContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CarConnectServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReviewController
    {
        private readonly IReviewLogic _reviewLogic;
        private readonly ICarLogic _carLogic;
        private readonly IUserLogic _userLogic;

        public ReviewController(IReviewLogic reviewLogic, ICarLogic carLogic, IUserLogic userLogic)
        {
            _reviewLogic = reviewLogic;
            _carLogic = carLogic;
            _userLogic = userLogic;
        }

        [HttpGet]
        public CarWithReviewsViewModel GetCarWithReviews(string LicensePlate)
        {
            var car = _carLogic.Read(new CarBindingModel { LicensePlate = LicensePlate });
            var carViewModel = car.FirstOrDefault(); // Получаем первый элемент списка CarViewModel

            if (carViewModel == null)
            {
                // Обработка случая, когда автомобиль не найден
                return null;
            }
            var reviews = _reviewLogic.Read(new ReviewBindingModel { CarId = carViewModel.Id }).ToList();
            var carWithReviews = new CarWithReviewsViewModel
            {
                CarId = carViewModel.Id,
                Brand = carViewModel.Brand,
                Model = carViewModel.Model,
                Year = carViewModel.Year,
                VIN = carViewModel.VIN,
                LicensePlate = carViewModel.LicensePlate,
                Colour = carViewModel.Colour,
                FileName = carViewModel.FileName,
                Path = carViewModel.Path,
                UserName = carViewModel.UserName,
                UserPhone = carViewModel.UserPhone,
                UserEmail = carViewModel.UserEmail,
                Reviews = reviews
            };

            return carWithReviews;
        }

        [HttpGet]
        public List<ReviewViewModel> GetReviewList() => _reviewLogic.Read(null)?.ToList();

        [HttpGet]
        public ReviewViewModel GetReview(int reviewId) => _reviewLogic.Read(new ReviewBindingModel { Id = reviewId })?[0];

        [HttpPost]
        public void CreateOrUpdateReview(ReviewBindingModel model) => _reviewLogic.CreateOrUpdate(model);

        [HttpPost]
        public void DeleteReview(ReviewBindingModel model) => _reviewLogic.Delete(model);

        [HttpGet]
        public List<ReviewViewModel> GetCarReviewList(int carId) => _reviewLogic.Read(new ReviewBindingModel { CarId = carId });


    }
}
