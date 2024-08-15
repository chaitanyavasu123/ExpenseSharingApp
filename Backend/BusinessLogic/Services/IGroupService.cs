using System;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IGroupService
    {
        Task CreateGroupAsync(Group group);
        Task<Group> GetGroupByIdAsync(int groupId);
        Task<bool> DeleteGroupAsync(int groupId,int currentUserId);
        Task<bool> InviteUserToGroupAsync(int groupId, int userId);
        Task ExitGroupAsync(int groupId, int userId);
        Task<IEnumerable<Group>> GetAllGroupsAsync();
        Task<List<Group>> GetGroupsUserIsNotInAsync(int userId);
    }
}
