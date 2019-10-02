using MagaWishlist.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace MagaWishlist
{
    public class JwtSecurityTokenHelper : IJwtSecurityTokenHelper
    {
        readonly JwtAuthentication _jwtAuthentication;
        public JwtSecurityTokenHelper(IOptionsMonitor<JwtAuthentication> options)
        {
            _jwtAuthentication = options.CurrentValue;
        }

        public BearerTokenResponse CreateTokenReponse()
        {
            var handler = new JwtSecurityTokenHandler();

            var token = CreateToken();

            var tokenResponse = new BearerTokenResponse { Token = handler.WriteToken(token), Expiration = token.ValidTo.ToString() };

            return tokenResponse;
        }

        public SecurityToken CreateToken()
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtAuthentication.SecurityKey);

            if (!double.TryParse(_jwtAuthentication.ExpirationInMinutes, out double expiration))
            {
                expiration = 60;
            }

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = _jwtAuthentication.Issuer,
                Audience = _jwtAuthentication.Audience,
                Expires = DateTime.UtcNow.AddMinutes(expiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            return handler.CreateToken(tokenDescriptor);
        }
    }
}
