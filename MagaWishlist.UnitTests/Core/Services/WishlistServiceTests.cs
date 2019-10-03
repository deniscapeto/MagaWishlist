using NSubstitute;
using System.Threading.Tasks;
using Xunit;
using MagaWishlist.Core.Authorization.Interfaces;
using MagaWishlist.Core.Authorization.Models;
using MagaWishlist.Core.Authorization.Services;

namespace MagaWishlist.UnitTests.Core.Services
{
    public class WishlistServiceTests
    {
        readonly ICustomerRepository _customerRepository;
        public WishlistServiceTests()
        {
            _customerRepository = Substitute.For<ICustomerRepository>();
        }

        [Fact]
        public async Task AddNewCustomerAsync_ShouldReturnNull_WhenCustomerDoesExist()
        {
            //Arrange
            string email = "fake-email";

            Customer existingCustomer = new Customer() { Name = "name", Email = email };
            _customerRepository.GetByEmailAsync(email).Returns(existingCustomer);

            //Act
            var sut = new WishlistService(_customerRepository);
            var cutomerReturn = await sut.AddNewCustomerAsync("name", email);

            //Assert
            Assert.Null(cutomerReturn);
        }

        [Fact]
        public async Task AddNewCustomerAsync_ShouldPassCustomerToRepository_WhenCustomerDoesNotExist()
        {
            //Arrange
            string email = "fake-email";

            Customer nullReturn = null;
            _customerRepository.GetByEmailAsync(email).Returns(nullReturn);

            //Act
            var sut = new WishlistService(_customerRepository);
            var cutomerReturn = await sut.AddNewCustomerAsync("name", email);

            //Assert
            await _customerRepository.Received(1).InsertAsync(
                Arg.Is<Customer>(
                    x =>
                    x.Email == email &&
                    x.Name == "name"
                    )
                );
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
        {
            //Arrange
            int id = 1;

            Customer nullReturn = null;
            _customerRepository.GetByIdAsync(id).Returns(nullReturn);

            //Act
            var sut = new WishlistService(_customerRepository);
            var cutomerReturn = await sut.UpdateCustomerAsync(new Customer());

            //Assert
            Assert.Null(cutomerReturn);
            await _customerRepository.DidNotReceive().UpdateAsync(
                Arg.Any<Customer>()
                );
        }

        [Fact]
        public async Task updateCustomerAsync_ShouldCallRepository_WhenCustomerDoesExist()
        {
            //Arrange
            int id = 1;

            Customer existingCustomer = new Customer() { Id = 1, Name = "name", Email = "email-fake" };
            Customer updatedCustomer = new Customer() { Id = 1, Name = "name2", Email = "email-fake2" };
            _customerRepository.GetByIdAsync(id).Returns(existingCustomer);

            //Act
            var sut = new WishlistService(_customerRepository);
            await sut.UpdateCustomerAsync(updatedCustomer);

            //Assert
            await _customerRepository.Received().UpdateAsync(
                    Arg.Is<Customer>
                    (
                        x =>
                        x.Email == updatedCustomer.Email &&
                        x.Name == updatedCustomer.Name &&
                        x.Id == updatedCustomer.Id
                    )
                );
        }

        [Fact]
        public async Task DeleteCustomerAsync_ShouldNotCallRepository_WhenCustomerDoesNotExist()
        {
            //Arrange
            int id = 1;

            Customer nullReturn = null;
            _customerRepository.GetByIdAsync(id).Returns(nullReturn);

            //Act
            var sut = new WishlistService(_customerRepository);
            await sut.DeleteCustomerAsync(id);

            //Assert
            await _customerRepository.DidNotReceive().DeleteAsync(Arg.Any<int>());
        }

        [Fact]
        public async Task DeleteCustomerAsync_ShouldCallRepository_WhenCustomerDoesExist()
        {
            //Arrange
            int id = 1;

            Customer existingCustomer = new Customer() { Id = 1, Name = "name", Email = "email-fake" };
            _customerRepository.GetByIdAsync(id).Returns(existingCustomer);

            //Act
            var sut = new WishlistService(_customerRepository);
            await sut.DeleteCustomerAsync(id);

            //Assert
            await _customerRepository.Received().DeleteAsync(id);
        }

        [Fact]
        public async Task GetCustomerAsync_ShouldReturnValidCustomer_WhenCustomerExistsInRepository()
        {
            //Arrange
            string email = "fake-email";

            Customer existingCustomer = new Customer() { Id = 1, Name = "name", Email = email };
            _customerRepository.GetByIdAsync(1).Returns(existingCustomer);

            //Act
            var sut = new WishlistService(_customerRepository);
            var customer = await sut.GetCustomerAsync(1);

            //Assert
            Assert.NotNull(customer);
            await _customerRepository.Received().GetByIdAsync(1);
        }
    }
}
