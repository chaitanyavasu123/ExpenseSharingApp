using DataAccess.Data;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.DataAccess
{
    public class UserRepositoryTests
    {
        private readonly UserRepository _repository;
        private readonly ExpenseSharingDbContext _context;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ExpenseSharingDbContext>()
                .UseInMemoryDatabase(databaseName: "ExpenseSharingDb")
                .Options;

            _context = new ExpenseSharingDbContext(options);
            _repository = new UserRepository(_context);
        }

        private void SeedDatabase()
        {
            // Clear existing data
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();

            var users = new List<User>
            {
                new User { UserId = 1, Email = "user1@example.com", Name = "User One", Password = "Password1", Role = "User" },
                new User { UserId = 2, Email = "user2@example.com", Name = "User Two", Password = "Password2", Role = "User" }
            };

            _context.Users.AddRange(users);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetUserByIdAsync_ValidId_ReturnsUser()
        {
            // Arrange
            SeedDatabase();
            int userId = 1;

            // Act
            var result = await _repository.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task GetUserByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            SeedDatabase();
            int userId = 99;

            // Act
            var result = await _repository.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ExistingEmail_ReturnsUser()
        {
            // Arrange
            SeedDatabase();
            string email = "user1@example.com";

            // Act
            var result = await _repository.GetUserByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task GetUserByEmailAsync_NonExistingEmail_ReturnsNull()
        {
            // Arrange
            SeedDatabase();
            string email = "nonexistent@example.com";

            // Act
            var result = await _repository.GetUserByEmailAsync(email);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddUserAsync_ValidUser_UserAdded()
        {
            // Arrange
            SeedDatabase();
            var newUser = new User { UserId = 3, Name = "New User", Email = "newuser@example.com", Password = "Password3", Role = "User" };

            // Act
            await _repository.AddUserAsync(newUser);
            await _repository.SaveChangesAsync();

            // Assert
            var result = await _context.Users.FindAsync(3);
            Assert.NotNull(result);
            Assert.Equal(newUser.Name, result.Name);
        }

        [Fact]
        public async Task UpdateUserAsync_ValidUser_UserUpdated()
        {
            // Arrange
            SeedDatabase();
            var existingUser = await _context.Users.FindAsync(1);
            existingUser.Name = "Updated User";

            // Act
            await _repository.UpdateUserAsync(existingUser);
            await _repository.SaveChangesAsync();

            // Assert
            var result = await _context.Users.FindAsync(1);
            Assert.Equal("Updated User", result.Name);
        }

        [Fact]
        public async Task DeleteUserAsync_ValidId_UserDeleted()
        {
            // Arrange
            SeedDatabase();
            int userId = 1;

            // Act
            await _repository.DeleteUserAsync(userId);
            await _repository.SaveChangesAsync();

            // Assert
            var result = await _context.Users.FindAsync(userId);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            // Arrange
            SeedDatabase();

            // Act
            var result = await _repository.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }
}
