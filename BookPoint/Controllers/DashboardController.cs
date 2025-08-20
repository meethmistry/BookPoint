using BookPoint.Migrations;
using BookPoint.Models;
using BookPoint.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookPoint.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDBContext _dbContext;

        public DashboardController(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Overview()
        {
            return PartialView();
        }

        // Categories
        public IActionResult Categories()
        {
            var categories = _dbContext.Categories.ToList();
            return PartialView(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEditCategory(CategoryModel category)
        {
            if (category.Id == 0)
                _dbContext.Categories.Add(category);
            else
                _dbContext.Categories.Update(category);

            _dbContext.SaveChanges();

            TempData["CurrentTab"] = "Categories";

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int id)
        {
            var entity = _dbContext.Categories.Find(id);
            if (entity == null) return NotFound();

            _dbContext.Categories.Remove(entity);
            _dbContext.SaveChanges();

            TempData["CurrentTab"] = "Categories";
            return RedirectToAction(nameof(Dashboard));
        }



        // Books
        public IActionResult Books()
        {
            return PartialView();
        }

        // Delivery Agents
        public IActionResult DeliveryAgents()
        {
            return PartialView();
        }

        // Orders
        public IActionResult Orders()
        {
            return PartialView();
        }

        // Customers
        public IActionResult Customers()
        {
            return PartialView();
        }

        // Walk In Sales
        public IActionResult WalkInSales()
        {
            return PartialView();
        }

        // Payments
        public IActionResult Payments()
        {
            return PartialView();
        }

        // Sales
        public IActionResult Sales()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginRegister", "User");
        }


    }
}
