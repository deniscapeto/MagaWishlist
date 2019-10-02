using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using MagaWishlist.Models;
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
        readonly IJwtSecurityTokenHelper _jwtSecurityTokenHelper;
        public AuthController(IJwtSecurityTokenHelper jwtSecurityTokenHelper)
        {
            _jwtSecurityTokenHelper = jwtSecurityTokenHelper;
        }

        [HttpPost]
        public ActionResult GetToken([FromBody] AuthViewModel authViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();

                return BadRequest(errors);
            }

            var tokenResponse = _jwtSecurityTokenHelper.CreateTokenReponse();

            return Ok(tokenResponse);
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }
    }
}
