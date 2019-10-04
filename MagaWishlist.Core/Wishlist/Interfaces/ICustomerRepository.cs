using MagaWishlist.Core.Wishlist.Models;
using System.Threading.Tasks;

namespace MagaWishlist.Core.Wishlist.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> InsertAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(int id);
        Task<Customer> GetByEmailAsync(string email);
        Task<Customer> GetByIdAsync(int id);
    }
}
