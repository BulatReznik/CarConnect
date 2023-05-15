using CarConnect.Models;
using CarConnectContracts.BindingModels;
using CarConnectContracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Xml.Linq;


namespace CarConnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }
        public IActionResult Search()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult CarCreate()
        {
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Enter()
        {
            return View();
        }

        [HttpPost]
        public void Enter(string email, string password)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                Program.User = APIClient.GetRequest<UserViewModel>($"api/user/login?email={email}&password={password}");
                if (Program.User == null)
                {
                    throw new Exception("Неверный логин/пароль");
                }
                Response.Redirect("Index");
                return;
            }
            throw new Exception("Введите логин, пароль");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public void Register(string Email, string Password, string FirstName, string LastName, string MidleName, bool Gender, string PhoneNumber)
        {
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(FirstName))
            {
                APIClient.PostRequest("api/user/register", new UserBindingModel
                {
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    MiddleName = MidleName,
                    Password = Password,
                    PhoneNumber = PhoneNumber,
                    Gender = Gender
                }); ;
                Response.Redirect("Enter");
                return;
            }
            throw new Exception("Введите логин, пароль и ФИО");
        }

        public IActionResult Cars()
        {
            return View(APIClient.GetRequest<List<CarViewModel>>($"api/car/getcarlist"));
        }

        [HttpPost]
        public void CarCreate(string Brand, string Model, int Year,
            string VIN, string LicensePlate, string Colour, IFormFile Files)
        {
            if (!string.IsNullOrEmpty(Brand))
            {
                DateTime date = new DateTime(Year, 1, 1);
                //Getting FileName
                var fileName = Path.GetFileName(Files.FileName);
                //Getting file Extension
                var fileExtension = Path.GetExtension(fileName);
                // concatenating  FileName + FileExtension
                var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

                CarBindingModel car = new CarBindingModel {
                    Brand = Brand,
                    Model = Model,
                    Year = date,
                    VIN = VIN,
                    LicensePlate = LicensePlate,
                    Colour = Colour,
                    Name = newFileName,
                    FileType = fileExtension
                };
                using (var target = new MemoryStream())
                {
                    Files.CopyTo(target);
                    car.DataFiles = target.ToArray();
                }

                APIClient.PostRequest("api/car/createorupdatecar", car);

                
                Response.Redirect("Index");
                return;
            }
            throw new Exception("Вы пропустили поле");
        }

        [HttpGet]
        public IActionResult CarUpdate(int carId)
        {
            ViewBag.Car = APIClient.GetRequest<CarViewModel>($"api/car/getcar?carId={carId}");
            return View();
        }

        [HttpPost]
        public void CarUpdate(int carId, string Brand, string Model, int Year, 
            string VIN, string LicensePlate, string Colour, IFormFile Files)
        {
            if(!string.IsNullOrEmpty(Brand))
            {
                DateTime date = new DateTime(Year, 1, 1);
                var car = APIClient.GetRequest<CarViewModel>($"api/car/getcar?carId={carId}");
                if (car == null)
                {
                    return;
                }
                //Getting FileName
                var fileName = Path.GetFileName(Files.FileName);
                //Getting file Extension
                var fileExtension = Path.GetExtension(fileName);
                // concatenating  FileName + FileExtension
                var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);
                CarBindingModel newcar = new CarBindingModel
                {
                    Brand = Brand,
                    Model = Model,
                    Year = date,
                    VIN = VIN,
                    LicensePlate = LicensePlate,
                    Colour = Colour,
                    Name = newFileName,
                    FileType = fileExtension
                };
                using (var target = new MemoryStream())
                {
                    Files.CopyTo(target);
                    car.DataFiles = target.ToArray();
                }

                APIClient.PostRequest("api/car/createorupdatecar", newcar);
                Response.Redirect("Cars");
                return;
            }
            throw new Exception("Вы ввели не все данные!");
        }

        [HttpGet]
        public void CarDelete(int carId)
        {
            var car = APIClient.GetRequest<CarViewModel>($"api/car/getcar?carId={carId}");
            APIClient.PostRequest("api/car/deletecar", car);
            Response.Redirect("Cars");
        }




    }
}