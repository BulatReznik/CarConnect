using CarConnectContracts.BindingModels;
using CarConnectContracts.StorageContracts;
using CarConnectContracts.ViewModels;
using CarConnectDataBase.Models;

namespace CarConnectDataBase.Implements
{
    public class UserStorage : IUserStorage
    {
        public List<UserViewModel> GetFullList()
        {
            using var context = new CarConnectDataBase();
            return context.Users
                .Select(CreateModel)
                .ToList();
        }
        public List<UserViewModel> GetFilteredList(UserBindingModel model)
        {
            if (model == null)
            {
                return null;
            }

            using var context = new CarConnectDataBase();
            return context.Users
                .Where(rec => rec.Email == model.Email && rec.Password == rec.Password)
                .Select(CreateModel)
                .ToList();
        }
        public UserViewModel GetElement(UserBindingModel model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new CarConnectDataBase();
            var user = context.Users
                .FirstOrDefault(rec => rec.Id == model.Id || rec.Email == model.Email);
            return user != null ? CreateModel(user) : null;
        }
        public void Insert(UserBindingModel model)
        {
            using var context = new CarConnectDataBase();
            context.Users.Add(CreateModel(model, new User()));
            context.SaveChanges();
        }
        public void Update(UserBindingModel model)
        {
            using var context = new CarConnectDataBase();
            var user = context.Users
                .FirstOrDefault(rec => rec.Id == model.Id);
            if (user == null)
            {
                throw new Exception("Пользователь не найден");
            }
            CreateModel(model, user);
            context.SaveChanges();
        }
        public void Delete(UserBindingModel model)
        {
            using var context = new CarConnectDataBase();
            var user = context.Users.FirstOrDefault(rec => rec.Id == model.Id);
            if (user != null)
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
            else
            {
                throw new Exception("Пользователь не найден");
            }
        }

        private User CreateModel(UserBindingModel model, User user)
        {
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.MidleName = model.MiddleName;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            user.Password = model.Password;
            user.Gender = model.Gender;
            return user;
        }

        private static UserViewModel CreateModel(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MidleName = user.MidleName,
                Email = user.Email,
                Password = user.Password,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender
            };
        }
    }
}
