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

        /// <summary>
        /// Get customer informations
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <returns>Customer Informations</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
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

        /// <summary>
        /// Add new customer with empty wishlist
        /// </summary>
        /// <param name="customer">Customer name and e-mail</param>        
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(201)]
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

        /// <summary>
        /// Update customer informations. Name and E-mail       
        /// </summary>
        /// <remarks>
        /// Notice that e-mail must be unique. It's not possible to have more than one customer using the same e-mail.
        /// </remarks>
        /// <param name="id">customer Id</param>
        /// <param name="customer">Customer informations</param>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
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

        /// <summary>
        /// Delete a Customer from MagaWishlist 
        /// </summary>
        /// <param name="id">customer Id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            if (id == 0)
                return BadRequest("Please provide valid customer ID to delete");

            await _customerService.DeleteCustomerAsync(id);

            return NoContent();
        }
    }
}
