using BookPoint.Models;
using BookPoint.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookPoint.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDBContext _context;

        public CustomerController(ApplicationDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var books = _context.Books
         .Select(b => new BookModel
         {
             Id = b.Id,
             BookName = b.BookName,
             AuthorName = b.AuthorName,
             CategoryId = b.CategoryId,
             Category = b.Category,
             CoverImagePath = b.CoverImagePath,
             Description = b.Description,
             PurchasePrice = b.PurchasePrice,
             SalesPrice = b.SalesPrice,
             ProfitPerBook = b.ProfitPerBook,
             Quantity = b.Quantity,
             CreatedAt = b.CreatedAt,
             LastUpdatedAt = b.LastUpdatedAt,
             SamplePdfPath = b.SamplePdfPath
         })
         .ToList();

            var categories = _context.Categories.ToList();

            var viewModel = new BooksCategoriesClass
            {
                Books = books,
                Categories = categories
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GetCustomerProfile(int uid)
        {
            try
            {
                var customer = _context.Customers
                    .Include(c => c.User)
                    .Where(c => c.UID == uid)
                    .Select(c => new
                    {
                        UserName = c.UserName,
                        Email = c.User.Email,
                        Phone = c.Phone,
                        Address = c.Address
                    })
                    .FirstOrDefault();

                if (customer == null)
                {
                    return Json(new { success = false, message = "Customer not found." });
                }

                return Json(new { success = true, data = customer });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }


        [HttpPost]
        public IActionResult UpdateCustomerProfile([FromBody] CustomerModel model)
        {
            try
            {
                // 🔹 Validate UserName
                if (string.IsNullOrWhiteSpace(model.UserName))
                {
                    return Json(new { success = false, message = "Username is required." });
                }

                // 🔹 Validate Phone (optional but must be 10 digits if provided)
                if (!string.IsNullOrEmpty(model.Phone))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(model.Phone, @"^\d{10}$"))
                    {
                        return Json(new { success = false, message = "Phone number must be 10 digits." });
                    }
                }

                var customer = _context.Customers.FirstOrDefault(c => c.UID == model.UID);
                if (customer == null)
                {
                    return Json(new { success = false, message = "Customer not found." });
                }

                // ✅ Update fields
                customer.UserName = model.UserName;
                customer.Phone = model.Phone;
                customer.Address = model.Address;

                _context.Customers.Update(customer);
                _context.SaveChanges();

                return Json(new { success = true, message = "Profile updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }



        [HttpPost]
        public IActionResult AddOrUpdateCart([FromBody] CartRequest request)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                int bookId = request.BookId;
                int quantity = request.Quantity;

                if (userId <= 0)
                {
                    return Json(new { success = false, message = "User not logged in." });
                }

                // ✅ Ensure a Cart exists for this user
                var userCart = _context.Carts.FirstOrDefault(c => c.UserId == userId);
                if (userCart == null)
                {
                    userCart = new CartModel
                    {
                        UserId = userId,
                        CreatedAt = DateTime.Now
                    };

                    _context.Carts.Add(userCart);
                    _context.SaveChanges();
                }

                // ✅ Check if the item already exists in cart
                var existingItem = _context.CartItems
                    .FirstOrDefault(ci => ci.CartId == userCart.CartId && ci.BookId == bookId);

                if (existingItem != null)
                {
                    // Update quantity
                    existingItem.Quantity += quantity;
                    _context.CartItems.Update(existingItem);
                }
                else
                {
                    // Add new cart item
                    var newItem = new CartItemModel
                    {
                        CartId = userCart.CartId,
                        BookId = bookId,
                        Quantity = quantity,
                        AddedAt = DateTime.Now
                    };
                    _context.CartItems.Add(newItem);
                }

                _context.SaveChanges();

                return Json(new { success = true, message = "Book added to cart successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        public class CartRequest
        {
            public int BookId { get; set; }
            public int Quantity { get; set; }
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
