using MagaWishlist.Core.Wishlist.Models;
using MagaWishlist.Core.Wishlist.Interfaces;
using MagaWishlist.Rest.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Polly.Timeout;
using Polly.CircuitBreaker;
using Microsoft.Extensions.Logging;

namespace MagaWishlist.Rest
{
    public class ProductRest : IProductRest
    {
        readonly IHttpClientFactoryWrapper _httpClientFactoryWrapper;
        readonly IConfiguration _configuration;
        readonly ILogger<ProductRest> _logger;

        public ProductRest(IHttpClientFactoryWrapper httpClientFactoryWrapper, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _httpClientFactoryWrapper = httpClientFactoryWrapper ?? throw new ArgumentNullException(nameof(httpClientFactoryWrapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = loggerFactory?.CreateLogger<ProductRest>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task<WishListProduct> GetProductByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Product id not provided");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["ProductAPI:Url"]}/{id}");

            HttpResponseMessage response;
            try
            {
                response = await _httpClientFactoryWrapper.SendAsync(request, "product");
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError($"Get Product by Id: {ex.Message}");
                throw new HttpRequestException("Unable to get product informations.");
            }
            catch (TimeoutRejectedException)
            {
                _logger.LogError($"Unable to get product informations.Timeout was reached");
                throw new HttpRequestException("Unable to get product informations.");
            }

            if (response.IsSuccessStatusCode)
            {
                return await ProcessResponse(response);
            }

            throw new HttpRequestException("Unable to get product informations");
        }

        private static async Task<WishListProduct> ProcessResponse(HttpResponseMessage response)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<Product>(responseContent);

            return new WishListProduct()
            {
                ProductId = product.id,
                Image = product.image,
                Price = product.price.ToString(),
                Title = product.title
            };
        }
    }
}
