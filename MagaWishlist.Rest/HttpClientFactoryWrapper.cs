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

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, string policyName)
        {
            using (var httpClient = string.IsNullOrEmpty(policyName) ? _httpClientFactory.CreateClient() : _httpClientFactory.CreateClient(policyName))
            {
                return httpClient.SendAsync(request);
            }
        }
    }
}
