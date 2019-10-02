using MagaWishlist.Controllers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using MagaWishlist.Core.Authentication.Interfaces;
using System.Threading.Tasks;
using MagaWishlist.Models;
using MagaWishlist.Core.Authentication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagaWishlist.UnitTests.API.Controllers
{
    public class AuthControllerTests
    {
        IJwtSecurityTokenHelper tokenHelper;
        IAuthenticationService authenticationService;
        public AuthControllerTests()
        {
            tokenHelper = Substitute.For<IJwtSecurityTokenHelper>();
            authenticationService = Substitute.For<IAuthenticationService>();
        }

        [Fact]
        public async Task FindAsync_shouldReturnNotFound_WhenNoUserFound()
        {
            //Arrange
            tokenHelper = Substitute.For<IJwtSecurityTokenHelper>();
            authenticationService = Substitute.For<IAuthenticationService>();
            var viewModel = new AuthViewModel()
            {
                Username = "userTest",
                AccessKey ="userAccessKey"
            };

            User userReturn = null;
            authenticationService.FindAsync(viewModel.Username).Returns(userReturn);

            var sut = new AuthController(tokenHelper, authenticationService);

            //Act 
            var result = (ObjectResult)await sut.GetTokenAsync(viewModel);

            //Assert            
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task FindAsync_shouldReturnOk_WhenExistingUser()
        {
            //Arrange
            var viewModel = new AuthViewModel()
            {
                Username = "userTest",
                AccessKey = "userAccessKey"
            };

            User userReturn = new User() { UserID = viewModel.Username, AccessKey = viewModel.AccessKey };

            authenticationService.FindAsync(viewModel.Username).Returns(userReturn);
            var tokenResponse = new BearerTokenResponse() { Token = "tokenexemplo", Expiration = "01/02/2019 18:34:22" };
            tokenHelper.CreateTokenReponse().Returns(tokenResponse);

            var sut = new AuthController(tokenHelper, authenticationService);

            //Act 
            var result = (ObjectResult)await sut.GetTokenAsync(viewModel);

            //Assert            
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }


        [Fact]
        public async Task FindAsync_shouldReturnBadRequest_WhenInvalidInputData()
        {
            //Arrange
            var viewModel = new AuthViewModel()
            {
                Username = "",
                AccessKey = "userAccessKey"
            };

            var sut = new AuthController(tokenHelper, authenticationService);
            sut.ModelState.AddModelError("", "invalid data");

            //Act 
            var result = (ObjectResult)await sut.GetTokenAsync(viewModel);

            //Assert            
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        
    }
}
