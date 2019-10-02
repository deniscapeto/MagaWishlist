using MagaWishlist.Core.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MagaWishlist.Core.Authorization.Interfaces
{
    public interface IUserRepository
    {
        Task<User> FindAsync(string userID);
    }
}
