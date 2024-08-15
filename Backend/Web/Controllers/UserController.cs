using BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Dtos;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {

            
            var user = await _userService.AuthenticateAsync(loginDto.Email, loginDto.Password);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Generate JWT token here
            var token = GenerateJwtToken(user);

            return Ok(new { Token = token, User = user });
        }
        catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message});
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {

            var userId = int.Parse(User.FindFirst("UserId").Value);
            try
            {
                await _userService.LogoutAsync(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{userId}/groups")]
        public async Task<ActionResult> GetUserGroups(int userId)
        {
            try
            {
                var groups = await _userService.GetUserGroupsAsync(userId);
                if (groups == null || !groups.Any())
                {
                    return NotFound();
                }

                // Use DTOs to avoid serialization issues
                var groupDtos = groups.Select(g => new GroupDto
                {
                    GroupId = g.GroupId,
                    Name = g.Name,
                    Description = g.Description,
                    CreatedDate = g.CreatedDate,
                    CreatedById = g.CreatedById
                }).ToList();

                return Ok(groupDtos);
            }
            catch (Exception ex)
            {
                // Log the exception
                //_logger.LogError(ex, "An error occurred while getting user groups.");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("CreateGroup")]
        [Authorize]
        public async Task<ActionResult> CreateGroup([FromBody] Group group)
        {
            var userIdClaim = User.FindFirst("UserId");
            try
            {
                if (userIdClaim == null)
                {
                    return Unauthorized("UserId claim not found in token");
                }
                var userId = int.Parse(userIdClaim.Value);
                //var user = await _userService.GetUserByIdAsync(userId);
                //group.CreatedBy = user;
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _userService.CreateGroupAsync(group, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("groups/{groupId}/invite")]
        [Authorize]
        public async Task<ActionResult> InviteUserToGroup(int groupId, int userId)
        {
            try
            {
                var result = await _userService.InviteUserToGroupAsync(groupId, userId);
                if (!result)
                {
                    return BadRequest("Failed to add user to group or group already has 10 members.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("groups/{groupId}/join")]
        [Authorize]
        public async Task<ActionResult> JoinGroup(int groupId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value);
                var result = await _userService.JoinGroupAsync(userId, groupId);
                if (!result) return BadRequest("Failed to join the group.");

                return Ok("Successfully joined the group.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("groups/{groupId}/exit")]
        [Authorize]
        public async Task<ActionResult> ExitGroup(int groupId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId").Value);
                await _userService.ExitGroupAsync(groupId, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("groups/{groupId}")]
        [Authorize]
        public async Task<ActionResult> DeleteGroup(int groupId)
        {
            await _userService.DeleteGroupAsync(groupId);
            return NoContent();
        }

        [HttpGet("{userId}/balance")]
        [Authorize]
        public async Task<ActionResult<BalanceDto>> GetUserBalance(int userId)
        {
            var balance = await _userService.GetUserBalanceAsync(userId);
            if (balance == null)
            {
                return NotFound("User not found.");
            }

            return Ok(balance);
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
