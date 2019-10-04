using System.Collections.Generic;
using System.Threading.Tasks;
using MagaWishlist.Core.Wishlist.Models;

namespace MagaWishlist.Core.Wishlist.Interfaces
{
    public interface IWishlistService
    {
        Task<List<WishListProduct>> GetCustomerWishlistAsync(int customerId);

        Task<WishListProduct> AddProductToCustomerrWishlistAsync(int customerId, string productId);

        Task<bool> RemoveProductFromCustomerrWishlistAsync(int customerId, string productId);
    }
}