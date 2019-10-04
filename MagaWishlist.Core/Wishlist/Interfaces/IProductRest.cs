using System.Threading.Tasks;
using MagaWishlist.Core.Wishlist.Models;

namespace MagaWishlist.Core.Wishlist.Interfaces
{
    public interface IProductRest
    {
        Task<Product> GetProductByIdAsync(int id);
    }
}