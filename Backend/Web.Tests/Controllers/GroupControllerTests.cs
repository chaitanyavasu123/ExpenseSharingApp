using BusinessLogic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Controllers;
using Xunit;

namespace UnitTests.Web.Controllers
{
    public class GroupControllerTests
    {
        private readonly Mock<IGroupService> _groupServiceMock;
        private readonly GroupController _groupController;

        public GroupControllerTests()
        {
            _groupServiceMock = new Mock<IGroupService>();
            _groupController = new GroupController(_groupServiceMock.Object);
        }
        private void SetupUserClaims(int userId)
        {
            var userClaims = new List<Claim>
            {
                new Claim("UserId", userId.ToString())
            };
            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _groupController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task GetAllGroups_ReturnsOkWithGroups()
        {
            // Arrange
            var groups = new List<Group> { new Group { GroupId = 1, Name = "Group1" } };
            _groupServiceMock.Setup(x => x.GetAllGroupsAsync()).ReturnsAsync(groups);

            // Act
            var result = await _groupController.GetAllGroups();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(groups, okResult.Value);
        }

        [Fact]
        public async Task CreateGroup_ValidGroup_ReturnsOk()
        {
            // Arrange
            var group = new Group { GroupId = 1, Name = "New Group" };

            // Act
            var result = await _groupController.CreateGroup(group);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _groupServiceMock.Verify(x => x.CreateGroupAsync(group), Times.Once);
        }

        [Fact]
        public async Task GetGroup_ExistingGroupId_ReturnsOkWithGroup()
        {
            // Arrange
            int groupId = 1;
            var group = new Group { GroupId = groupId, Name = "Group1" };
            _groupServiceMock.Setup(x => x.GetGroupByIdAsync(groupId)).ReturnsAsync(group);

            // Act
            var result = await _groupController.GetGroup(groupId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(group, okResult.Value);
        }

        [Fact]
        public async Task GetGroup_NonExistingGroupId_ReturnsNotFound()
        {
            // Arrange
            int groupId = 1;
            _groupServiceMock.Setup(x => x.GetGroupByIdAsync(groupId)).ReturnsAsync((Group)null);

            // Act
            var result = await _groupController.GetGroup(groupId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task InviteUserToGroup_ValidInput_ReturnsOk()
        {
            // Arrange
            int groupId = 1;
            int userId = 2;
            _groupServiceMock.Setup(x => x.InviteUserToGroupAsync(groupId, userId)).ReturnsAsync(true);

            // Act
            var result = await _groupController.InviteUserToGroup(groupId, userId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _groupServiceMock.Verify(x => x.InviteUserToGroupAsync(groupId, userId), Times.Once);
        }

        [Fact]
        public async Task InviteUserToGroup_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            int groupId = 1;
            int userId = 2;
            _groupServiceMock.Setup(x => x.InviteUserToGroupAsync(groupId, userId)).ReturnsAsync(false);

            // Act
            var result = await _groupController.InviteUserToGroup(groupId, userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to add user to group or group already has 10 members.", badRequestResult.Value);
        }

        [Fact]
        public async Task ExitGroup_ValidInput_ReturnsOk()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("UserId", "2")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _groupController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            int groupId = 1;

            // Act
            var result = await _groupController.ExitGroup(groupId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _groupServiceMock.Verify(x => x.ExitGroupAsync(groupId, 2), Times.Once);
        }
    }
}
