using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookPoint.Models
{
    public class BookModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Book Name is required")]
        [StringLength(200, ErrorMessage = "Book Name cannot exceed 200 characters")]
        public string BookName { get; set; }

        [StringLength(150, ErrorMessage = "Author Name cannot exceed 150 characters")]
        public string? AuthorName { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual CategoryModel Category { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Purchase Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Purchase Price must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public double PurchasePrice { get; set; }

        [Required(ErrorMessage = "Sales Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sales Price must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public double SalesPrice { get; set; }
        public double ProfitPerBook { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Cover Image is required")]
        public IFormFile CoverImageFile { get; set; }

        public string CoverImagePath { get; set; }

        [NotMapped]
        public IFormFile? SamplePdfFile { get; set; } 
        public string? SamplePdfPath { get; set; }    


        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
