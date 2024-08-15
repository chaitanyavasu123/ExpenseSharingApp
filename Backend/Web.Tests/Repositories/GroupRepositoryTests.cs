using DataAccess.Data;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.DataAccess
{
    public class GroupRepositoryTests
    {
        private DbContextOptions<ExpenseSharingDbContext> _contextOptions;
        private readonly InMemoryDatabaseRoot _databaseRoot;

        public GroupRepositoryTests()
        {
            _databaseRoot = new InMemoryDatabaseRoot();
            _contextOptions = new DbContextOptionsBuilder<ExpenseSharingDbContext>()
                .UseInMemoryDatabase("ExpenseSharingDb", _databaseRoot)
                .Options;
        }

        private void SeedDatabase(ExpenseSharingDbContext context)
        {
            // Seed users
            var users = new List<User>
            {
                new User { UserId = 1, Email = "user1@example.com", Name = "User One", Password = "Password1", Role = "User" },
                new User { UserId = 2, Email = "user2@example.com", Name = "User Two", Password = "Password2", Role = "User" }
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            // Seed groups
            var groups = new List<Group>
            {
                new Group { GroupId = 1, Name = "Group One",Description="This is First Group", Members = new List<GroupMember> {
                    new GroupMember { GroupId = 1, UserId = 1 },
                    new GroupMember { GroupId = 1, UserId = 2 }
                }},
                new Group { GroupId = 2, Name = "Group Two",Description="This is Second Group" }
            };

            context.Groups.AddRange(groups);
            context.SaveChanges();
        }

        private ExpenseSharingDbContext CreateContext()
        {
            var context = new ExpenseSharingDbContext(_contextOptions);
            SeedDatabase(context);
            return context;
        }

        [Fact]
        public async Task GetGroupByIdAsync_ValidId_ReturnsGroup()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new GroupRepository(context);
            int groupId = 1;

            // Act
            var result = await repository.GetGroupByIdAsync(groupId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(groupId, result.GroupId);
            Assert.Equal(2, result.Members.Count);
        }

        [Fact]
        public async Task GetGroupByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new GroupRepository(context);
            int groupId = 99;

            // Act
            var result = await repository.GetGroupByIdAsync(groupId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddGroupAsync_ValidGroup_GroupAdded()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new GroupRepository(context);
            var newGroup = new Group { GroupId = 3, Name = "New Group",Description="This is New Group" };

            // Act
            await repository.AddGroupAsync(newGroup);
            var result = await context.Groups.FindAsync(3);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newGroup.Name, result.Name);
        }

        [Fact]
        public async Task UpdateGroupAsync_ValidGroup_GroupUpdated()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new GroupRepository(context);
            var existingGroup = await context.Groups.FindAsync(1);
            existingGroup.Name = "Updated Group";

            // Act
            await repository.UpdateGroupAsync(existingGroup);
            var result = await context.Groups.FindAsync(1);

            // Assert
            Assert.Equal("Updated Group", result.Name);
        }

        [Fact]
        public async Task DeleteGroupAsync_ValidGroup_GroupDeleted()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new GroupRepository(context);
            var existingGroup = await context.Groups.Include(g => g.Members).FirstAsync(g => g.GroupId == 1);

            // Act
            await repository.DeleteGroupAsync(existingGroup);
            var result = await context.Groups.FindAsync(1);
            var groupMembers = await context.GroupMembers.Where(gm => gm.GroupId == 1).ToListAsync();

            // Assert
            Assert.Null(result);
            Assert.Empty(groupMembers);
        }
        [Fact]
        public async Task GetAllGroupsAsync_ReturnsAllGroups()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new GroupRepository(context);

            // Act
            var result = await repository.GetAllGroupsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }
}
