using BookPoint.Migrations;
using BookPoint.Models;
using BookPoint.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    //        // Folder path
    //        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "books");

    //        // List of files to keep
    //        var filesToKeep = new HashSet<string>
    //{
    //    "ea050166-5648-42fd-8b17-86a818e3a175.jpg",
    //    "c3724553-c6a8-41da-897e-fb23b0edf693.jpg",
    //    "ad136c08-f931-4534-b52a-807c3d33306d.jpeg",
    //    "824a9417-c222-40c0-b0f9-7a35427abfb7.jpg",
    //    "f9f8953a-8f23-4698-8bd8-da8458c8f571.jpeg"
    //};

    //        if (Directory.Exists(folderPath))
    //        {
    //            var allFiles = Directory.GetFiles(folderPath);

    //            foreach (var filePath in allFiles)
    //            {
    //                string fileName = Path.GetFileName(filePath);

    //                if (!filesToKeep.Contains(fileName))
    //                {
    //                    System.IO.File.Delete(filePath); // Delete the file
    //                    Console.WriteLine($"Deleted: {fileName}");
    //                }
    //            }
    //        }
            TempData.Remove("currentView");
            return View();
        }

        public IActionResult Overview()
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
