using Dapper;
using MagaWishlist.Core.Authentication.Interfaces;
using MagaWishlist.Core.Authentication.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading.Tasks;

namespace MagaWishlist.Data
{
    public class UserRepository : IUserRepository
    {
        readonly IDbConnection _connection;
        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<User> FindAsync(string userID)
        {
            return _connection.QueryFirstOrDefaultAsync<User>(
                "SELECT UserID, AccessKey " +
                "FROM Users " +
                "WHERE UserID = @UserID", new { UserID = userID});
        }
    }
}
