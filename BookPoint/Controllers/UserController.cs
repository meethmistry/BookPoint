using System.Text.RegularExpressions;
using BookPoint.Models;
using BookPoint.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            return View(new Tuple<LoginViewModel, RegisterViewModel>(new LoginViewModel(), new RegisterViewModel()));
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            // Print values in console
            Console.WriteLine("Login Attempt:");
            Console.WriteLine($"Email: {model.Email}");
            Console.WriteLine($"Password: {model.Password}");

            if (ModelState.IsValid)
            {
                var user = _dbContext.Users
                .FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password && u.IsActive);

                if (user != null)
                {
                    if (user.Role == "Customer")
                    {
                        return RedirectToAction("Index", "Customer");
                    }
                    else if(user.Role == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Dashboard");
                    }
                }
                else
                {
                    ViewBag.LoginError = "Invalid email or password.";
                }
            }

            return View("LoginRegister", new Tuple<LoginViewModel, RegisterViewModel>(model, new RegisterViewModel()));
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            Console.WriteLine("Registration Attempt:");
            Console.WriteLine($"UserName: {model.UserName}");
            Console.WriteLine($"Email: {model.Email}");
            Console.WriteLine($"Password: {model.Password}");
            Console.WriteLine($"ConfirmPassword: {model.ConfirmPassword}");


            // ===== SAVE USER =====
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

            // ===== SAVE CUSTOMER =====
            var customer = new CustomerModel
            {
                UID = user.Id,
                UserName = model.UserName,
                Phone = "",
                Address = "",
                User = user,
            };

            _dbContext.Customers.Add(customer);
            _dbContext.SaveChanges();

            TempData["Success"] = "Registration successful!";
            return RedirectToAction("Index", "Customer");
        }


    }
}
