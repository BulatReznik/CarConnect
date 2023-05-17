using CarConnectContracts.BindingModels;
using CarConnectContracts.BusinessLogicsContracts;
using CarConnectContracts.StorageContracts;
using CarConnectContracts.ViewModels;
using CarConnectDataBase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CarConnectDataBase.Implements
{
    public class ReviewStorage : IReviewStorage
    {
        public List<ReviewViewModel> GetFullList()
        {
            using var context = new CarConnectDataBase();
            return context.Reviews
                .Include(r => r.User) // Загрузка связанного объекта User
                .ToList()
                .Select(CreateModel)
                .ToList();
        }
        public List<ReviewViewModel> GetFiltredList(ReviewBindingModel model)
        {
            if (model == null)
            {
                return null;
            }

            using var context = new CarConnectDataBase();

            return context.Reviews
                .Include(r => r.User) // Загрузка связанного объекта User
                .Where(rec => (rec.Text.Contains(model.Text)) || rec.UserId == model.UserId || rec.CarId == model.CarId)
                .ToList()
                .Select(CreateModel)
                .ToList();
        }

        public ReviewViewModel GetReview(ReviewBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new CarConnectDataBase();
            var review = context.Reviews
                .FirstOrDefault(rec => rec.Id == model.Id || rec.CarId == model.CarId || rec.UserId == model.UserId);
            return review != null ? CreateModel(review) : null;
        }
        public void Delete(ReviewBindingModel model)
        {
            using var context = new CarConnectDataBase();
            Review review = context.Reviews.FirstOrDefault(rec => rec.Id == model.Id);
            if (review != null)
            {
                context.Reviews.Remove(review);
                context.SaveChanges();
            }
            else
            {
                throw new Exception("Отзыв не найден");
            }
        }

        public void Insert(ReviewBindingModel model)
        {
            using var context = new CarConnectDataBase();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                Review review = new Review
                {
                    Text = model.Text,
                    UserId = model.UserId,
                    CarId = model.CarId,
                };
                context.Reviews.Add(review);
                context.SaveChanges();
                CreateModel(model, review, context);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void Update(ReviewBindingModel model)
        {
            using var context = new CarConnectDataBase();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var review = context.Reviews.FirstOrDefault(rec => rec.Id == model.Id);
                if (review == null) 
                {
                    throw new Exception("Отзыв не найден");
                }
                CreateModel(model, review, context);
                context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private Review CreateModel(ReviewBindingModel model, Review review, CarConnectDataBase context)
        {
            review.Text = model.Text;
            review.UserId = model.UserId;
            review.CarId = model.CarId;
            if (model.Id.HasValue)
            {
                context.SaveChanges();
            }
            return review;
        }

        private static ReviewViewModel CreateModel(Review review)
        {
            return new ReviewViewModel
            {
                Id = review.Id,
                Text = review.Text,
                CarId = review.CarId,
                UserId = review.UserId,
                UserName = review.User != null ? $"{review.User.FirstName} {review.User.LastName}" : string.Empty
            };
        }
    }
}
