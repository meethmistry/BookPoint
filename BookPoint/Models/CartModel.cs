namespace BookPoint.Models
{
    public class CartModel
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public virtual UserModel User { get; set; }
        public virtual ICollection<CartItemModel> Items { get; set; }
    }
}
