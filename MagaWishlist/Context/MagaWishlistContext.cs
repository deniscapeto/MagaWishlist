using MagaWishlist.Core.Authentication.Models;
using MagaWishlist.Core.Wishlist.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MySql.Data.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace MagaWishlist.Context
{
    public class MagaWishlistContext : DbContext
    {
        public MagaWishlistContext(DbContextOptions<MagaWishlistContext> options)
          : base(options)
        { }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<WishListProduct> WishListProducts { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User() { AccessKey = "teste", UserID = "teste" });
        }
    }
}
