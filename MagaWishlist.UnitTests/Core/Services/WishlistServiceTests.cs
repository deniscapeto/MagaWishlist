using NSubstitute;
using System.Threading.Tasks;
using Xunit;
using MagaWishlist.Core.Wishlist.Interfaces;
using MagaWishlist.Core.Wishlist.Models;
using MagaWishlist.Core.Wishlist.Services;
using System.Collections.Generic;

namespace MagaWishlist.UnitTests.Core.Services
{
    public class WishlistServiceTests
    {
        readonly IProductRest _productRest;
        readonly ICustomerService _customerService;
        readonly IWishlistRepository _wishlistRepository;
        private Customer existingCustomer = new Customer() { Id = 1, Name = "name", Email = "email-fake" };
        private Customer nullReturn = null;
        private int customerId = 1;

        public WishlistServiceTests()
        {
            _productRest = Substitute.For<IProductRest>();
            _customerService = Substitute.For<ICustomerService>();
            _wishlistRepository = Substitute.For<IWishlistRepository>();
        }

        [Fact]
        public async Task GetCustomerWishlistAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
        {
            //Arrange
            _customerService.GetCustomerAsync(customerId).Returns(nullReturn);

            //Act
            var sut = new WishlistService(_productRest, _customerService, _wishlistRepository);
            var products = await sut.GetCustomerWishlistAsync(customerId);

            //Assert
            Assert.Null(products);
        }

        [Fact]
        public async Task GetCustomerWishlistAsync_ShouldReturnList_WhenCustomerDoesExist()
        {
            //Arrange
            _customerService.GetCustomerAsync(customerId).Returns(existingCustomer);

            List<WishListProduct> list = new List<WishListProduct>();
            list.Add(new WishListProduct());

            _wishlistRepository.GetCustomerWishlistAsync(customerId).Returns(list);

            //Act
            var sut = new WishlistService(_productRest, _customerService, _wishlistRepository);
            var products = await sut.GetCustomerWishlistAsync(customerId);

            //Assert
            Assert.NotNull(products);
            Assert.Single(products);
        }

        [Fact]
        public async Task AddProductToCustomerrWishlistAsync_ShouldCallRepository_WhenCustomerDoesExist()
        {
            //Arrange
            string newProductId = "3";
            string existingProductId = "4";
            _customerService.GetCustomerAsync(customerId).Returns(existingCustomer);
            _wishlistRepository.InsertWishlistProductAsync(customerId, Arg.Any<WishListProduct>()).Returns(new WishListProduct());

            var wishlistProduct = new WishListProduct()
            {
                ProductId = newProductId,
                Image = "http://images.luizalabs.com/123.png",
                Price = "30.00",
                Title = "Product123"
            };
            _productRest.GetProductByIdAsync(newProductId).Returns(wishlistProduct);
            var existingList = new List<WishListProduct>() { new WishListProduct() { ProductId = existingProductId } };
            _wishlistRepository.GetCustomerWishlistAsync(customerId).Returns(existingList);

            //Act
            var sut = new WishlistService(_productRest, _customerService, _wishlistRepository);
            var product = await sut.AddProductToCustomerrWishlistAsync(customerId, newProductId);

            //Assert
            Assert.NotNull(product);
            _ = _wishlistRepository.Received(1).InsertWishlistProductAsync(
                customerId,
                Arg.Is<WishListProduct>(
                    x =>
                    x.ProductId == wishlistProduct.ProductId &&
                    x.Image == wishlistProduct.Image &&
                    x.Price == wishlistProduct.Price &&
                    x.Title == wishlistProduct.Title
                    ));
        }

        [Fact]
        public async Task AddProductToCustomerrWishlistAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
        {
            //Arrange
            _customerService.GetCustomerAsync(customerId).Returns(nullReturn);

            //Act
            var sut = new WishlistService(_productRest, _customerService, _wishlistRepository);
            var product = await sut.AddProductToCustomerrWishlistAsync(customerId, "1");

            //Assert
            Assert.Null(product);
        }

        [Fact]
        public async Task AddProductToCustomerrWishlistAsync_ShouldReturnExistingProduct_WhenProductIsAlreadyInWishlist()
        {
            //Arrange
            string productId = "1bf0f365-fbdd-4e21-9786-da459d78dd1f";
            _customerService.GetCustomerAsync(customerId).Returns(existingCustomer);
            var existingList = new List<WishListProduct>() { new WishListProduct() { ProductId = productId } };
            _wishlistRepository.GetCustomerWishlistAsync(customerId).Returns(existingList);

            //Act
            var sut = new WishlistService(_productRest, _customerService, _wishlistRepository);
            var product = await sut.AddProductToCustomerrWishlistAsync(customerId, productId);

            //Assert
            Assert.NotNull(product);
            await _wishlistRepository.DidNotReceive().InsertWishlistProductAsync(Arg.Any<int>(), Arg.Any<WishListProduct>());
            Assert.Equal(productId, product.ProductId);
        }

        [Fact]
        public async Task RemoveProductFromCustomerrWishlistAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
        {
            //Arrange
            _customerService.GetCustomerAsync(customerId).Returns(nullReturn);

            //Act
            var sut = new WishlistService(_productRest, _customerService, _wishlistRepository);
            var result = await sut.RemoveProductFromCustomerrWishlistAsync(customerId, "1");

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RemoveProductFromCustomerrWishlistAsync_ShouldCallRepository_WhenCustomerDoesExist()
        {
            //Arrange
            string productId = "3";
            _customerService.GetCustomerAsync(customerId).Returns(existingCustomer);
            _wishlistRepository.DeleteWishlistProductAsync(customerId, productId).Returns(true);

            var wishlistProduct = new WishListProduct()
            {
                Id = 1,
                ProductId = productId,
                Image = "http://images.luizalabs.com/123.png",
                Price = "30.00",
                Title = "Product123"
            };
            _productRest.GetProductByIdAsync(productId).Returns(wishlistProduct);

            //Act
            var sut = new WishlistService(_productRest, _customerService, _wishlistRepository);
            var result = await sut.RemoveProductFromCustomerrWishlistAsync(customerId, productId);

            //Assert
            Assert.True(result);
            _ = _wishlistRepository
                .Received(1)
                .DeleteWishlistProductAsync(customerId, productId);
        }
    }
}
