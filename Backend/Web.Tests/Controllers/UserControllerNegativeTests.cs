using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Models;
using Models.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Controllers;
using Xunit;
using Microsoft.AspNetCore.Http;

public class UserControllerNegativeTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly UserController _controller;

    public UserControllerNegativeTests()
    {
        _mockUserService = new Mock<IUserService>();
        _controller = new UserController(_mockUserService.Object);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "test@example.com", Password = "wrongpassword" };
        _mockUserService.Setup(service => service.AuthenticateAsync(loginDto.Email, loginDto.Password))
                        .ReturnsAsync((User)null);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid email or password.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task GetUserById_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        _mockUserService.Setup(service => service.GetUserByIdAsync(userId))
                        .ReturnsAsync((User)null);

        // Act
        var result = await _controller.GetUserById(userId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetUserGroups_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        _mockUserService.Setup(service => service.GetUserGroupsAsync(userId))
                        .ReturnsAsync((IEnumerable<Group>)null);

        // Act
        var result = await _controller.GetUserGroups(userId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task InviteUserToGroup_GroupOrUserInvalid_ReturnsBadRequest()
    {
        // Arrange
        var groupId = 1;
        var userId = 1;
        _mockUserService.Setup(service => service.InviteUserToGroupAsync(groupId, userId))
                        .ReturnsAsync(false);

        // Act
        var result = await _controller.InviteUserToGroup(groupId, userId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to add user to group or group already has 10 members.", badRequestResult.Value);
    }

    [Fact]
    public async Task JoinGroup_FailedToJoin_ReturnsBadRequest()
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

        _mockUserService.Setup(service => service.JoinGroupAsync(userId, groupId))
                        .ReturnsAsync(false);

        // Act
        var result = await _controller.JoinGroup(groupId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to join the group.", badRequestResult.Value);
    }

    [Fact]
    public async Task ExitGroup_GroupOrUserInvalid_ReturnsOk()
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

        _mockUserService.Setup(service => service.ExitGroupAsync(groupId, userId))
                        .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ExitGroup(groupId);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteGroup_GroupNotFound_ReturnsNoContent()
    {
        // Arrange
        var groupId = 1;

        _mockUserService.Setup(service => service.DeleteGroupAsync(groupId))
                        .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteGroup(groupId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    
    [Fact]
    public async Task GetAllUsers_NoUsersFound_ReturnsEmptyList()
    {
        // Arrange
        _mockUserService.Setup(service => service.GetAllUsersAsync())
                        .ReturnsAsync(new List<User>());

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsType<List<User>>(okResult.Value);
        Assert.Empty(users);
    }
}
