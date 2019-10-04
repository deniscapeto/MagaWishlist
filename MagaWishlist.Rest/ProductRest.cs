using MagaWishlist.Core.Wishlist.Models;
using MagaWishlist.Core.Wishlist.Interfaces;
using MagaWishlist.Rest.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

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

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["ProductApiUrl"]}/{id}");

            var response = await _httpClientFactoryWrapper.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Product>(responseContent);
            }

            throw new HttpRequestException("Unable to get product informations");
        }
    }
}
