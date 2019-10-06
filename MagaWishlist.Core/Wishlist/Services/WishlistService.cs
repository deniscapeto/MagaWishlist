using MagaWishlist.Core.Wishlist.Interfaces;
using MagaWishlist.Core.Wishlist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagaWishlist.Core.Wishlist.Services
{
    public class WishlistService : IWishlistService
    {
        readonly IProductRest _productRest;
        readonly ICustomerService _customerService;
        readonly IWishlistRepository _wishlistRepository;

        public WishlistService(IProductRest productRest, ICustomerService customerService, IWishlistRepository wishlistRepository)
        {
            _productRest = productRest;
            _customerService = customerService;
            _wishlistRepository = wishlistRepository;
        }

        public async Task<List<WishListProduct>> GetCustomerWishlistAsync(int customerId)
        {
            if (!await CustomerExists(customerId))
                return default;

            return await _wishlistRepository.GetCustomerWishlistAsync(customerId);
        }

        private async Task<bool> CustomerExists(int customerId)
        {
            var customer = await _customerService.GetCustomerAsync(customerId);

            return customer != null;
        }

        public async Task<WishListProduct> AddProductToCustomerrWishlistAsync(int customerId, string productId)
        {
            if (!await CustomerExists(customerId))
                return default;

            var customerWishlist = await GetCustomerWishlistAsync(customerId);

            var existingProduct = customerWishlist.FirstOrDefault(p => p.ProductId == productId);

            if(existingProduct != null)
                return existingProduct;

            var product = await _productRest.GetProductByIdAsync(productId);

            return await _wishlistRepository.InsertWishlistProductAsync(customerId, product);
        }

        public async Task<bool> RemoveProductFromCustomerrWishlistAsync(int customerId, string productId)
        {
            if (!await CustomerExists(customerId))
                return default;

            return await _wishlistRepository.DeleteWishlistProductAsync(productId: productId, customerId: customerId);
        }
    }
}
