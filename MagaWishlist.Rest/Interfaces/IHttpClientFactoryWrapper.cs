using System.Net.Http;
using System.Threading.Tasks;

namespace MagaWishlist.Rest.Interfaces
{
    public interface IHttpClientFactoryWrapper
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, string policyName = "");
    }
}
