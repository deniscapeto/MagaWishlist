using NSubstitute;
using System.Threading.Tasks;
using Xunit;
using MagaWishlist.Core.Wishlist.Interfaces;
using MagaWishlist.Core.Wishlist.Models;
using MagaWishlist.Core.Wishlist.Services;

namespace MagaWishlist.UnitTests.Core.Services
{
    public class CustomerServiceTests
    {
        readonly ICustomerRepository _customerRepository;
        readonly Customer existingCustomer = new Customer() { Id = 1, Name = "name", Email = "email-fake" };
        readonly Customer nullReturn = null;
        public CustomerServiceTests()
        {
            _customerRepository = Substitute.For<ICustomerRepository>();
        }

        [Fact]
        public async Task AddNewCustomerAsync_ShouldReturnNull_WhenCustomerDoesExist()
        {
            //Arrange
            _customerRepository.GetByEmailAsync(Arg.Any<string>()).Returns(existingCustomer);

            //Act
            var sut = new CustomerService(_customerRepository);
            var cutomerReturn = await sut.AddNewCustomerAsync("name", existingCustomer.Email);

            //Assert
            Assert.Null(cutomerReturn);
        }

        [Fact]
        public async Task AddNewCustomerAsync_ShouldPassCustomerToRepository_WhenCustomerDoesNotExist()
        {
            //Arrange
            string email = "fake-email";
            _customerRepository.GetByEmailAsync(email).Returns(nullReturn);

            //Act
            var sut = new CustomerService(_customerRepository);
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
            _customerRepository.GetByIdAsync(id).Returns(nullReturn);

            //Act
            var sut = new CustomerService(_customerRepository);
            var cutomerReturn = await sut.UpdateCustomerAsync(new Customer());

            //Assert
            Assert.Null(cutomerReturn);
            await _customerRepository.DidNotReceive().UpdateAsync(
                Arg.Any<Customer>()
                );
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShouldReturnNull_WhenItAlreadyExistACustomerWithTheSameEmail()
        {
            //Arrange
            int id = 1;
            _customerRepository.GetByIdAsync(id).Returns(nullReturn);
            _customerRepository.GetByEmailAsync(Arg.Any<string>()).Returns(existingCustomer);

            //Act
            var sut = new CustomerService(_customerRepository);
            var cutomerReturn = await sut.UpdateCustomerAsync(new Customer());

            //Assert
            Assert.Null(cutomerReturn);
            await _customerRepository.DidNotReceive().UpdateAsync(
                Arg.Any<Customer>()
                );
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShouldCallRepository_WhenTheEmailThatAlreadyExistsBelongsToTheSameCustomer()
        {
            //Arrange
            int id = 1;
            var modifiedCustomer = new Customer() { Id = 1, Name = "name", Email = "email-fake" };
            _customerRepository.GetByIdAsync(id).Returns(modifiedCustomer);
            _customerRepository.GetByEmailAsync(Arg.Any<string>()).Returns(modifiedCustomer);

            //Act
            var sut = new CustomerService(_customerRepository);
            var sameCustomer = new Customer()
            {
                Id = modifiedCustomer.Id,
                Email = modifiedCustomer.Email,
                Name = modifiedCustomer.Name
            };
            var cutomerReturn = await sut.UpdateCustomerAsync(sameCustomer);

            //Assert
            await _customerRepository.Received(1).UpdateAsync(
                Arg.Any<Customer>()
                );
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShouldCallRepository_WhenCustomerDoesExist()
        {
            //Arrange
            int id = 1;
            Customer updatedCustomer = new Customer() { Id = 1, Name = "name2", Email = "email-fake2" };
            _customerRepository.GetByIdAsync(id).Returns(existingCustomer);

            //Act
            var sut = new CustomerService(_customerRepository);
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
            _customerRepository.GetByIdAsync(id).Returns(nullReturn);

            //Act
            var sut = new CustomerService(_customerRepository);
            var result = await sut.DeleteCustomerAsync(id);

            //Assert
            await _customerRepository.DidNotReceive().DeleteAsync(Arg.Any<int>());
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteCustomerAsync_ShouldCallRepository_WhenCustomerDoesExist()
        {
            //Arrange
            int id = 1;
            _customerRepository.GetByIdAsync(id).Returns(existingCustomer);
            _customerRepository.DeleteAsync(id).Returns(true);

            //Act
            var sut = new CustomerService(_customerRepository);
            var result = await sut.DeleteCustomerAsync(id);

            //Assert
            await _customerRepository.Received().DeleteAsync(id);
            Assert.True(result);
        }

        [Fact]
        public async Task GetCustomerAsync_ShouldReturnValidCustomer_WhenCustomerExistsInRepository()
        {
            //Arrange
            _customerRepository.GetByIdAsync(1).Returns(existingCustomer);

            //Act
            var sut = new CustomerService(_customerRepository);
            var customer = await sut.GetCustomerAsync(1);

            //Assert
            Assert.NotNull(customer);
            await _customerRepository.Received().GetByIdAsync(1);
        }
    }
}
