using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagaWishlist.Core.Wishlist.Interfaces;
using MagaWishlist.Core.Wishlist.Models;
using MagaWishlist.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagaWishlist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<CustomerViewModel>> GetAsync(int id)
        {
            var customer = await _customerService.GetCustomerAsync(id);

            if (customer == null)
                return NotFound(id);

            
            return Ok(new CustomerViewModel()
            {
                Id = customer.Id.ToString(),
                Name = customer.Name,
                Email = customer.Email
            });
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] CustomerViewModel customer)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();

                return BadRequest(errors);
            }

            var customerAdded = await _customerService.AddNewCustomerAsync(customer.Name, customer.Email);

            if (customerAdded == null)
                return Conflict($"Email {customer.Email} is already being used");

            return Created($"/api/customer/{customerAdded.Id}", new CustomerViewModel()
            {
                Id = customerAdded.Id.ToString(),
                Name = customerAdded.Name,
                Email = customerAdded.Email
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(int id, [FromBody] CustomerViewModel customer)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();

                return BadRequest(errors);
            }

            if (id == 0)
                return BadRequest("Please provide valid customer ID to update");

            var customerToUpdate = new Customer() { Id = id, Email = customer.Email, Name = customer.Name };
            var customerUpdated = await _customerService.UpdateCustomerAsync(customerToUpdate);

            if (customerUpdated != null)
                return Ok(new CustomerViewModel()
                {
                    Id = customerUpdated.Id.ToString(),
                    Name = customerUpdated.Name,
                    Email = customerUpdated.Email
                });
            else
                return NotFound(customer);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            if (id == 0)
                return BadRequest("Please provide valid customer ID to delete");

            await _customerService.DeleteCustomerAsync(id);

            return NoContent();
        }
    }
}
