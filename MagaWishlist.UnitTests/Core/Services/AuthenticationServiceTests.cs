using MagaWishlist.Core.Authentication.Services;
using MagaWishlist.Core.Authentication.Interfaces;
using MagaWishlist.Core.Authentication.Models;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace MagaWishlist.UnitTests.Core.Services
{
    public class AuthenticationServiceTests
    {
        [Fact]
        public async Task FindAsync_shouldReturnValidUser_WhenValidId()
        {
            //Arrange
            string userId = "userTest";
            var repository = Substitute.For<IUserRepository>();
            var userFromRepo = new User() { UserID = userId };
            repository.FindAsync(userId).Returns(userFromRepo);
            var sut = new AuthenticationService(repository);

            //Act
            var user = await sut.FindAsync(userId);

            //Assert
            Assert.Equal(userFromRepo.UserID, user.UserID );
        }

        [Fact]
        public async Task FindAsync_shouldReturnNull_WhenUserNotFound()
        {
            //Arrange
            string userId = "userTest";
            var repository = Substitute.For<IUserRepository>();
            User userFromRepo = null;
            repository.FindAsync(userId).Returns(userFromRepo);
            var sut = new AuthenticationService(repository);

            //Act
            var user = await sut.FindAsync(userId);

            //Assert
            Assert.Null(user);
        }
    }
}
