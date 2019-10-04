using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagaWishlist.Core.Wishlist.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagaWishlist.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        [HttpGet]
        [Route("api/customer/{idCustomer}/wishlist/")]
        [Authorize]
        public async Task<ActionResult<List<Product>>> GetWishlistAsync(int id)
        {
            var customer = new Customer();
            customer.AddToWishlist(1);
            return customer.Wishlist;
        }

        //// GET: api/Wishlist
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/Wishlist/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/Wishlist
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: api/Wishlist/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
