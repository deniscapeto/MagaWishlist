using System.Collections.Generic;
using System.Threading.Tasks;
using MagaWishlist.Core.Wishlist.Interfaces;
using MagaWishlist.Core.Wishlist.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagaWishlist.Controllers
{
    [ApiController]
    public class WishlistController : ControllerBase
    {
        readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet]
        [Route("api/customer/{customerId}/wishlist/")]
        [Authorize]
        public async Task<ActionResult<List<WishListProduct>>> GetWishlistAsync(int customerId)
        {
            if (customerId == 0)
                return BadRequest("custumerId was not provided");

            var result = await _wishlistService.GetCustomerWishlistAsync(customerId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Route("api/customer/{customerId}/wishlist/{productId}")]
        public async Task<ActionResult<WishListProduct>> PostAsync(int customerId, string productId)
        {
            var result = await _wishlistService.AddProductToCustomerrWishlistAsync(customerId, productId);

            if (result == null)
                return NotFound();

            return Created($"/api/{customerId}/wishlist/{productId}", result);
        }

        [HttpDelete]
        [Route("api/customer/{customerId}/wishlist/{productId}")]
        public async Task<ActionResult> DeleteAsync(int customerId, string productId)
        {
            var success = await _wishlistService.RemoveProductFromCustomerrWishlistAsync(customerId, productId);

            if (success)
                return Ok();
            else
                return NotFound();
        }

    }
}
