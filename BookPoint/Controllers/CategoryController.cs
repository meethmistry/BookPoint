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

                return Json(new { success = false, message = "Category already exists. Please use another name." });


            }

            if (category.Id == 0)
                _dbContext.Categories.Add(category);
            else
                _dbContext.Categories.Update(category);

            _dbContext.SaveChanges();

            return Json(new { success = true });


        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int id)
        {
            var entity = _dbContext.Categories.Find(id);
            if (entity == null)
            {

                return Json(new { success = false, message = "Category not found." });

            }

            _dbContext.Categories.Remove(entity);
            _dbContext.SaveChanges();


            return Json(new { success = true });


        }

    }
}
