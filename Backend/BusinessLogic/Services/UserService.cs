using DataAccess.Repositories;
using Models;
using Models.Dtos;

namespace BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IExpenseRepository _expenseRepository;

        public UserService(IUserRepository userRepository, IGroupRepository groupRepository, IExpenseRepository expenseRepository)
        {
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _expenseRepository = expenseRepository;
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || user.Password != password)
                return null;

            // Password validation should use a hash in a real application

            return user;
        }

        public async Task<IEnumerable<Group>> GetUserGroupsAsync(int userId)
        {
            return await _userRepository.GetUserGroupsAsync(userId);
        }

        public async Task CreateGroupAsync(Group group, int userId)
        {
            // Add the user creating the group as a member of the group
            var groupMember = new GroupMember
            {
                UserId = userId,
                Group = group
            };
            var user = await _userRepository.GetUserByIdAsync(userId);
            group.Members.Add(groupMember);

            await _groupRepository.AddGroupAsync(group);
        }


        public async Task<bool> InviteUserToGroupAsync(int groupId, int userId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null || group.Members.Count >= 10)
                return false;

            if (group.Members.Any(m => m.UserId == userId))
                return false;

            group.Members.Add(new GroupMember { GroupId = groupId, UserId = userId });
            await _groupRepository.UpdateGroupAsync(group);
            return true;
        }

        public async Task ExitGroupAsync(int groupId, int userId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                return;

            var member = group.Members.SingleOrDefault(m => m.UserId == userId);
            if (member != null)
            {
                group.Members.Remove(member);
                await _groupRepository.UpdateGroupAsync(group);
            }
        }

        public async Task DeleteGroupAsync(int groupId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                return;

            await _groupRepository.DeleteGroupAsync(group);
        }

        public async Task<BalanceDto> GetUserBalanceAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return null;

            return new BalanceDto
            {
                AmountOwed = user.AmountOwed,
                AmountOwedTo = user.AmountOwedTo
            };
        }

        public async Task LogoutAsync(int userId)
        {
            // Implement logout logic if needed
            await Task.CompletedTask;
        }
        public async Task<bool> JoinGroupAsync(int userId, int groupId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null) return false;

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return false;

            var groupMember = new GroupMember
            {
                GroupId = groupId,
                UserId = userId
            };

            group.Members.Add(groupMember);
            await _groupRepository.UpdateGroupAsync(group);

            return true;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }
        public async Task<List<int>> GetUserIdsByGroupIdAsync(int groupId)
        {
            return await _userRepository.GetUserIdsByGroupIdAsync(groupId);
        }

    }
}
