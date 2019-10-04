namespace MagaWishlist.Core.Wishlist.Models
{
    public class Product
    {
        public Product(int id)
        {
            this.Id = id;
        }

        public int Id { get; set; }
        public string Price { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
    }
}
