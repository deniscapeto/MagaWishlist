using MagaWishlist.Core.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MagaWishlist.Core.Authentication.Interfaces
{
    public interface IUserRepository
    {
        Task<User> FindAsync(string userID);
    }
}
