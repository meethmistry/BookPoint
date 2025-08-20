using Microsoft.AspNetCore.Mvc;

namespace BookPoint.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
