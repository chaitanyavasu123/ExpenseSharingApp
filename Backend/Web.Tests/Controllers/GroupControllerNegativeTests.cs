using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Controllers;
using Models;
using Xunit;
using Microsoft.AspNetCore.Http;

public class GroupControllerNegativeTests
{
    private readonly Mock<IGroupService> _mockGroupService;
    private readonly GroupController _controller;

    public GroupControllerNegativeTests()
    {
        _mockGroupService = new Mock<IGroupService>();
        _controller = new GroupController(_mockGroupService.Object);
    }

    [Fact]
    public async Task GetAllGroups_NoGroupsFound_ReturnsEmptyList()
    {
        // Arrange
        _mockGroupService.Setup(service => service.GetAllGroupsAsync())
                         .ReturnsAsync(new List<Group>());

        // Act
        var result = await _controller.GetAllGroups();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var groups = Assert.IsType<List<Group>>(okResult.Value);
        Assert.Empty(groups);
    }

    [Fact]
    public async Task GetGroup_GroupNotFound_ReturnsNotFound()
    {
        // Arrange
        var groupId = 1;
        _mockGroupService.Setup(service => service.GetGroupByIdAsync(groupId))
                         .ReturnsAsync((Group)null);

        // Act
        var result = await _controller.GetGroup(groupId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteGroup_GroupNotFoundOrNotAuthorized_ReturnsForbid()
    {
        // Arrange
        var groupId = 1;
        var currentUserId = 1;

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId", currentUserId.ToString())
                }))
            }
        };

        _mockGroupService.Setup(service => service.DeleteGroupAsync(groupId, currentUserId))
                         .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteGroup(groupId);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task InviteUserToGroup_GroupNotFoundOrFull_ReturnsBadRequest()
    {
        // Arrange
        var groupId = 1;
        var userId = 1;

        _mockGroupService.Setup(service => service.InviteUserToGroupAsync(groupId, userId))
                         .ReturnsAsync(false);

        // Act
        var result = await _controller.InviteUserToGroup(groupId, userId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to add user to group or group already has 10 members.", badRequestResult.Value);
    }

    [Fact]
    public async Task ExitGroup_GroupNotFound_ReturnsOk()
    {
        // Arrange
        var groupId = 1;
        var userId = 1;

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId", userId.ToString())
                }))
            }
        };

        _mockGroupService.Setup(service => service.ExitGroupAsync(groupId, userId))
                         .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ExitGroup(groupId);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    

}

