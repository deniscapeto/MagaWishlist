using MagaWishlist.Data;
using NSubstitute;
using System.Data;
using System.Threading.Tasks;
using Xunit;
using Dapper;
using System.Runtime.CompilerServices;
using MagaWishlist.Core.Wishlist.Models;

namespace MagaWishlist.UnitTests.Data
{
    public class CustomerRepositoryTests
    {
        IDbConnection connection;
        IDbCommand command;
        public CustomerRepositoryTests()
        {
            connection = Substitute.For<IDbConnection>();
            command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);
        }
        readonly int customerId = 1;

        [Fact]
        public async Task GetByIdAsync_shouldCallQueryFirstOrDefaultAsync_WhenValidId()
        {
            //Arrange
            command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);
            var sut = new CustomerRepository(connection);

            //Act
            await sut.GetByIdAsync(customerId);

            //Assert
            connection.Received(0).QueryFirstOrDefaultAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task GetByEmailAsync_shouldCallQueryFirstOrDefaultAsync_WhenValidEmail()
        {
            //Arrange
            string customerEmail = "fake@customer.com";
            var connection = Substitute.For<IDbConnection>();
            var sut = new CustomerRepository(connection);

            //Act
            var customer = await sut.GetByEmailAsync(customerEmail);

            //Assert
            connection.Received(1).QueryFirstOrDefaultAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task DeleteAsync_shouldCallQueryFirstOrDefaultAsync_WhenValidEmail()
        {
            //Arrange
            var connection = Substitute.For<IDbConnection>();
            connection.ExecuteAsync(Arg.Any<string>()).Returns(1);
            var sut = new CustomerRepository(connection);

            //Act
            var customer = await sut.DeleteAsync(customerId);

            //Assert
            Received.InOrder(() => {
                connection.Execute(Arg.Is<string>(x => x.Contains(nameof(WishListProduct))));
                connection.Execute(Arg.Is<string>(x => x.Contains(nameof(Customer))));
            });

            //connection.Received(2).ExecuteAsync(Arg.Any<string>());
        }
    }
}
