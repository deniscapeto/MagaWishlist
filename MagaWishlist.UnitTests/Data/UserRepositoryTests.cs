using MagaWishlist.Core.Authorization.Models;
using MagaWishlist.Data;
using NSubstitute;
using System.Data;
using System.Threading.Tasks;
using Xunit;
using Dapper;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MagaWishlist.UnitTests")]
namespace MagaWishlist.UnitTests.Data
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task FindAsync_shouldReturnValidUser_whenValidId()
        {
            //Arrange
            string userId = "userTest";
            var connection = Substitute.For<IDbConnection>();
            var userFromRepo = new User() { UserID = userId };            
            dynamic parameters;

            //parameters.UserID = userId;

            //connection.QueryFirstOrDefaultAsync<User>(Arg.Do<string>((u) => ""),
            //    Arg.Do<object>(o => { parameters = o; })).ReturnsForAnyArgs(userFromRepo);

            connection.QueryFirstOrDefaultAsync<User>(Arg.Any<string>(),
                Arg.Do<object>(o => { parameters = o; })).ReturnsForAnyArgs(userFromRepo);

            var sut = new UserRepository(connection);

//            DbExecutor.Query<Order>("select * from Orders where CustomerId=@CustomerId and ShipperId=@ShipperId",
//)

            //Act
            var user = await sut.FindAsync(userId);

            //Assert
            Assert.Equal(userFromRepo.UserID, user.UserID);
        }
    }
}
