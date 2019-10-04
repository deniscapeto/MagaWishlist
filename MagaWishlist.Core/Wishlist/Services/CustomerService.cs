using MagaWishlist.Core.Wishlist.Interfaces;
using MagaWishlist.Core.Wishlist.Models;
using System;
using System.Threading.Tasks;

namespace MagaWishlist.Core.Wishlist.Services
{
    public class CustomerService : ICustomerService
    {
        readonly ICustomerRepository _customerRepository;
        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository ?? throw new ArgumentException("customerRepository");
        }

        public async Task<Customer> AddNewCustomerAsync(string name, string email)
        {
            if (await CustomerEmailExistsAsync(email))
                return null;

            var newCustomer = new Customer()
            {
                Name = name,
                Email = email
            };
            return await _customerRepository.InsertAsync(newCustomer);
        }

        public Task<Customer> GetCustomerAsync(int id)
        {
            return _customerRepository.GetByIdAsync(id);
        }

        public async Task DeleteCustomerAsync(int id)
        {
            if (!await CustomerIdExistsAsync(id))
                return;

            await _customerRepository.DeleteAsync(id);
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            if (!await CustomerIdExistsAsync(customer.Id))
                return null;

            return await _customerRepository.UpdateAsync(customer);
        }

        private async Task<bool> CustomerIdExistsAsync(int id)
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(id);
            return existingCustomer != null;
        }

        private async Task<bool> CustomerEmailExistsAsync(string email)
        {
            var existingCustomer = await _customerRepository.GetByEmailAsync(email);
            return existingCustomer != null;
        }
    }
}
