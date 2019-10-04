using System.Threading.Tasks;
using MagaWishlist.Core.Wishlist.Models;

namespace MagaWishlist.Core.Wishlist.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer> AddNewCustomerAsync(string name, string email);
        Task DeleteCustomerAsync(int id);
        Task<Customer> GetCustomerAsync(int id);
        Task<Customer> UpdateCustomerAsync(Customer customer);
    }
}