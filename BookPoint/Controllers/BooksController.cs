using BookPoint.Models;
using BookPoint.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookPoint.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDBContext _dbContext;

        public BooksController(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public IActionResult Index()
        {
            var categories = _dbContext.Categories.OrderBy(c => c.Name).ToList();
            ViewBag.Categories = categories;
            var books = _dbContext.Books.OrderBy(b => b.BookName).ToList();
            return PartialView(books);
        }

        public IActionResult BooksForm()
        {
            var categories = _dbContext.Categories.OrderBy(c => c.Name).ToList();
            ViewBag.Categories = categories;
            return PartialView("BooksForm", new BookModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEditBook(BookModel model)
        {
            TempData.Remove("BookError");
            TempData["CurrentTab"] = "Books";

            model.Category = _dbContext.Categories.First(c => c.Id == model.CategoryId);

            try
            {
                if (model.Id == 0)
                {
                    // ==== ADD NEW BOOK ====
                    // --- Cover Image ---
                    if (model.CoverImageFile != null && model.CoverImageFile.Length > 0)
                    {
                        var coverFileName = $"{Guid.NewGuid()}{Path.GetExtension(model.CoverImageFile.FileName)}";
                        var coverUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "books");

                        if (!Directory.Exists(coverUploadPath))
                            Directory.CreateDirectory(coverUploadPath);

                        var coverFilePath = Path.Combine(coverUploadPath, coverFileName);
                        using (var stream = new FileStream(coverFilePath, FileMode.Create))
                        {
                            model.CoverImageFile.CopyTo(stream);
                        }

                        model.CoverImagePath = $"/uploads/books/{coverFileName}";
                    }

                    // --- Sample PDF ---
                    if (model.SamplePdfFile != null && model.SamplePdfFile.Length > 0)
                    {
                        var pdfFileName = $"{Guid.NewGuid()}{Path.GetExtension(model.SamplePdfFile.FileName)}";
                        var pdfUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "books", "pdfs");

                        if (!Directory.Exists(pdfUploadPath))
                            Directory.CreateDirectory(pdfUploadPath);

                        var pdfFilePath = Path.Combine(pdfUploadPath, pdfFileName);
                        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
                        {
                            model.SamplePdfFile.CopyTo(stream);
                        }

                        model.SamplePdfPath = $"/uploads/books/pdfs/{pdfFileName}";
                    }

                    model.ProfitPerBook = model.SalesPrice - model.PurchasePrice;
                    model.CreatedAt = DateTime.UtcNow;
                    model.LastUpdatedAt = DateTime.UtcNow;

                    _dbContext.Books.Add(model);
                    _dbContext.SaveChanges();

                    TempData["SuccessMessage"] = "Book added successfully!";
                }
                else
                {
                    // ==== EDIT EXISTING BOOK ====
                    var existingBook = _dbContext.Books.FirstOrDefault(b => b.Id == model.Id);
                    if (existingBook == null)
                    {
                        TempData["BookError"] = "Book not found.";
                        TempData["View"] = "Insert";
                        return RedirectToAction("Dashboard", "Dashboard");
                    }

                    // Update fields
                    existingBook.BookName = model.BookName;
                    existingBook.AuthorName = model.AuthorName;
                    existingBook.CategoryId = model.CategoryId;
                    existingBook.Description = model.Description;
                    existingBook.Quantity = model.Quantity;
                    existingBook.PurchasePrice = model.PurchasePrice;
                    existingBook.SalesPrice = model.SalesPrice;
                    existingBook.ProfitPerBook = model.SalesPrice - model.PurchasePrice;
                    existingBook.LastUpdatedAt = DateTime.UtcNow;

                    // Update Cover Image if a new one is uploaded
                    if (model.CoverImageFile != null && model.CoverImageFile.Length > 0)
                    {
                        var coverFileName = $"{Guid.NewGuid()}{Path.GetExtension(model.CoverImageFile.FileName)}";
                        var coverUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "books");

                        if (!Directory.Exists(coverUploadPath))
                            Directory.CreateDirectory(coverUploadPath);

                        var coverFilePath = Path.Combine(coverUploadPath, coverFileName);
                        using (var stream = new FileStream(coverFilePath, FileMode.Create))
                        {
                            model.CoverImageFile.CopyTo(stream);
                        }

                        existingBook.CoverImagePath = $"/uploads/books/{coverFileName}";
                    }

                    // Update Sample PDF if a new one is uploaded
                    if (model.SamplePdfFile != null && model.SamplePdfFile.Length > 0)
                    {
                        var pdfFileName = $"{Guid.NewGuid()}{Path.GetExtension(model.SamplePdfFile.FileName)}";
                        var pdfUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "books", "pdfs");

                        if (!Directory.Exists(pdfUploadPath))
                            Directory.CreateDirectory(pdfUploadPath);

                        var pdfFilePath = Path.Combine(pdfUploadPath, pdfFileName);
                        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
                        {
                            model.SamplePdfFile.CopyTo(stream);
                        }

                        existingBook.SamplePdfPath = $"/uploads/books/pdfs/{pdfFileName}";
                    }

                    _dbContext.Books.Update(existingBook);
                    _dbContext.SaveChanges();

                    TempData["SuccessMessage"] = "Book updated successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["View"] = "Insert";
                TempData["BookError"] = $"Error saving book: {ex.Message}";
            }

            return RedirectToAction("Dashboard", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteBook(int id)
        {
            var entity = _dbContext.Books.Find(id);
            if (entity == null) return NotFound();

            _dbContext.Books.Remove(entity);
            _dbContext.SaveChanges();

            TempData["CurrentTab"] = "Books";
            return RedirectToAction("Dashboard", "Dashboard");
        }
    }
}
