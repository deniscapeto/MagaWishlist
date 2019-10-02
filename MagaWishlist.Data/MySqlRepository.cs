using Dapper;
using MagaWishlist.Core.Authorization.Interfaces;
using MagaWishlist.Core.Authorization.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace MagaWishlist.Data
{
    public class UserRepository : IUserRepository
    {
        readonly IConfiguration _configuration;
        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<User> FindAsync(string userID)
        {
            using (MySqlConnection conexao = new MySqlConnection(_configuration.GetConnectionString("defaultConnection")))
            {
                return conexao.QueryFirstOrDefaultAsync<User>(
                    "SELECT UserID, AccessKey " +
                    "FROM dbo.Users " +
                    "WHERE UserID = @UserID", new { UserID = userID});
            }
        }
    }    
}
