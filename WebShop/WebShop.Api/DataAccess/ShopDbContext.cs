using Microsoft.EntityFrameworkCore;
using WebShop.Api.DataAccess.Configurations;
using WebShop.Api.Domain;

namespace WebShop.Api.DataAccess
{
    public class ShopDbContext : DbContext
    {
        public ShopDbContext(DbContextOptions options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}