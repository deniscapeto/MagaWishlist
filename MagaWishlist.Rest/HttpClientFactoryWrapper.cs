using MagaWishlist.Rest.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MagaWishlist.Rest
{
    public class HttpClientFactoryWrapper : IHttpClientFactoryWrapper
    {
        readonly IHttpClientFactory _httpClientFactory;
        public HttpClientFactoryWrapper(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            var httpClient = _httpClientFactory.CreateClient();
            return httpClient.SendAsync(request);
        }
    }
}
