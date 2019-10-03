﻿using Dapper;
using MagaWishlist.Core.Authorization.Interfaces;
using MagaWishlist.Core.Authorization.Models;
using System.Data;
using System.Threading.Tasks;

namespace MagaWishlist.Data
{
    public class CustomerRepository : ICustomerRepository
    {
        readonly IDbConnection _connection;
        public CustomerRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task DeleteAsync(int id)
        {
            return _connection.ExecuteAsync(
                "DELETE " +
                "FROM Customers " +
                "WHERE Id = @id", new { id });
        }

        public Task<Customer> GetByEmailAsync(string email)
        {
            return _connection.QueryFirstOrDefaultAsync<Customer>(
                "SELECT Id, Name, Email " +
                "FROM Customers " +
                "WHERE email = @email", new {  email });
        }

        public Task<Customer> GetByIdAsync(int id)
        {
            return _connection.QueryFirstOrDefaultAsync<Customer>(
                "SELECT Id, Name, Email " +
                "FROM Customers " +
                "WHERE id = @id", new { id });
        }

        public async Task<Customer> InsertAsync(Customer customer)
        {
            await _connection.ExecuteAsync(
                "INSERT INTO " +
                "Customers (name, email) " +
                "VALUES (@Name,@Email)", 
                new { customer.Name, customer.Email });

            return await GetByEmailAsync(customer.Email);
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            await _connection.ExecuteAsync(
                "UPDATE Customers SET" +
                " name = @Name, " +
                " email = @Email " +
                " WHERE id = @Id ",
                new { customer.Name, customer.Email, customer.Id });

            return await GetByIdAsync(customer.Id);
        }
    }
}
