using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MagaWishlist.Core.Wishlist.Interfaces;
using MagaWishlist.Core.Wishlist.Models;

namespace MagaWishlist.Data
{
    public class WishlistRepository : IWishlistRepository
    {
        readonly IDbConnection _connection;
        public WishlistRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<WishListProduct>> GetCustomerWishlistAsync(int customerId)
        {
            var enumerable = await _connection.QueryAsync<WishListProduct>(
                "SELECT Id, ProductId, Price, Title, Image " +
                "FROM WishlistProducts " +
                "WHERE customerId = @customerId", new { customerId });

            return enumerable.ToList();
        }

        public Task<WishListProduct> GetWishlistProductAsync(int customerId, string productId)
        {
            return _connection.QueryFirstOrDefaultAsync<WishListProduct>(
                "SELECT Id, ProductId, Price, Title, Image " +
                "FROM WishlistProducts " +
                "WHERE productId = @productId and customerId = @customerId", 
                new { productId, customerId });
        }

        public async Task<WishListProduct> InsertWishlistProductAsync(int customerId, WishListProduct wishListProduct)
        {
            var rowsAffected = await _connection.ExecuteAsync(
                "INSERT INTO WishlistProducts (ProductId, CustomerId, Price, Title, Image) " +
                "VALUES (@ProductId, @CustomerId, @Price, @Title, @Image)" 
                , new
                {
                    wishListProduct.ProductId,
                    CustomerId= customerId,
                    wishListProduct.Price,
                    wishListProduct.Title,
                    wishListProduct.Image
                });

            if (rowsAffected == 0)
                return default;

            return await GetWishlistProductAsync(customerId: customerId, productId: wishListProduct.ProductId);
        }

        public async Task<bool> DeleteWishlistProductAsync(int customerId, string productId)
        {
            var rowsAffected = await _connection.ExecuteAsync(
                "DELETE " +
                "FROM WishlistProducts " +
                "WHERE productId = @productId and customerId = @customerId",
                new { productId, customerId });

            return rowsAffected == 1;
        }
    }
}
