using Newtonsoft.Json;

namespace MagaWishlist.Core.Wishlist.Models
{
    public class WishListProduct
    {
        public WishListProduct()
        {
        }

        public WishListProduct(string productId)
        {
            this.ProductId = productId;
        }

        [JsonIgnore]
        public int Id { get; set; }
        public string ProductId { get; set; }
        public string Price { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
    }
}
