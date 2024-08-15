using BusinessLogic.Services;
using DataAccess.Repositories;
using Models;
using Models.Dtos;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.BusinessLogic
{
    public class UserServiceNegativeTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IGroupRepository> _mockGroupRepository;
        private readonly Mock<IExpenseRepository> _mockExpenseRepository;
        private readonly UserService _userService;

        public UserServiceNegativeTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockGroupRepository = new Mock<IGroupRepository>();
            _mockExpenseRepository = new Mock<IExpenseRepository>();
            _userService = new UserService(
                _mockUserRepository.Object,
                _mockGroupRepository.Object,
                _mockExpenseRepository.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_UserNotFound_ReturnsNull()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_UserNotFound_ReturnsNull()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.AuthenticateAsync("nonexistent@example.com", "password");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_InvalidPassword_ReturnsNull()
        {
            // Arrange
            var user = new User { Email = "user@example.com", Password = "correctpassword" };
            _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.AuthenticateAsync("user@example.com", "wrongpassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserGroupsAsync_UserNotInAnyGroups_ReturnsEmptyList()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetUserGroupsAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Group>());

            // Act
            var result = await _userService.GetUserGroupsAsync(999);

            // Assert
            Assert.Empty(result);
        }


        [Fact]
        public async Task InviteUserToGroupAsync_GroupNotFound_ReturnsFalse()
        {
            // Arrange
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Group)null);

            // Act
            var result = await _userService.InviteUserToGroupAsync(999, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task InviteUserToGroupAsync_GroupIsFull_ReturnsFalse()
        {
            // Arrange
            var group = new Group
            {
                GroupId = 1,
                Members = new List<GroupMember>(Enumerable.Repeat(new GroupMember(), 10))
            };
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(group);

            // Act
            var result = await _userService.InviteUserToGroupAsync(1, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task InviteUserToGroupAsync_UserAlreadyInGroup_ReturnsFalse()
        {
            // Arrange
            var group = new Group
            {
                GroupId = 1,
                Members = new List<GroupMember> { new GroupMember { UserId = 1 } }
            };
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(group);

            // Act
            var result = await _userService.InviteUserToGroupAsync(1, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExitGroupAsync_GroupNotFound_DoesNothing()
        {
            // Arrange
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Group)null);

            // Act
            await _userService.ExitGroupAsync(999, 1);

            // Assert
            _mockGroupRepository.Verify(repo => repo.UpdateGroupAsync(It.IsAny<Group>()), Times.Never);
        }

        [Fact]
        public async Task ExitGroupAsync_UserNotInGroup_DoesNothing()
        {
            // Arrange
            var group = new Group
            {
                GroupId = 1,
                Members = new List<GroupMember>()
            };
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(group);

            // Act
            await _userService.ExitGroupAsync(1, 1);

            // Assert
            _mockGroupRepository.Verify(repo => repo.UpdateGroupAsync(It.IsAny<Group>()), Times.Never);
        }

        [Fact]
        public async Task DeleteGroupAsync_GroupNotFound_DoesNothing()
        {
            // Arrange
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Group)null);

            // Act
            await _userService.DeleteGroupAsync(999);

            // Assert
            _mockGroupRepository.Verify(repo => repo.DeleteGroupAsync(It.IsAny<Group>()), Times.Never);
        }

        [Fact]
        public async Task GetUserBalanceAsync_UserNotFound_ReturnsNull()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserBalanceAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task JoinGroupAsync_GroupNotFound_ReturnsFalse()
        {
            // Arrange
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Group)null);

            // Act
            var result = await _userService.JoinGroupAsync(1, 999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task JoinGroupAsync_UserNotFound_ReturnsFalse()
        {
            // Arrange
            var group = new Group { GroupId = 1 };
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(group);
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.JoinGroupAsync(999, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_NoUsers_ReturnsEmptyList()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(new List<User>());

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserIdsByGroupIdAsync_NoUsersInGroup_ReturnsEmptyList()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetUserIdsByGroupIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<int>());

            // Act
            var result = await _userService.GetUserIdsByGroupIdAsync(1);

            // Assert
            Assert.Empty(result);
        }
    }
}

