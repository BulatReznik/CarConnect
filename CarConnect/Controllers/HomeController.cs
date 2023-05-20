using CarConnect.Models;
using CarConnectContracts.BindingModels;
using CarConnectContracts.ViewModels;
using Emgu.CV.CvEnum;
using Emgu.CV;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CarConnectBusinessLogic.BusinessLogics;

namespace CarConnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly CarCheck carCheck;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
            carCheck = new CarCheck();

        }

        public IActionResult Logout()
        {
            Program.User = null;
            return Redirect("~/Home/Enter");
        }

        [HttpGet]
        public IActionResult CarViewReview(string LicensePlate)
        {
            ViewBag.CarWithReviews = APIClient.GetRequest<CarWithReviewsViewModel>($"api/review/getcarwithreviews/?LicensePlate={LicensePlate}");
            return View();
        }

        [HttpPost]
        public IActionResult CarViewReview(string Text, int CarId, string LicensePlate)
        {
            var review = new ReviewBindingModel
            {
                UserId = Program.User.Id,
                CarId = CarId,
                Text = Text
            };
            APIClient.PostRequest("api/review/createorupdatereview", review);
            return RedirectToAction("CarViewReview", new { LicensePlate = LicensePlate });
        }

        public IActionResult Search()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CarView(string LicensePlate)
        {
            ViewBag.Car = APIClient.GetRequest<CarViewModel>($"api/car/getcarbyplate?LicensePlate={LicensePlate}");
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            if (Program.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            return View(Program.User);
        }

       

        [HttpPost]
        public void Privacy(string Email, string Password, string FirstName, string LastName, string MidleName, bool Gender, string PhoneNumber)
        {
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(FirstName))
            {
                APIClient.PostRequest("api/user/updatedata", new UserBindingModel
                {
                    Id = Program.User.Id,
                    FirstName = FirstName,
                    LastName = LastName,
                    MiddleName = MidleName,
                    Email = Email,
                    Password = Password,
                    PhoneNumber = PhoneNumber,
                    Gender = Gender
                });
                Program.User.Email = Email;
                Program.User.Password = Password;
                Program.User.FirstName = FirstName;
                Program.User.LastName = LastName;
                Program.User.MidleName = MidleName;
                Program.User.Gender = Gender;
                Program.User.PhoneNumber = PhoneNumber;
                Response.Redirect("Index");
                return;
            }
            throw new Exception("Введите логин, пароль, ФИО и номер телефона");
        }

        [HttpPost]
        public async Task<IActionResult> Search(IFormFile uploadedFile)
        {
            string str = "";
            if (uploadedFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await uploadedFile.CopyToAsync(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();

                    // Создание объекта Mat из массива байтов
                    Mat inputImage = new();
                    CvInvoke.Imdecode(fileBytes, ImreadModes.Color, inputImage);
                    UMat um = inputImage.GetUMat(AccessType.ReadWrite);
                    List<string> list = carCheck.ProcessImage(um);
                    str = string.Join(",", list);
                }
            }
            var routeValues = new RouteValueDictionary
                {
                    { "str", str }
                };
            return RedirectToAction("SearchGet", routeValues);
        }

        [HttpGet]
        public IActionResult SearchGet(string str)
        {
            ViewData["EditValue"] = str;
            return View();
        }

        public IActionResult Index()
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

        public IActionResult UserCars()
        {
            if (Program.User == null)
            {
                return Redirect("~/Home/Enter");
            }
            return View(APIClient.GetRequest<List<CarViewModel>>($"api/car/getusercarlist?userId={Program.User.Id}"));
        }

        [HttpPost]
        public async Task<IActionResult> CarCreate(string Brand, string Model, int Year,
            string VIN, string LicensePlate, string Colour, IFormFile uploadedFile)
        {
            if (LicensePlate.Length < 8 || LicensePlate.Length > 9)
            {
                ModelState.AddModelError("LicensePlate", "Номер должен содержать от 8 до 9 символов.");
                ViewData["EditValue"] = LicensePlate;
                return View();
            }
            if (Year != 0 || uploadedFile != null)
            {
                DateTime date = new(Year, 1, 1);
                NumberCreate numberCreate = new();
                numberCreate.CreateLicensePlateImage(LicensePlate);
                // путь к папке Files
                string path = "/Files/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_environment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                CarBindingModel car = new()
                {
                    Brand = Brand,
                    Model = Model,
                    Year = date,
                    VIN = VIN,
                    LicensePlate = LicensePlate,
                    Colour = Colour,
                    FileName = uploadedFile.FileName,
                    Path = path,
                    UserId = (int)Program.User.Id,
                    UserName = Program.User.FirstName,
                    UserPhone = Program.User.PhoneNumber,
                    UserEmail = Program.User.Email

                };

                APIClient.PostRequest("api/car/createorupdatecar", car);
            }
            return Redirect("Cars");
        }

        [HttpGet]
        public IActionResult CarUpdate(int carId)
        {
            ViewBag.Car = APIClient.GetRequest<CarViewModel>($"api/car/getcar?carId={carId}");
            return View();
        }

        [HttpPost]
        public IActionResult SearchGet(string LicensePlate, int k)
        {
            if (LicensePlate.Length < 8 || LicensePlate.Length > 9)
            {
                ModelState.AddModelError("LicensePlate", "Номер должен содержать от 8 до 9 символов.");
                ViewData["EditValue"] = LicensePlate;
                return RedirectToAction("CarNotFound");
            }

            var car = APIClient.GetRequest<CarViewModel>($"api/car/getcarbyplate?LicensePlate={LicensePlate}");
            if (car == null)
            {
                return RedirectToAction("CarNotFound");
            }

            ViewBag.Car = car;

            if (Program.User != null)
            {
                return RedirectToAction("CarViewReview", new { LicensePlate = LicensePlate });
            }
            else
            {
                return RedirectToAction("CarView", new { LicensePlate = LicensePlate });
            }
        }

        public IActionResult CarNotFound()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CarUpdate(int carId, string Brand, string Model, int Year,
            string VIN, string LicensePlate, string Colour, IFormFile uploadedFile)
        {
            var car = APIClient.GetRequest<CarViewModel>($"api/car/getcar?carId={carId}");
            if (car == null)
            {
                return Redirect("Cars");
            }
            if (carId != 0)
            {
                NumberCreate numberCreate = new();
                numberCreate.CreateLicensePlateImage(LicensePlate);
                DateTime date = new(Year, 1, 1);
                string filename = car.FileName;
                string path = car.Path;
                if (uploadedFile != null)
                {
                    // путь к папке Files
                    path = "/Files/" + uploadedFile.FileName;
                    // сохраняем файл в папку Files в каталоге wwwroot
                    using (var fileStream = new FileStream(_environment.WebRootPath + path, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }
                    filename = uploadedFile.FileName;
                }

                CarBindingModel newcar = new()
                {
                    Id = carId,
                    Brand = Brand,
                    Model = Model,
                    Year = date,
                    VIN = VIN,
                    LicensePlate = LicensePlate,
                    Colour = Colour,
                    FileName = filename,
                    Path = path,
                    UserId = (int)Program.User.Id,
                    UserName = Program.User.FirstName,
                    UserPhone = Program.User.PhoneNumber,
                    UserEmail = Program.User.Email
                };

                APIClient.PostRequest("api/car/createorupdatecar", newcar);
            }
            return Redirect("Cars");
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