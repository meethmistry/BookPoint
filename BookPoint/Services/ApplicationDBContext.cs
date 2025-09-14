using BookPoint.Models;
using Microsoft.EntityFrameworkCore;

namespace BookPoint.Services
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; } = null!;
        public DbSet<CustomerModel> Customers { get; set; } = null!;
        public DbSet<CategoryModel> Categories { get; set; } = null!;
        public DbSet<AgentModel> Agents { get; set; } = null!;
        public DbSet<BookModel> Books { get; set; } = null!;

        public DbSet<CartItemModel> CartItems { get; set; }
        public DbSet<CartModel> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔹 Set primary keys
            modelBuilder.Entity<CartModel>()
                .HasKey(c => c.CartId);

            modelBuilder.Entity<CartItemModel>()
                .HasKey(ci => ci.CartItemId);

            // 🔹 Relationships: Cart → User (One user has many carts)
            modelBuilder.Entity<CartModel>()
                .HasOne(c => c.User)
                .WithMany() // if UserModel doesn’t have ICollection<CartModel>
                .HasForeignKey(c => c.UserId);

            // 🔹 Relationships: CartItem → Cart (One cart has many items)
            modelBuilder.Entity<CartItemModel>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId);

            // 🔹 Relationships: CartItem → Book (One book can appear in many cart items)
            modelBuilder.Entity<CartItemModel>()
                .HasOne(ci => ci.Book)
                .WithMany() // if BookModel doesn’t have ICollection<CartItemModel>
                .HasForeignKey(ci => ci.BookId);
        }
    }
}
