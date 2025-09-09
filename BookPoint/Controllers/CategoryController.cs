using BookPoint.Models;
using BookPoint.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookPoint.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDBContext _dbContext;

        public CategoryController(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public IActionResult Categories()
        {
            var categories = _dbContext.Categories.ToList();
            return PartialView(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEditCategory(CategoryModel category)
        {
            if (_dbContext.Categories.Any(c => c.Name.ToLower() == category.Name.ToLower() && c.Id != category.Id))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Category already exists. Please use another name." });
                }

                TempData["CategoryError"] = "Category already exists. Please use another name.";
                TempData["CurrentTab"] = "Categories";
                return RedirectToAction("Dashboard", "Dashboard");
            }

            if (category.Id == 0)
                _dbContext.Categories.Add(category);
            else
                _dbContext.Categories.Update(category);

            _dbContext.SaveChanges();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            TempData["CurrentTab"] = "Categories";
            return RedirectToAction("Dashboard", "Dashboard");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int id)
        {
            var entity = _dbContext.Categories.Find(id);
            if (entity == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Category not found." });
                }
                return NotFound();
            }

            _dbContext.Categories.Remove(entity);
            _dbContext.SaveChanges();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            TempData["CurrentTab"] = "Categories";
            return RedirectToAction("Dashboard", "Dashboard");
        }

    }
}
