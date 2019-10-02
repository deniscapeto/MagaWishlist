using MagaWishlist.Models;
using Microsoft.IdentityModel.Tokens;

namespace MagaWishlist
{
    public interface IJwtSecurityTokenHelper
    {
        SecurityToken CreateToken();
        BearerTokenResponse CreateTokenReponse();
    }
}