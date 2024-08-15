using DataAccess.Data;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.DataAccess
{
    public class UserRepositoryNegativeTests
    {
        private readonly ExpenseSharingDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryNegativeTests()
        {
            var options = new DbContextOptionsBuilder<ExpenseSharingDbContext>()
                .UseInMemoryDatabase(databaseName: "TestUserDb_NegativeTests")
                .Options;

            _context = new ExpenseSharingDbContext(options);
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async Task GetUserByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var invalidUserId = 999;

            // Act
            var result = await _repository.GetUserByIdAsync(invalidUserId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByEmailAsync_InvalidEmail_ReturnsNull()
        {
            // Arrange
            var invalidEmail = "nonexistent@example.com";

            // Act
            var result = await _repository.GetUserByEmailAsync(invalidEmail);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddUserAsync_NullUser_ThrowsException()
        {
            // Arrange
            User nullUser = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddUserAsync(nullUser));
        }

        [Fact]
        public async Task UpdateUserAsync_InvalidUser_ThrowsException()
        {
            // Arrange
            var invalidUser = new User { UserId = 999, Email = "invalid@example.com" };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _repository.UpdateUserAsync(invalidUser));
        }

        [Fact]
        public async Task DeleteUserAsync_InvalidUserId_DoesNothing()
        {
            // Arrange
            var invalidUserId = 999;

            // Act
            await _repository.DeleteUserAsync(invalidUserId);

            // Assert
            var result = await _repository.GetUserByIdAsync(invalidUserId);
            Assert.Null(result); // Ensure the user still doesn't exist
        }

        [Fact]
        public async Task GetUserGroupsAsync_InvalidUserId_ReturnsEmptyList()
        {
            // Arrange
            var invalidUserId = 999;

            // Act
            var result = await _repository.GetUserGroupsAsync(invalidUserId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_NoUsers_ReturnsEmptyList()
        {
            // Arrange
            // Ensure database is empty
            foreach (var user in _context.Users.ToList())
            {
                _context.Users.Remove(user);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllUsersAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserIdsByGroupIdAsync_InvalidGroupId_ReturnsEmptyList()
        {
            // Arrange
            var invalidGroupId = 999;

            // Act
            var result = await _repository.GetUserIdsByGroupIdAsync(invalidGroupId);

            // Assert
            Assert.Empty(result);
        }
    }
}

