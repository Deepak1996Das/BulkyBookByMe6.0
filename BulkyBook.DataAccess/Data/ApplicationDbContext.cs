  using BulkyBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess
{
    public class ApplicationDbContext:IdentityDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }

        public DbSet<Category> categories { get; set; }

        public DbSet<CoverType> coverTypes { get; set; }

        public DbSet<Product> products { get; set; }

        public DbSet<ApplicationUser> applicationUsers { get; set; }
        
        public DbSet<Company> companies { get; set; }
        public DbSet<ShoppingCart> shoppingCarts { get; set; }

        public DbSet<OrderHeader> orderHeaders { get; set; }

        public DbSet<OrderDetail> orderDetails { get; set; }


    }
}
