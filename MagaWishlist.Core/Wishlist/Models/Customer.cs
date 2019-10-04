using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MagaWishlist.Core.Wishlist.Models
{
    public class Customer
    {
        public Customer()
        {
            this.Wishlist = new List<Product>();
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public List<Product> Wishlist { get; }

        public void AddToWishlist(int productId)
        {
            this.Wishlist.Add(new Product(productId));
        }
    }
}
