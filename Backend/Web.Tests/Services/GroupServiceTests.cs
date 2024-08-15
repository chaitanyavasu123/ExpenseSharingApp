using BusinessLogic.Services;
using DataAccess.Repositories;
using Models;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.BusinessLogic.Services
{
    public class GroupServiceTests
    {
        private readonly Mock<IGroupRepository> _mockGroupRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IExpenseRepository> _mockExpenseRepository;
        private readonly GroupService _groupService;

        public GroupServiceTests()
        {
            _mockGroupRepository = new Mock<IGroupRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _groupService = new GroupService(_mockGroupRepository.Object, _mockUserRepository.Object,_mockExpenseRepository.Object);
        }

        [Fact]
        public async Task CreateGroupAsync_ValidGroup_GroupCreated()
        {
            // Arrange
            var group = new Group { GroupId = 1, Name = "Group 1" };

            // Act
            await _groupService.CreateGroupAsync(group);

            // Assert
            _mockGroupRepository.Verify(repo => repo.AddGroupAsync(group), Times.Once);
        }

        [Fact]
        public async Task GetGroupByIdAsync_ValidGroupId_ReturnsGroup()
        {
            // Arrange
            var groupId = 1;
            var group = new Group { GroupId = groupId, Name = "Group 1" };
            _mockGroupRepository.Setup(repo => repo.GetGroupByIdAsync(groupId)).ReturnsAsync(group);

            // Act
            var result = await _groupService.GetGroupByIdAsync(groupId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(groupId, result.GroupId);
        }

        [Fact]
        public async Task DeleteGroupAsync_GroupExistsAndUserIsCreator_ReturnsTrue()
        {
            // Arrange
            int groupId = 1;
            int currentUserId = 2;
            var group = new Group { GroupId = groupId, CreatedById = currentUserId };

            _mockGroupRepository.Setup(x => x.GetGroupByIdAsync(groupId)).ReturnsAsync(group);
            _mockGroupRepository.Setup(x => x.DeleteGroupAsync(group)).Returns(Task.CompletedTask);

            // Act
            var result = await _groupService.DeleteGroupAsync(groupId, currentUserId);

            // Assert
            Assert.True(result);
            _mockGroupRepository.Verify(x => x.GetGroupByIdAsync(groupId), Times.Once);
            _mockGroupRepository.Verify(x => x.DeleteGroupAsync(group), Times.Once);
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
            var result = await _groupService.InviteUserToGroupAsync(groupId, userId);

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
            var result = await _groupService.InviteUserToGroupAsync(groupId, userId);

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
            await _groupService.ExitGroupAsync(groupId, userId);

            // Assert
            Assert.Empty(group.Members);
            _mockGroupRepository.Verify(repo => repo.UpdateGroupAsync(group), Times.Once);
        }

        [Fact]
        public async Task GetAllGroupsAsync_ReturnsAllGroups()
        {
            // Arrange
            var groups = new List<Group>
            {
                new Group { GroupId = 1, Name = "Group 1" },
                new Group { GroupId = 2, Name = "Group 2" }
            };
            _mockGroupRepository.Setup(repo => repo.GetAllGroupsAsync()).ReturnsAsync(groups);

            // Act
            var result = await _groupService.GetAllGroupsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }
    }

}

