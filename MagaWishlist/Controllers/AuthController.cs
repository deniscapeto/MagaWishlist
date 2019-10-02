using System.Linq;
using System.Threading.Tasks;
using MagaWishlist.Core.Authentication.Interfaces;
using MagaWishlist.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagaWishlist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IJwtSecurityTokenHelper _jwtSecurityTokenHelper;
        readonly IAuthenticationService _authenticationService;
        public AuthController(
            IJwtSecurityTokenHelper jwtSecurityTokenHelper,
            IAuthenticationService authenticationService)
        {
            _jwtSecurityTokenHelper = jwtSecurityTokenHelper;
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<ActionResult> GetTokenAsync([FromBody] AuthViewModel authViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();

                return BadRequest(errors);
            }

            var user = await _authenticationService.FindAsync(authViewModel.Username);

            if (user == null)
                return NotFound(authViewModel.Username);

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
