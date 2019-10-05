using NSubstitute;
using System.Threading.Tasks;
using Xunit;
using MagaWishlist.Core.Wishlist.Models;
using MagaWishlist.Rest.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MagaWishlist.Rest;
using System.Net.Http;
using System;
using Polly.Timeout;
using Polly.CircuitBreaker;

namespace MagaWishlist.UnitTests.Core.Services
{
    public class ProductRestTests
    {
        readonly IHttpClientFactoryWrapper _httpClientFactoryWrapper;
        readonly IConfiguration _configuration;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogger<ProductRest> _logger;
        string productId = "1bf0f365-fbdd-4e21-9786-da459d78dd1f";

        public ProductRestTests()
        {
            _httpClientFactoryWrapper = Substitute.For<IHttpClientFactoryWrapper>();
            _configuration = Substitute.For<IConfiguration>();
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _configuration["ProductApiUrl"] = "http://mocked.test.com";
            _logger = Substitute.For<ILogger<ProductRest>>();
            _loggerFactory.CreateLogger<ProductRest>().Returns(_logger);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenExternalServiceResponseIsValid()
        {
            HttpResponseMessage response = GetSuccessMockResponse();

            _httpClientFactoryWrapper.SendAsync(Arg.Any<HttpRequestMessage>(), "product").Returns(response);

            //Act
            var sut = new ProductRest(_httpClientFactoryWrapper, _configuration, _loggerFactory);
            var product = await sut.GetProductByIdAsync(productId);

            //Assert
            Assert.NotNull(product);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldCallExternalService_WhenProductIdIsValid()
        {
            HttpResponseMessage response = GetSuccessMockResponse();

            _httpClientFactoryWrapper.SendAsync(Arg.Any<HttpRequestMessage>(), "product").Returns(response);

            //Act
            var sut = new ProductRest(_httpClientFactoryWrapper, _configuration, _loggerFactory);
            var product = await sut.GetProductByIdAsync(productId);

            //Assert
            Assert.NotNull(product);
            _ = _httpClientFactoryWrapper.Received(1).SendAsync(Arg.Any<HttpRequestMessage>(), "product");
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldThrowArgumentException_WhenProductIdIsInValid()
        {
            //Arrange
            var sut = new ProductRest(_httpClientFactoryWrapper, _configuration, _loggerFactory);

            //Act
            Func<Task<WishListProduct>> func = () => sut.GetProductByIdAsync(string.Empty);

            //Assert
            Assert.ThrowsAsync<ArgumentException>(func);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldThrowHttpRequestException_WhenCircuitBrakerIsOpen()
        {
            //Arrange
            var sut = new ProductRest(_httpClientFactoryWrapper, _configuration, _loggerFactory);
            _httpClientFactoryWrapper
                .When(x => x.SendAsync(Arg.Any<HttpRequestMessage>(), "product"))
                .Do((callinfo) => throw new BrokenCircuitException());

            //Act
            Func<Task<WishListProduct>> func = () => sut.GetProductByIdAsync(productId);

            //Assert
            Assert.ThrowsAsync<HttpRequestException>(func);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldThrowHttpRequestException_WhenTimeoutIsReached()
        {
            //Arrange
            var sut = new ProductRest(_httpClientFactoryWrapper, _configuration, _loggerFactory);
            _httpClientFactoryWrapper
                .When(x => x.SendAsync(Arg.Any<HttpRequestMessage>(), "product"))
                .Do((callinfo) => throw new TimeoutRejectedException());

            //Act
            Func<Task<WishListProduct>> func = () => sut.GetProductByIdAsync(productId);

            //Assert
            Assert.ThrowsAsync<HttpRequestException>(func);
        }

        private static HttpResponseMessage GetSuccessMockResponse()
        {
            //Arrange
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent("{" +
                "\"price\": 1699.0, " +
                "\"image\": \"http://challenge-api.luizalabs.com/images/1bf0f365-fbdd-4e21-9786-da459d78dd1f.jpg\", " +
                "\"brand\": \"b\u00e9b\u00e9 confort\", " +
                "\"id\": \"1bf0f365-fbdd-4e21-9786-da459d78dd1f\", " +
                "\"title\": \"Cadeira para Auto Iseos B\u00e9b\u00e9 Confort Earth Brown\"" +
                "}");
            return response;
        }

    }
}
