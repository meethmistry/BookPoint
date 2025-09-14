using BookPoint.Models;
using BookPoint.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookPoint.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDBContext _dbContext;

        public CartController(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("LoginRegister", "User");
            }

            var cart = _dbContext.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Book)
                .ThenInclude(b => b.Category)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new CartModel
                {
                    UserId = userId.Value,
                    Items = new List<CartItemModel>()
                };
            }


            return View(cart);
        }

        [HttpPost]
        public IActionResult UpdateCart([FromBody] CartRequest request)
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

                // ✅ Find the user's cart
                var userCart = _dbContext.Carts.FirstOrDefault(c => c.UserId == userId);
                if (userCart == null)
                {
                    return Json(new { success = false, message = "Cart not found for this user." });
                }

                // ✅ Find the existing item in cart
                var existingItem = _dbContext.CartItems
                    .FirstOrDefault(ci => ci.CartId == userCart.CartId && ci.BookId == bookId);

                if (existingItem == null)
                {
                    return Json(new { success = false, message = "Item not found in cart." });
                }

                // ✅ Update quantity (replace instead of increment)
                existingItem.Quantity = quantity;
                _dbContext.CartItems.Update(existingItem);
                _dbContext.SaveChanges();

                return Json(new { success = true, message = "Cart updated successfully." });
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
        public IActionResult RemoveItem([FromBody] RemoveCartRequest request)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                if (userId <= 0)
                {
                    return Json(new { success = false, message = "User not logged in." });
                }

                // Find user's cart
                var userCart = _dbContext.Carts
                    .Include(c => c.Items)
                    .FirstOrDefault(c => c.UserId == userId);

                if (userCart == null)
                {
                    return Json(new { success = false, message = "Cart not found for this user." });
                }

                // Find the cart item to remove
                var cartItem = userCart.Items.FirstOrDefault(ci => ci.CartItemId == request.CartItemId);
                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Item not found in cart." });
                }

                // Remove item
                _dbContext.CartItems.Remove(cartItem);
                _dbContext.SaveChanges();

                return Json(new { success = true, message = "Item removed from cart successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // DTO for JS request
        public class RemoveCartRequest
        {
            public int CartItemId { get; set; }
        }


    }
}
