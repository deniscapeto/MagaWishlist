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
            var newCustomer = new Customer()
            {
                Name = name,
                Email = email
            };

            if (await CustomerEmailExistsAsync(newCustomer))
                return null;

            return await _customerRepository.InsertAsync(newCustomer);
        }

        public Task<Customer> GetCustomerAsync(int id)
        {
            return _customerRepository.GetByIdAsync(id);
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            if (!await CustomerIdExistsAsync(id))
                return false;

            return await _customerRepository.DeleteAsync(id);
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            if (!await CustomerIdExistsAsync(customer.Id))
                return null;

            if (await CustomerEmailExistsAsync(customer))
                return null;

            return await _customerRepository.UpdateAsync(customer);
        }

        private async Task<bool> CustomerIdExistsAsync(int id)
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(id);
            return existingCustomer != null;
        }

        private async Task<bool> CustomerEmailExistsAsync(Customer customer)
        {
            var existingCustomer = await _customerRepository.GetByEmailAsync(customer.Email);

            if (existingCustomer == null)
                return false;

            return existingCustomer.Id != customer.Id;
        }
    }
}
