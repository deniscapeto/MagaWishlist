using MagaWishlist.Core.Authentication.Interfaces;
using MagaWishlist.Core.Authentication.Models;
using System.Threading.Tasks;

namespace MagaWishlist.Core.Authentication.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<User> FindAsync(string userID)
        {
            return _userRepository.FindAsync(userID);
        }
    }
}
