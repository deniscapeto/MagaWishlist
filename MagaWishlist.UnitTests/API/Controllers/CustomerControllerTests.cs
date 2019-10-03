using MagaWishlist.Controllers;
using NSubstitute;
using Xunit;
using System.Threading.Tasks;
using MagaWishlist.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using MagaWishlist.Core.Authorization.Interfaces;
using MagaWishlist.Core.Authorization.Models;

namespace MagaWishlist.UnitTests.API.Controllers
{
    public class CustomerControllerTests
    {
        readonly IWishlistService _wishlistService;
        public CustomerControllerTests()
        {
            _wishlistService = Substitute.For<IWishlistService>();
        }

        [Fact]
        public async Task GetAsync_shouldReturnNotFound_WhenNoCustomerFound()
        {
            //Arrange
            int id = 1;
            Customer notFound = null;
            _wishlistService.GetCustomerAsync(id).Returns(notFound);

            var sut = new CustomerController(_wishlistService);

            //Act 
            var result = await sut.GetAsync(id);

            //Assert            
            Assert.Equal((int)HttpStatusCode.NotFound, (result.Result as ObjectResult).StatusCode);
        }

        [Fact]
        public async Task GetAsync_shouldReturnValidResponse_WhenExistingCustomer()
        {
            //Arrange
            int id = 1;
            Customer existingCustomer = new Customer() { Id = 1, Name = "name", Email = "email-fake" };
            _wishlistService.GetCustomerAsync(id).Returns(existingCustomer);

            var sut = new CustomerController(_wishlistService);

            //Act 
            var result = await sut.GetAsync(id);

            //Assert            
            Assert.Equal((int)HttpStatusCode.OK, (result.Result as ObjectResult).StatusCode);
        }

        [Fact]
        public async Task PostAsync_shouldReturnBadRequest_WhenInvalidInput()
        {
            //Arrange
            int id = 1;
            CustomerViewModel newCustomer = new CustomerViewModel() { Name = "name", Email = "email-fake" };
            
            var sut = new CustomerController(_wishlistService);
            sut.ModelState.AddModelError("", "invalid data");


            //Act 
            var result = (ObjectResult)await sut.PostAsync(newCustomer);

            //Assert            
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task PostAsync_shouldReturnConflict_WhenExistingCustomer()
        {
            //Arrange
            int id = 1;
            string email = "fake@email.com";
            CustomerViewModel newCustomer = new CustomerViewModel() { Name = "name", Email = email };
            Customer existingCustomer = new Customer() { Id = 1, Name = "name", Email = email };
            _wishlistService.GetCustomerAsync(id).Returns(existingCustomer);

            var sut = new CustomerController(_wishlistService);

            //Act 
            var result = (ObjectResult)await sut.PostAsync(newCustomer);

            //Assert            
            Assert.Equal((int)HttpStatusCode.Conflict, result.StatusCode);
        }

        [Fact]
        public async Task PutAsync_shouldReturnBadRequest_WhenInvalidInput()
        {
            //Arrange
            int id = 1;
            CustomerViewModel modifiedCustomer = new CustomerViewModel() { Name = "name", Email = "email-fake" };

            var sut = new CustomerController(_wishlistService);
            sut.ModelState.AddModelError("", "invalid data");


            //Act 
            var result = (ObjectResult)await sut.PutAsync(id, modifiedCustomer);

            //Assert            
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task PutAsync_shouldReturnBadRequest_WhenInvalidInputId()
        {
            //Arrange
            int id = 0;
            CustomerViewModel modifiedCustomer = new CustomerViewModel() { Name = "name", Email = "email-fake" };

            var sut = new CustomerController(_wishlistService);
            sut.ModelState.AddModelError("", "invalid data");


            //Act 
            var result = (ObjectResult)await sut.PutAsync(id, modifiedCustomer);

            //Assert            
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }


        [Fact]
        public async Task PutAsync_shouldReturnOkAndCallService_WhenValidInput()
        {
            //Arrange
            int id = 1;
            string email = "fake@email.com";
            CustomerViewModel modifiedCustomerViewModel = new CustomerViewModel() { Name = "name complete", Email = email };
            Customer modifiedCustomer = new Customer() { Id = id, Name = "name complete", Email = email };

            _wishlistService.UpdateCustomerAsync(Arg.Any<Customer>()).Returns(modifiedCustomer);

            var sut = new CustomerController(_wishlistService);

            //Act 
            var result = (ObjectResult)await sut.PutAsync(id, modifiedCustomerViewModel);

            //Assert            
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            _ = _wishlistService.Received(1).UpdateCustomerAsync(Arg.Is<Customer>(
                x =>
                x.Id == id &&
                x.Email == email &&
                x.Name == "name complete"
                ));
        }

        [Fact]
        public async Task PutAsync_shouldReturnNotFound_WhenValidCustomerDoesNotExist()
        {
            //Arrange
            int id = 1;
            string email = "fake@email.com";
            CustomerViewModel modifiedCustomerViewModel = new CustomerViewModel() { Name = "name complete", Email = email };
            Customer nullReturn = null;

            _wishlistService.UpdateCustomerAsync(Arg.Any<Customer>()).Returns(nullReturn);

            var sut = new CustomerController(_wishlistService);

            //Act 
            var result = (ObjectResult)await sut.PutAsync(id, modifiedCustomerViewModel);

            //Assert            
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            _ = _wishlistService.Received(1).UpdateCustomerAsync(Arg.Is<Customer>(
                x =>
                x.Id == id &&
                x.Email == email &&
                x.Name == "name complete"
                ));
        }

        [Fact]
        public async Task DeleteAsync_shouldReturnBadRequest_WhenMissingId()
        {
            //Arrange
            int id = 0;

            var sut = new CustomerController(_wishlistService);

            //Act 
            var result = (ObjectResult)await sut.DeleteAsync(id);

            //Assert            
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task DeleteAsync_shouldReturnNoContent_WhenValidId()
        {
            //Arrange
            int id = 1;

            var sut = new CustomerController(_wishlistService);

            //Act 
            var result = (StatusCodeResult)await sut.DeleteAsync(id);

            //Assert            
            Assert.Equal((int)HttpStatusCode.NoContent, result.StatusCode);
        }
    }
}
