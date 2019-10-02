using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MagaWishlist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly JwtAuthentication _jwtAuthentication;
        public AuthController(IOptionsMonitor<JwtAuthentication> options)
        {
            _jwtAuthentication = options.CurrentValue;
        }

        [HttpGet]
        public ActionResult<JwtSecurityToken> Get()
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtAuthentication.SecurityKey);
            var token = new SecurityTokenDescriptor()
            {
                Issuer = _jwtAuthentication.Issuer,
                Audience = _jwtAuthentication.Audience,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            return Ok(new
            {
                token = handler.WriteToken(handler.CreateToken(token))
            });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }
    }
}
