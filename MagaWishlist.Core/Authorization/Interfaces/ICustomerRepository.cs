using MagaWishlist.Core.Authorization.Models;
using System.Threading.Tasks;

namespace MagaWishlist.Core.Authorization.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> InsertAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
        Task<Customer> GetByEmailAsync(string email);
        Task<Customer> GetByIdAsync(int id);
    }
}
