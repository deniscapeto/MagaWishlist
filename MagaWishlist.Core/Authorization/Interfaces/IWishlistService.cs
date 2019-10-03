using System.Threading.Tasks;
using MagaWishlist.Core.Authorization.Models;

namespace MagaWishlist.Core.Authorization.Interfaces
{
    public interface IWishlistService
    {
        Task<Customer> AddNewCustomerAsync(string name, string email);
        Task DeleteCustomerAsync(int id);
        Task<Customer> GetCustomerAsync(int id);
        Task<Customer> UpdateCustomerAsync(Customer customer);
    }
}