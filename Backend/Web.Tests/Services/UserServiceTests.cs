using BusinessLogic.Services;
using DataAccess.Repositories;
using Models;
using Models.Dtos;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.BusinessLogic.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IGroupRepository> _mockGroupRepository;
        private readonly Mock<IExpenseRepository> _mockExpenseRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockGroupRepository = new Mock<IGroupRepository>();
            _mockExpenseRepository = new Mock<IExpenseRepository>();
            _userService = new UserService(_mockUserRepository.Object, _mockGroupRepository.Object, _mockExpenseRepository.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidUser_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password";
            var user = new User { Email = email, Password = password };
            _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(user);

            // Act
            var result = await _userService.AuthenticateAsync(email, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task AuthenticateAsync_InvalidUser_ReturnsNull()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password";
            _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.AuthenticateAsync(email, password);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserGroupsAsync_ValidUserId_ReturnsGroups()
        {
            // Arrange
            var userId = 1;
            var groups = new List<Group> { new Group { GroupId = 1, Name = "Group 1" } };
            _mockUserRepository.Setup(repo => repo.GetUserGroupsAsync(userId)).ReturnsAsync(groups);

            // Act
            var result = await _userService.GetUserGroupsAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task CreateGroupAsync_ValidGroup_GroupCreated()
        {
            // Arrange
            var group = new Group { GroupId = 1, Name = "Group 1", Members = new List<GroupMember>() };
            var userId = 1;

            // Act
            await _userService.CreateGroupAsync(group, userId);

            // Assert
            _mockGroupRepository.Verify(repo => repo.AddGroupAsync(group), Times.Once);
            Assert.Single(group.Members);
            Assert.Equal(userId, group.Members.First().UserId);
        }

        [Fact]
        public async Task InviteUserToGroupAsync_ValidGroupAndUser_ReturnsTrue()
        {
            // Arrange
            var groupId = 1;
            var userId = 2;
            var group = new Group { GroupId = groupId, Members = new List<GroupMember>() };
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(groupId)).ReturnsAsync(group);

            // Act
            var result = await _userService.InviteUserToGroupAsync(groupId, userId);

            // Assert
            Assert.True(result);
            _mockGroupRepository.Verify(repo => repo.UpdateGroupAsync(group), Times.Once);
            Assert.Single(group.Members);
            Assert.Equal(userId, group.Members.First().UserId);
        }

        [Fact]
        public async Task InviteUserToGroupAsync_ExistingMember_ReturnsFalse()
        {
            // Arrange
            var groupId = 1;
            var userId = 2;
            var group = new Group { GroupId = groupId, Members = new List<GroupMember> { new GroupMember { UserId = userId } } };
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(groupId)).ReturnsAsync(group);

            // Act
            var result = await _userService.InviteUserToGroupAsync(groupId, userId);

            // Assert
            Assert.False(result);
            _mockGroupRepository.Verify(repo => repo.UpdateGroupAsync(It.IsAny<Group>()), Times.Never);
        }

        [Fact]
        public async Task ExitGroupAsync_ValidGroupAndUser_UserRemoved()
        {
            // Arrange
            var groupId = 1;
            var userId = 1;
            var group = new Group { GroupId = groupId, Members = new List<GroupMember> { new GroupMember { UserId = userId } } };
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(groupId)).ReturnsAsync(group);

            // Act
            await _userService.ExitGroupAsync(groupId, userId);

            // Assert
            Assert.Empty(group.Members);
            _mockGroupRepository.Verify(repo => repo.UpdateGroupAsync(group), Times.Once);
        }

        [Fact]
        public async Task DeleteGroupAsync_ValidGroupId_GroupDeleted()
        {
            // Arrange
            var groupId = 1;
            var group = new Group { GroupId = groupId };
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(groupId)).ReturnsAsync(group);

            // Act
            await _userService.DeleteGroupAsync(groupId);

            // Assert
            _mockGroupRepository.Verify(repo => repo.DeleteGroupAsync(group), Times.Once);
        }

        [Fact]
        public async Task GetUserBalanceAsync_ValidUserId_ReturnsBalance()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = userId, AmountOwed = 100, AmountOwedTo = 50 };
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserBalanceAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100, result.AmountOwed);
            Assert.Equal(50, result.AmountOwedTo);
        }

        [Fact]
        public async Task JoinGroupAsync_ValidUserIdAndGroupId_ReturnsTrue()
        {
            // Arrange
            var groupId = 1;
            var userId = 1;
            var group = new Group { GroupId = groupId, Members = new List<GroupMember>() };
            var user = new User { UserId = userId };
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(groupId)).ReturnsAsync(group);
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.JoinGroupAsync(userId, groupId);

            // Assert
            Assert.True(result);
            _mockGroupRepository.Verify(repo => repo.UpdateGroupAsync(group), Times.Once);
            Assert.Single(group.Members);
            Assert.Equal(userId, group.Members.First().UserId);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserId = 1, Email = "user1@example.com" },
                new User { UserId = 2, Email = "user2@example.com" }
            };
            _mockUserRepository.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }
    }
}

