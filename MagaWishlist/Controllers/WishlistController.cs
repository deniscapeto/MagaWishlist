using System.Collections.Generic;
using System.Threading.Tasks;
using MagaWishlist.Core.Wishlist.Interfaces;
using MagaWishlist.Core.Wishlist.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagaWishlist.Controllers
{
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        /// <summary>
        /// Get Customer wishlist products
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <returns>Product list</returns>
        [HttpGet]
        [Route("api/customer/{customerId}/wishlist/")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<WishListProduct>>> GetWishlistAsync(int customerId)
        {
            if (customerId == 0)
                return BadRequest("custumerId was not provided");

            var result = await _wishlistService.GetCustomerWishlistAsync(customerId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Add new product to customer wishlist
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <param name="productId">Product Id</param>
        [HttpPost]
        [Route("api/customer/{customerId}/wishlist/{productId}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<WishListProduct>> PostAsync(int customerId, string productId)
        {
            if (customerId == 0 || string.IsNullOrEmpty(productId))
                return BadRequest("Provide both customerId and productId");

            var result = await _wishlistService.AddProductToCustomerrWishlistAsync(customerId, productId);

            if (result == null)
                return NotFound();

            return Created($"/api/{customerId}/wishlist/{productId}", result);
        }

        /// <summary>
        /// Removes a product from customer wishlist
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <param name="productId">Product Id</param>
        [HttpDelete]
        [Route("api/customer/{customerId}/wishlist/{productId}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteAsync(int customerId, string productId)
        {
            if (customerId == 0 || string.IsNullOrEmpty(productId))
                return BadRequest("Provide both customerId and productId");

            var success = await _wishlistService.RemoveProductFromCustomerrWishlistAsync(customerId, productId);

            if (success)
                return Ok();
            else
                return NotFound();
        }

    }
}
