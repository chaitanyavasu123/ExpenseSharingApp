using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Models;
using Models.Dtos;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Controllers;
using Xunit;
using Microsoft.AspNetCore.Http;

namespace UnitTests.Web.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _userController = new UserController(_userServiceMock.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "test@example.com", Password = "password" };
            var user = new User { UserId = 1, Email = "test@example.com", Role = "User" };
            _userServiceMock.Setup(x => x.AuthenticateAsync(loginDto.Email, loginDto.Password)).ReturnsAsync(user);

            // Act
            var result = await _userController.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            Assert.Contains("Token", okResult.Value.ToString());
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "test@example.com", Password = "wrongpassword" };
            _userServiceMock.Setup(x => x.AuthenticateAsync(loginDto.Email, loginDto.Password)).ReturnsAsync((User)null);

            // Act
            var result = await _userController.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid email or password.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Logout_ValidUser_ReturnsOk()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("UserId", "1")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _userController.Logout();

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _userServiceMock.Verify(x => x.LogoutAsync(1), Times.Once);
        }

       

        [Fact]
        public async Task CreateGroup_ValidGroup_ReturnsOk()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("UserId", "1")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var group = new Group { Name = "New Group" };

            // Act
            var result = await _userController.CreateGroup(group);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _userServiceMock.Verify(x => x.CreateGroupAsync(group, 1), Times.Once);
        }

        [Fact]
        public async Task InviteUserToGroup_ValidInput_ReturnsOk()
        {
            // Arrange
            int groupId = 1;
            int userId = 2;
            _userServiceMock.Setup(x => x.InviteUserToGroupAsync(groupId, userId)).ReturnsAsync(true);

            // Act
            var result = await _userController.InviteUserToGroup(groupId, userId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _userServiceMock.Verify(x => x.InviteUserToGroupAsync(groupId, userId), Times.Once);
        }

        [Fact]
        public async Task InviteUserToGroup_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            int groupId = 1;
            int userId = 2;
            _userServiceMock.Setup(x => x.InviteUserToGroupAsync(groupId, userId)).ReturnsAsync(false);

            // Act
            var result = await _userController.InviteUserToGroup(groupId, userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to add user to group or group already has 10 members.", badRequestResult.Value);
        }

        [Fact]
        public async Task JoinGroup_ValidInput_ReturnsOk()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("UserId", "2")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            int groupId = 1;
            _userServiceMock.Setup(x => x.JoinGroupAsync(2, groupId)).ReturnsAsync(true);

            // Act
            var result = await _userController.JoinGroup(groupId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully joined the group.", okResult.Value);
        }

        [Fact]
        public async Task JoinGroup_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("UserId", "2")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            int groupId = 1;
            _userServiceMock.Setup(x => x.JoinGroupAsync(2, groupId)).ReturnsAsync(false);

            // Act
            var result = await _userController.JoinGroup(groupId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to join the group.", badRequestResult.Value);
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
            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            int groupId = 1;

            // Act
            var result = await _userController.ExitGroup(groupId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _userServiceMock.Verify(x => x.ExitGroupAsync(groupId, 2), Times.Once);
        }

        [Fact]
        public async Task DeleteGroup_ValidInput_ReturnsNoContent()
        {
            // Arrange
            int groupId = 1;

            // Act
            var result = await _userController.DeleteGroup(groupId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            _userServiceMock.Verify(x => x.DeleteGroupAsync(groupId), Times.Once);
        }

        [Fact]
        public async Task GetUserBalance_ValidUserId_ReturnsOkWithBalance()
        {
            // Arrange
            int userId = 1;
            var balance = new BalanceDto { AmountOwed = 50, AmountOwedTo = 20 };
            _userServiceMock.Setup(x => x.GetUserBalanceAsync(userId)).ReturnsAsync(balance);

            // Act
            var result = await _userController.GetUserBalance(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(balance, okResult.Value);
        }

        [Fact]
        public async Task GetUserBalance_InvalidUserId_ReturnsNotFound()
        {
            // Arrange
            int userId = 1;
            _userServiceMock.Setup(x => x.GetUserBalanceAsync(userId)).ReturnsAsync((BalanceDto)null);

            // Act
            var result = await _userController.GetUserBalance(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOkWithUsers()
        {
            // Arrange
            var users = new List<User> { new User { UserId = 1, Email = "user1@example.com" } };
            _userServiceMock.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _userController.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(users, okResult.Value);
        }
    }
}

