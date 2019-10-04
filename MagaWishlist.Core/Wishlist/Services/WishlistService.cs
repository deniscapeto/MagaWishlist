using MagaWishlist.Core.Wishlist.Interfaces;
using MagaWishlist.Core.Wishlist.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MagaWishlist.Core.Wishlist.Services
{
    public class WishlistService : IWishlistService
    {
        IProductRest _productRest;
        ICustomerService _customerService;
        IWishlistRepository _wishlistRepository;

        public WishlistService(IProductRest productRest, ICustomerService customerService, IWishlistRepository wishlistRepository)
        {
            _productRest = productRest;
            _customerService = customerService;
            _wishlistRepository = wishlistRepository;
        }

        public async Task<List<WishListProduct>> GetCustomerWishlistAsync(int customerId)
        {
            var customer = await _customerService.GetCustomerAsync(customerId);

            if (customer == null)
                return default;

            return await _wishlistRepository.GetCustomerWishlistAsync(customerId);
        }

        public async Task<WishListProduct> AddProductToCustomerrWishlistAsync(int customerId, string productId)
        {
            var customer = await _customerService.GetCustomerAsync(customerId);

            if (customer == null)
                return default;

            var product = await _productRest.GetProductByIdAsync(productId);

            return await _wishlistRepository.InsertWishlistProductAsync(customerId, product);
        }

        public async Task<bool> RemoveProductFromCustomerrWishlistAsync(int customerId, string productId)
        {
            var customer = await _customerService.GetCustomerAsync(customerId);

            if (customer == null)
                return default;

            return await _wishlistRepository.DeleteWishlistProductAsync(productId: productId, customerId: customerId);
        }
    }
}
