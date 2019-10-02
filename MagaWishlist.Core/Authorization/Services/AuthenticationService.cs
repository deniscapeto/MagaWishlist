using MagaWishlist.Core.Authorization.Interfaces;
using MagaWishlist.Core.Authorization.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace MagaWishlist.Core.Authentication.Services
{
    public class AuthenticationService
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
