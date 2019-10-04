using MagaWishlist.Core.Wishlist.Models;
using MagaWishlist.Core.Wishlist.Interfaces;
using MagaWishlist.Rest.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Polly.Timeout;

namespace MagaWishlist.Rest
{
    public class ProductRest : IProductRest
    {
        readonly IHttpClientFactoryWrapper _httpClientFactoryWrapper;
        IConfiguration _configuration;

        public ProductRest(IHttpClientFactoryWrapper httpClientFactoryWrapper, IConfiguration configuration)
        {
            _httpClientFactoryWrapper = httpClientFactoryWrapper;
            _configuration = configuration;
        }

        public async Task<WishListProduct> GetProductByIdAsync(string id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["ProductApiUrl"]}/{id}");

            HttpResponseMessage response;
            try
            {
                response = await _httpClientFactoryWrapper.SendAsync(request, "product");
            }
            catch (TimeoutRejectedException)
            {
                throw new HttpRequestException("Unable to get product informations");
            }

            if (response.IsSuccessStatusCode)
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

            throw new HttpRequestException("Unable to get product informations");
        }
    }
}
