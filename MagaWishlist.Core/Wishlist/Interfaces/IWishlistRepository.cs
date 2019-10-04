using MagaWishlist.Core.Wishlist.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MagaWishlist.Core.Wishlist.Interfaces
{
    public interface IWishlistRepository
    {
        Task<List<WishListProduct>> GetCustomerWishlistAsync(int customerId);

        Task<WishListProduct> InsertWishlistProductAsync(int customerId, WishListProduct wishListProduct);

        Task<WishListProduct> GetWishlistProductAsync(int customerId, string productId);

        Task<bool> DeleteWishlistProductAsync(int customerId, string productId);
    }
}
