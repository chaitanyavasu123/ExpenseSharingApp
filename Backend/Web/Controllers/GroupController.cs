using BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Web.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet("GetAllGroups")]
        public async Task<ActionResult<IEnumerable<Group>>> GetAllGroups()
        {
            var groups = await _groupService.GetAllGroupsAsync();
            return Ok(groups);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateGroup([FromBody] Group group)
        {
            try
            {
                await _groupService.CreateGroupAsync(group);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{groupId}")]
        [Authorize]
        public async Task<ActionResult<Group>> GetGroup(int groupId)
        {
            try
            {
                var group = await _groupService.GetGroupByIdAsync(groupId);
                if (group == null)
                {
                    return NotFound();
                }

                return Ok(group);
            }
            catch (Exception ex)
            {
                // Log the detailed error
                //_logger.LogError(ex, "Error fetching group details for GroupId: {groupId}", groupId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{groupId}/delete")]
        [Authorize]
        public async Task<ActionResult> DeleteGroup(int groupId)
        {
            var currentUserId = int.Parse(User.FindFirst("UserId").Value);// Retrieve the current user's ID from the authentication context, e.g., JWT token
            try
            {
                var result = await _groupService.DeleteGroupAsync(groupId, currentUserId);
                if (result)
                {
                    return Ok(new { message = "Group deleted successfully" });
                }
                return Forbid("You are not authorized to delete this group or the group does not exist");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{groupId}/invite")]
        [Authorize]
        public async Task<ActionResult> InviteUserToGroup(int groupId, int userId)
        {
            try
            {
                var result = await _groupService.InviteUserToGroupAsync(groupId, userId);
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

        [HttpPost("{groupId}/exit")]
        [Authorize]
        public async Task<ActionResult> ExitGroup(int groupId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId").Value);
                await _groupService.ExitGroupAsync(groupId, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user-not-in/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetGroupsUserIsNotIn(int userId)
        {
            try
            {


                var groups = await _groupService.GetGroupsUserIsNotInAsync(userId);
                if (groups == null || !groups.Any())
                {
                    return NotFound(new { message = "No groups found" });
                }
                return Ok(groups);


            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
