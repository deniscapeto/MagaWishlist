using MagaWishlist.Controllers;
using NSubstitute;
using Xunit;
using System.Threading.Tasks;
using MagaWishlist.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using MagaWishlist.Core.Wishlist.Interfaces;
using MagaWishlist.Core.Wishlist.Models;
using System.Collections.Generic;

namespace MagaWishlist.UnitTests.API.Controllers
{
    public class WishlistControllerTests
    {
        readonly IWishlistService _wishlistService;
        public WishlistControllerTests()
        {
            _wishlistService = Substitute.For<IWishlistService>();
        }

        [Fact]
        public async Task GetWishlistAsync_shouldReturnBadRequest_WhenCustomerIdNotProvided()
        {
            //Arrange
            int id = 0;
            List<WishListProduct> notFound = null;

            var sut = new WishlistController(_wishlistService);

            //Act 
            var result = await sut.GetWishlistAsync(id);

            //Assert            
            Assert.Equal((int)HttpStatusCode.BadRequest, (result.Result as ObjectResult).StatusCode);
        }

        [Fact]
        public async Task GetWishlistAsync_shouldReturnNotFound_WhenItIsNotPossibleToGet()
        {
            //Arrange
            int id = 1;
            List<WishListProduct> notFound = null;
            _wishlistService.GetCustomerWishlistAsync(id).Returns(notFound);

            var sut = new WishlistController(_wishlistService);

            //Act 
            var result = await sut.GetWishlistAsync(id);

            //Assert            
            Assert.Equal((int)HttpStatusCode.NotFound, (result.Result as StatusCodeResult).StatusCode);
        }

        [Fact]
        public async Task GetWishlistAsync_shouldReturnOk_WhenItIsPossibleToGet()
        {
            //Arrange
            int id = 1;
            List<WishListProduct> existingList = new List<WishListProduct>() { new WishListProduct()};
            _wishlistService.GetCustomerWishlistAsync(id).Returns(existingList);

            var sut = new WishlistController(_wishlistService);

            //Act 
            var result = await sut.GetWishlistAsync(id);

            //Assert            
            Assert.Equal((int)HttpStatusCode.OK, (result.Result as ObjectResult).StatusCode);
        }

        [Fact]
        public async Task Post_shouldReturnCreated_WhenItIsPossibleToAddProduct()
        {
            //Arrange
            int customerId = 1;
            string productId = "1";
            WishListProduct existingProduct = new WishListProduct();
            _wishlistService.AddProductToCustomerrWishlistAsync(customerId, productId).Returns(existingProduct);

            var sut = new WishlistController(_wishlistService);

            //Act 
            var result = await sut.PostAsync(customerId, productId);

            //Assert            
            Assert.Equal((int)HttpStatusCode.Created, (result.Result as ObjectResult).StatusCode);
        }

        [Fact]
        public async Task Post_shouldReturnNotFound_WhenItIsNotPossibleToAddProduct()
        {
            //Arrange
            int customerId = 1;
            string productId = "1";
            WishListProduct notFound = null;
            _wishlistService.AddProductToCustomerrWishlistAsync(customerId, productId).Returns(notFound);

            var sut = new WishlistController(_wishlistService);

            //Act 
            var result = await sut.PostAsync(customerId, productId);

            //Assert            
            Assert.Equal((int)HttpStatusCode.NotFound, (result.Result as StatusCodeResult).StatusCode);
        }

        [Fact]
        public async Task Delete_shouldReturnOk_WhenItIsPossibleToAddProduct()
        {
            //Arrange
            int customerId = 1;
            string productId = "1";
            _wishlistService.RemoveProductFromCustomerrWishlistAsync(customerId, productId).Returns(true);

            var sut = new WishlistController(_wishlistService);

            //Act 
            var result = await sut.DeleteAsync(customerId, productId);

            //Assert            
            Assert.Equal((int)HttpStatusCode.OK, (result as StatusCodeResult).StatusCode);
        }

        [Fact]
        public async Task Delete_shouldReturnNotFound_WhenItIsNotPossibleToAddProduct()
        {
            //Arrange
            int customerId = 1;
            string productId = "1";
            _wishlistService.RemoveProductFromCustomerrWishlistAsync(customerId, productId).Returns(false);

            var sut = new WishlistController(_wishlistService);

            //Act 
            var result = await sut.DeleteAsync(customerId, productId);

            //Assert            
            Assert.Equal((int)HttpStatusCode.NotFound, (result as StatusCodeResult).StatusCode);
        }

    }
}
