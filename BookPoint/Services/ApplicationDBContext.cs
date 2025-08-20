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
        public DbSet<CategoryModel> Categories { get; set; } = null;
        public DbSet<AgentModel> Agents { get; set; } = null!;
    }
}
