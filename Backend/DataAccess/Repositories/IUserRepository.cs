using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int userId);
        Task<IEnumerable<Group>> GetUserGroupsAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<List<int>> GetUserIdsByGroupIdAsync(int groupId);
        Task SaveChangesAsync();
    }
}
