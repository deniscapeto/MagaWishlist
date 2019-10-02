using System.Threading.Tasks;
using MagaWishlist.Core.Authentication.Models;

namespace MagaWishlist.Core.Authentication.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User> FindAsync(string userID);
    }
}