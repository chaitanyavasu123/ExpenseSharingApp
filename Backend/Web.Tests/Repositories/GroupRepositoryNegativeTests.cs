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
    public class GroupRepositoryNegativeTests
    {
        private readonly ExpenseSharingDbContext _context;
        private readonly GroupRepository _repository;

        public GroupRepositoryNegativeTests()
        {
            var options = new DbContextOptionsBuilder<ExpenseSharingDbContext>()
                .UseInMemoryDatabase(databaseName: "TestGroupDb_NegativeTests")
                .Options;

            _context = new ExpenseSharingDbContext(options);
            _repository = new GroupRepository(_context);
        }

        [Fact]
        public async Task GetGroupByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var invalidGroupId = 999;

            // Act
            var result = await _repository.GetGroupByIdAsync(invalidGroupId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddGroupAsync_NullGroup_ThrowsException()
        {
            // Arrange
            Group nullGroup = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddGroupAsync(nullGroup));
        }

        [Fact]
        public async Task UpdateGroupAsync_InvalidGroup_ThrowsException()
        {
            // Arrange
            var invalidGroup = new Group { GroupId = 999, Name = "Invalid" };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _repository.UpdateGroupAsync(invalidGroup));
        }

        [Fact]
        public async Task DeleteGroupAsync_InvalidGroup_ThrowsException()
        {
            // Arrange
            var invalidGroup = new Group { GroupId = 999, Name = "Invalid" };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _repository.DeleteGroupAsync(invalidGroup));
        }

        [Fact]
        public async Task GetAllGroupsAsync_NoGroups_ReturnsEmptyList()
        {
            // Arrange
            // Ensure database is empty
            foreach (var group in _context.Groups.ToList())
            {
                _context.Groups.Remove(group);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllGroupsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGroupsUserIsNotInAsync_InvalidUserId_ReturnsEmptyList()
        {
            // Arrange
            var invalidUserId = 999;

            // Act
            var result = await _repository.GetGroupsUserIsNotInAsync(invalidUserId);

            // Assert
            Assert.Empty(result);
        }
    }
}

