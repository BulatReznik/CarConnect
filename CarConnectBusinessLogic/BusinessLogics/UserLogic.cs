using CarConnectContracts.BindingModels;
using CarConnectContracts.BusinessLogicsContracts;
using CarConnectContracts.StorageContracts;
using CarConnectContracts.ViewModels;
using System.Text.RegularExpressions;

namespace CarConnectBusinessLogic.BusinessLogics
{
    public class UserLogic : IUserLogic
    {
        private readonly IUserStorage _userStorage;
        private readonly int _passwordMaxLength = 50;
        private readonly int _passwordMinLength = 6;
        public UserLogic(IUserStorage userStorage)
        {
            _userStorage = userStorage;
        }


        public List<UserViewModel> Read(UserBindingModel model)
        {
            if (model == null)
            {
                return _userStorage.GetFullList();
            }
            if (model.Id.HasValue)
            {
                return new List<UserViewModel> { _userStorage.GetElement(model) };
            }
            return _userStorage.GetFilteredList(model);
        }

        public void CreateOrUpdate(UserBindingModel model)
        {
            var element = _userStorage.GetElement(new UserBindingModel
            {
                Email = model.Email
            });
            if (element != null && element.Id != model.Id)
            {
                throw new Exception("Уже есть пользователь с таким логином");
            }
            if (!Regex.IsMatch(model.Email, @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                            + "@"
                                            + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$"))
            {
                throw new Exception("В качестве логина почта указана должна быть");
            }
            if (model.Password.Length > _passwordMaxLength ||
                model.Password.Length < _passwordMinLength ||
                !Regex.IsMatch(model.Password, @"^((\w+\d+\W+)|(\w+\W+\d+)|(\d+\w+\W+)|(\d+\W+\w+)|(\W+\w+\d+)|(\W+\d+\w+))[\w\d\W]*$"))
            {
                throw new Exception($"Пароль длиной от {_passwordMinLength} до {_passwordMaxLength} должен быть и из цифр, букв и небуквенных символов должен состоять");
            }
            if (model.Id.HasValue)
            {
                _userStorage.Update(model);
            }
            else
            {
                _userStorage.Insert(model);
            }
        }

        public void Delete(UserBindingModel model)
        {
            var element = _userStorage.GetElement(new UserBindingModel
            {
                Id = model.Id
            });
            if (element == null)
            {
                throw new Exception("Пользователь не найден");
            }
            _userStorage.Delete(model);
        }
    }
}
