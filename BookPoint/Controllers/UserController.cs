using System;
using System.Linq;
using BookPoint.Models;
using BookPoint.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookPoint.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDBContext _dbContext;

        public UserController(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult LoginRegister()
        {
            var vm = new AuthViewModel
            {
                Login = new LoginViewModel(),
                Register = new RegisterViewModel()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(AuthViewModel vm)
        {
            var model = vm.Register;

            // Check if username already exists
            if (_dbContext.Users.Any(c => c.Email.ToLower() == model.Email.ToLower()))
            {
                var errorMessage = "Email already exists. Please choose a different email.";

                return Json(new { success = false, message = errorMessage });

            }

            try
            {
                // Create user
                var user = new UserModel
                {
                    Email = model.Email,
                    Password = model.Password,
                    Role = "Customer",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();

                // Create customer
                var customer = new CustomerModel
                {
                    UID = user.Id,
                    UserName = model.UserName,
                    Phone = "",
                    Address = "",
                    User = user
                };

                _dbContext.Customers.Add(customer);
                _dbContext.SaveChanges();
                HttpContext.Session.SetInt32("UserId", user.Id);
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Customer") });


            }
            catch (Exception)
            {
                var errorMessage = "Something went wrong during registration. Please try again.";

                return Json(new { success = false, message = errorMessage });

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(AuthViewModel vm)
        {
            var model = vm.Login;

            try
            {
                var user = _dbContext.Users
                    .FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password && u.IsActive);

                if (user != null)
                {
                    string redirectUrl = "";
                    if (user.Role == "Customer")
                        redirectUrl = Url.Action("Index", "Customer");
                    else if (user.Role == "Admin")
                        redirectUrl = Url.Action("Dashboard", "Dashboard");
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    return Json(new { success = true, redirectUrl = redirectUrl });


                }

                var errorMessage = "Invalid email or password.";
                return Json(new { success = false, message = errorMessage });

            }
            catch (Exception)
            {
                var errorMessage = "Something went wrong during login. Please try again.";

                return Json(new { success = false, message = errorMessage });

            }
        }
    }
}