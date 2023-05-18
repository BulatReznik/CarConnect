using CarConnectContracts.BindingModels;
using CarConnectContracts.BusinessLogicsContracts;
using CarConnectContracts.StorageContracts;
using CarConnectContracts.ViewModels;
using CarConnectDataBase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConnectDataBase.Implements
{
    public class CarStorage : ICarStorage
    {
        public List<CarViewModel> GetFullList()
        {
            using var context = new CarConnectDataBase();
            return context.Cars
                .Include(r => r.User) // Загрузка связанного объекта User
                .ToList()
                .Select(CreateModel)
                .ToList();

        }
        public List<CarViewModel> GetFiltredList(CarBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new CarConnectDataBase();
            return context.Cars
                .Include(r => r.User) // Загрузка связанного объекта User
                .ToList()
                .Where(rec => (rec.UserId == model.UserId))
                .Select(CreateModel)
                .ToList();
        }
        public CarViewModel GetCar(CarBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new CarConnectDataBase();
            var car = context.Cars
                 .FirstOrDefault(rec => rec.Id == model.Id || rec.LicensePlate == model.LicensePlate);
            return car != null ? CreateModel(car) : null;
        }
        public void Insert(CarBindingModel model)
        {
            using var context = new CarConnectDataBase();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                Car car = new()
                {
                    Brand = model.Brand,
                    Model = model.Model,
                    Year = model.Year,
                    VIN = model.VIN,
                    LicensePlate = model.LicensePlate,
                    Colour = model.Colour,
                    FileName = model.FileName,
                    Path = model.Path,
                    UserId = model.UserId,
                    UserName = model.UserName,
                    UserEmail = model.UserEmail,
                    UserPhone = model.UserPhone

                };
                context.Cars.Add(car);
                context.SaveChanges();
                CreateModel(model, car, context);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public void Update(CarBindingModel model)
        {
            using var context = new CarConnectDataBase();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var car = context.Cars.FirstOrDefault(rec => rec.Id == model.Id);
                if (car == null)
                {
                    throw new Exception("Элемент не найден");
                }
                CreateModel(model, car, context);
                context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public void Delete(CarBindingModel model)
        {
            using var context = new CarConnectDataBase();
            Car car = context.Cars.FirstOrDefault(rec => rec.Id == model.Id);
            if (car != null)
            {
                context.SaveChanges();
                context.Cars.Remove(car);
                context.SaveChanges();
            }
            else
            {
                throw new Exception("Автомобиль не найден");
            }
        }
        private static Car CreateModel(CarBindingModel model, Car car, CarConnectDataBase context)
        {
            car.Brand = model.Brand;
            car.Model = model.Model;
            car.Year = model.Year;
            car.VIN = model.VIN;
            car.Colour = model.Colour;
            car.LicensePlate = model.LicensePlate;
            car.FileName = model.FileName;
            car.Path = model.Path;
            return car;

        }
        private static CarViewModel CreateModel(Car car)
        {
            return new CarViewModel
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                VIN = car.VIN,
                LicensePlate = car.LicensePlate,
                Colour = car.Colour,
                FileName = car.FileName,
                Path = car.Path,
                UserId = car.UserId,
                UserName = car.UserName,
                UserEmail = car.UserEmail,
                UserPhone = car.UserPhone
            };
        }
    }
}
