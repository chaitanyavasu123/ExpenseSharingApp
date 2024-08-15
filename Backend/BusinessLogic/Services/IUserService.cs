using Models;
using Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<User> AuthenticateAsync(string email, string password);
        Task<IEnumerable<Group>> GetUserGroupsAsync(int userId);
        Task CreateGroupAsync(Group group, int userId);
        Task<bool> InviteUserToGroupAsync(int groupId, int userId);
        Task ExitGroupAsync(int groupId, int userId);
        Task DeleteGroupAsync(int groupId);
        Task<BalanceDto> GetUserBalanceAsync(int userId);
        Task LogoutAsync(int userId);
        Task<bool> JoinGroupAsync(int userId, int groupId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<List<int>> GetUserIdsByGroupIdAsync(int groupId);
    }
}
