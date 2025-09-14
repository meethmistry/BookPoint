namespace BookPoint.Models
{
    public class CartItemModel
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;

        // Navigation
        public virtual CartModel Cart { get; set; }
        public virtual BookModel Book { get; set; }
    }
}
