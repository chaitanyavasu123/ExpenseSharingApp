using BusinessLogic.Services;
using DataAccess.Repositories;
using Models;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.BusinessLogic
{
    public class GroupServiceNegativeTests
    {
        private readonly Mock<IGroupRepository> _mockGroupRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IExpenseRepository> _mockExpenseRepository;
        private readonly GroupService _groupService;

        public GroupServiceNegativeTests()
        {
            _mockGroupRepository = new Mock<IGroupRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _groupService = new GroupService(
                _mockGroupRepository.Object,
                _mockUserRepository.Object,_mockExpenseRepository.Object);
        }

        [Fact]
        public async Task GetGroupByIdAsync_GroupNotFound_ReturnsNull()
        {
            // Arrange
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Group)null);

            // Act
            var result = await _groupService.GetGroupByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteGroupAsync_GroupNotFound_ReturnsFalse()
        {
            // Arrange
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Group)null);

            // Act
            var result = await _groupService.DeleteGroupAsync(999, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteGroupAsync_NotGroupCreator_ReturnsFalse()
        {
            // Arrange
            var group = new Group { GroupId = 1, CreatedById = 2 };
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(group);

            // Act
            var result = await _groupService.DeleteGroupAsync(1, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task InviteUserToGroupAsync_GroupNotFound_ReturnsFalse()
        {
            // Arrange
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Group)null);

            // Act
            var result = await _groupService.InviteUserToGroupAsync(999, 1);

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
            var result = await _groupService.InviteUserToGroupAsync(1, 1);

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
            var result = await _groupService.InviteUserToGroupAsync(1, 1);

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
            await _groupService.ExitGroupAsync(999, 1);

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
            await _groupService.ExitGroupAsync(1, 1);

            // Assert
            _mockGroupRepository.Verify(repo => repo.UpdateGroupAsync(It.IsAny<Group>()), Times.Never);
        }

        [Fact]
        public async Task GetGroupsUserIsNotInAsync_InvalidUserId_ReturnsEmptyList()
        {
            // Arrange
            _mockGroupRepository.Setup(repo => repo.GetGroupsUserIsNotInAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Group>());

            // Act
            var result = await _groupService.GetGroupsUserIsNotInAsync(999);

            // Assert
            Assert.Empty(result);
        }
    }
}
