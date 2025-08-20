using Microsoft.AspNetCore.Mvc;

namespace BookPoint.Controllers
{
    public class BooksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BooksForm()
        {
            return PartialView();
        }
    }
}
